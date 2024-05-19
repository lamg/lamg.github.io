module NotebookToHtml

open System
open System.IO
open Newtonsoft.Json.Linq
open Markdig

open Fss
open Falco.Markup

let html, htmlNode = fss [ Custom "margin" "0"; Custom "padding" "0" ]

let basePage body =
  Elem.html
    [ Attr.lang "en"; Attr.class' html ]
    [ Elem.head
        []
        [ Elem.meta [ Attr.charset "UTF-8" ]
          Elem.title [] [ Text.raw "LAMG" ]
          Elem.link
            [ Attr.href "https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.5.1/styles/default.min.css"
              Attr.rel "stylesheet" ]
          Elem.script [ Attr.src "https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.5.1/highlight.min.js" ] []
          Elem.script [] [ Text.raw "hljs.highlightAll();" ] ]
      Elem.body [] body ]

let extractCode (cell: JToken) =
  let code = String.concat "\n" (cell.["source"].Values<string>())
  Elem.pre [] [ Elem.code [ Attr.class' "language-fsharp" ] [ Text.raw (Web.HttpUtility.HtmlEncode code) ] ]

let convert (inputPath: string) (outputPath: string) =
  // Read the Jupyter notebook JSON
  let notebookJson = File.ReadAllText inputPath
  let notebook = JObject.Parse(notebookJson)

  // Extract the cells from the notebook
  let cells = notebook.["cells"].AsJEnumerable()

  let html =
    cells
    |> Seq.choose (fun cell ->
      let cellType = cell.["cell_type"].Value<string>()

      match cellType with
      | "markdown" ->
        // Convert markdown to HTML
        let markdown = String.concat "\n" (cell.["source"].Values<string>())
        let html = Markdown.ToHtml(markdown)
        Text.raw html |> Some
      | "code" ->
        // Extract and highlight the F# code
        let code = String.concat "\n" (cell.["source"].Values<string>())

        Elem.pre [] [ Elem.code [ Attr.class' "language-fsharp" ] [ Text.raw code ] ]
        |> Some
      | _ -> None)
    |> Seq.toList
    |> basePage
    |> renderHtml

  // Write the HTML to the output file
  File.WriteAllText(outputPath, html)
