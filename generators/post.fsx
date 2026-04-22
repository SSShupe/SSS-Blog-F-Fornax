#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html


let generate' (ctx: SiteContents) (page: string) =
    let post =
        ctx.TryGetValues<Postloader.Post>()
        |> Option.defaultValue Seq.empty
        |> Seq.find (fun n -> n.file = page)

    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo>()

    let desc =
        siteInfo
        |> Option.map (fun si -> si.description)
        |> Option.defaultValue ""

    let siteUrl =
        siteInfo
        |> Option.map (fun si -> si.siteUrl)
        |> Option.defaultValue ""

    let ogMeta =
        [ yield "og:type",  "article"
          yield "og:title", post.title
          yield "og:url",   siteUrl + post.link
          yield! post.image |> Option.map (fun img -> [ "og:image", siteUrl + img ]) |> Option.defaultValue [] ]

    Layout.layout
        ctx
        post.title
        ogMeta
        [ section [ Class "hero is-info is-medium is-bold" ] [
              div [ Class "hero-body" ] [
                  div [ Class "container has-text-centered" ] [
                      h1 [ Class "title has-text-black" ] [
                          !!desc
                      ]
                  ]
              ]
          ]
          div [ Class "container" ] [
              section [ Class "articles" ] [
                  div [ Class "column is-8 is-offset-2" ] [
                      Layout.postLayout false post
                      Layout.shareSection (siteUrl + post.link) post.title
                  ]
              ]
          ] ]

let generate (ctx: SiteContents) (projectRoot: string) (page: string) = generate' ctx page |> Layout.render ctx
