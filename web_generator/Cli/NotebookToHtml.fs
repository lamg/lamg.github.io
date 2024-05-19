module NotebookToHtml

open System.IO
open Newtonsoft.Json.Linq
open Markdig

let convert (inputPath: string) (outputPath: string) =
  // Read the Jupyter notebook JSON
  let notebookJson = File.ReadAllText inputPath
  let notebook = JObject.Parse(notebookJson)

  // Create an HTML string
  let htmlBuilder = System.Text.StringBuilder()
  htmlBuilder.AppendLine("<html>") |> ignore
  htmlBuilder.AppendLine("<head>") |> ignore
  htmlBuilder.AppendLine("<title>Blog Post</title>") |> ignore

  htmlBuilder.AppendLine(
    "<link href=\"https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.5.1/styles/default.min.css\" rel=\"stylesheet\">"
  )
  |> ignore

  htmlBuilder.AppendLine(
    "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.5.1/highlight.min.js\"></script>"
  )
  |> ignore

  htmlBuilder.AppendLine("<script>hljs.highlightAll();</script>") |> ignore
  htmlBuilder.AppendLine("</head>") |> ignore
  htmlBuilder.AppendLine("<body>") |> ignore

  // Extract the cells from the notebook
  let cells = notebook.["cells"].AsJEnumerable()

  for cell in cells do
    let cellType = cell.["cell_type"].Value<string>()

    match cellType with
    | "markdown" ->
      // Convert markdown to HTML
      let markdown = String.concat "\n" (cell.["source"].Values<string>())
      let html = Markdown.ToHtml(markdown)
      htmlBuilder.AppendLine(html) |> ignore
    | "code" ->
      // Extract and highlight the F# code
      let code = String.concat "\n" (cell.["source"].Values<string>())
      htmlBuilder.AppendLine("<pre><code class=\"language-fsharp\">") |> ignore
      htmlBuilder.AppendLine(System.Web.HttpUtility.HtmlEncode(code)) |> ignore
      htmlBuilder.AppendLine("</code></pre>") |> ignore
    | _ -> ()

  htmlBuilder.AppendLine("</body>") |> ignore
  htmlBuilder.AppendLine("</html>") |> ignore

  // Write the HTML to the output file
  File.WriteAllText(outputPath, htmlBuilder.ToString())
