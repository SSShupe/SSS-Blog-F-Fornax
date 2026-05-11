#r "../_lib/Fornax.Core.dll"
#r "../_lib/Markdig.dll"
#load "layout.fsx"

open Html
open System.IO
open Markdig

let markdownPipeline =
    MarkdownPipelineBuilder()
        .UsePipeTables()
        .UseGridTables()
        .Build()

let generate' (ctx: SiteContents) (projectRoot: string) (_: string) =
    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo>()

    let desc =
        siteInfo |> Option.map (fun si -> si.description) |> Option.defaultValue ""

    let aboutContent =
        let path = Path.Combine(projectRoot, "pages", "about.md")
        if File.Exists path then
            File.ReadAllText path |> fun t -> Markdown.ToHtml(t, markdownPipeline)
        else
            ""

    Layout.layout
        ctx
        "About"
        []
        [ section
              [ Class "hero is-info is-medium is-bold" ]
              [ div
                    [ Class "hero-body" ]
                    [ div
                          [ Class "container has-text-centered" ]
                          [ h1 [ Class "title has-text-white is-size-1" ] [ !!desc ] ] ] ]
          div
              [ Class "container" ]
              [ section
                    [ Class "articles" ]
                    [ div
                          [ Class "column is-8 is-offset-2" ]
                          [ div
                                [ Class "card article" ]
                                [ div [ Class "card-content" ] [ div [ Class "content article-body" ] [ !!aboutContent ] ] ] ] ] ] ]

let generate (ctx: SiteContents) (projectRoot: string) (page: string) = generate' ctx projectRoot page |> Layout.render ctx
