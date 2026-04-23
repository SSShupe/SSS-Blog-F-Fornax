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
        siteInfo |> Option.map (fun si -> si.description) |> Option.defaultValue ""

    let siteUrl =
        siteInfo |> Option.map (fun si -> si.siteUrl) |> Option.defaultValue ""

    let plainDesc =
        System.Text.RegularExpressions.Regex.Replace(post.summary, "<[^>]*>", "").Trim()
        |> fun s -> if s.Length <= 160 then s else s.[..156] + "..."

    let ogMeta =
        [ yield "property", "og:type", "article"
          yield "property", "og:title", post.title
          yield "property", "og:url", siteUrl + post.link
          yield "property", "og:description", plainDesc
          yield!
              post.image
              |> Option.map (fun img -> [ "property", "og:image", siteUrl + img ])
              |> Option.defaultValue []
          yield
              "name",
              "twitter:card",
              (if post.image.IsSome then
                   "summary_large_image"
               else
                   "summary")
          yield "name", "twitter:title", post.title
          yield "name", "twitter:description", plainDesc
          yield!
              post.image
              |> Option.map (fun img -> [ "name", "twitter:image", siteUrl + img ])
              |> Option.defaultValue [] ]

    Layout.layout
        ctx
        post.title
        ogMeta
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
                          [ Layout.postLayout false post
                            Layout.shareSection (siteUrl + post.link) post.title ] ] ] ]

let generate (ctx: SiteContents) (projectRoot: string) (page: string) = generate' ctx page |> Layout.render ctx
