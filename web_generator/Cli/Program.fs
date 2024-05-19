open System
open System.IO

open Fss
open Fss.Types
open Falco.Markup

let html, htmlNode = fss [ Custom "margin" "0"; Custom "padding" "0" ]

[<Literal>]
let indexHtml = "index.html"

let createLink (filePath: string) =
  let fileName = Path.ChangeExtension(Path.GetFileName filePath, null)
  Elem.li [] [ Elem.a [ Attr.href filePath ] [ Text.raw fileName ] ]

let generate (inputDir: string) (outputDir: string) =
  let ipynbFiles = Directory.GetFiles(inputDir, "*.ipynb") |> Array.toList
  let outputPath = Path.Join(Environment.CurrentDirectory, outputDir)

  try
    Directory.Delete outputPath
  with _ ->
    ()

    Directory.CreateDirectory outputPath |> ignore

  let htmlFiles =
    ipynbFiles
    |> List.map (fun f ->
      let htmlFile = Path.ChangeExtension(Path.GetFileName f, "html")

      NotebookToHtml.convert f (Path.Join(outputPath, htmlFile))
      htmlFile)


  let links = htmlFiles |> List.map createLink

  let htmlContent =
    Elem.html
      [ Attr.lang "en"; Attr.class' html ]
      [ Elem.head [] [ Elem.meta [ Attr.charset "UTF-8" ]; Elem.title [] [ Text.raw "LAMG" ] ]
        Elem.body [] [ Elem.li [] links ] ]
    |> renderHtml

  File.WriteAllText(Path.Join(outputPath, indexHtml), htmlContent)

[<EntryPoint>]
let main args =
  match args with
  | [| inputDir |] ->
    generate inputDir "docs"
    0
  | _ -> 1
