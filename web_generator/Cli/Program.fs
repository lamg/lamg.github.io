open System
open System.IO

[<Literal>]
let indexHtml = "index.html"

let createLink (filePath: string) =
  let fileName = Path.ChangeExtension(Path.GetFileName filePath, null)
  sprintf "<li><a href=\"%s\">%s</a></li>" filePath fileName

let generate (inputDir: string) (outputDir: string) =
  let ipynbFiles = Directory.GetFiles(inputDir, "*.ipynb")
  let outputPath = Path.Join(Environment.CurrentDirectory, outputDir)

  try
    Directory.Delete outputPath
  with _ ->
    ()

    Directory.CreateDirectory outputPath |> ignore

  let htmlFiles =
    ipynbFiles
    |> Array.map (fun f ->
      let htmlFile = Path.ChangeExtension(Path.GetFileName f, "html")

      NotebookToHtml.convert f (Path.Join(outputPath, htmlFile))
      htmlFile)

  let links = htmlFiles |> Array.map createLink |> String.concat "\n"

  let htmlContent =
    sprintf
      """
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Index of Files</title>
            </head>
            <body>
                <h1>Index of Files</h1>
                <ul>
                    %s
                </ul>
            </body>
            </html>
            """
      links


  File.WriteAllText(Path.Join(outputPath, indexHtml), htmlContent)

[<EntryPoint>]
let main args =
  match args with
  | [| inputDir |] ->
    generate inputDir "docs"
    0
  | _ -> 1
