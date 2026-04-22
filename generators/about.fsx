#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html

let about =
    "I am a sort-of Iowa native now living in the Berkeley Hills with my wife Suzie and our dog Sparky. I graduated from the University of Iowa with a B.A. in political science and history, and received a J.D. from Cornell University. For the largest stretch of my legal career I worked in the Office of the Sonoma County Counsel, mostly on water, environmental, and energy issues. I helped form the Sonoma Clean Power Authority (the second CCA entity in California) and later served as its General Counsel. I'm a francophile (Suzie and I live part-time in La Rochelle, France), photographer, amateur coder, sports nut (Hawkeyes, Cal Bears, Bay Area teams), and current events junkie. I'm politically moderate but strongly anti-Trump and anti-MAGA. I hope you enjoy at least some of what you read on my blog."

let generate' (ctx: SiteContents) (_: string) =
    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo>()

    let desc =
        siteInfo
        |> Option.map (fun si -> si.description)
        |> Option.defaultValue ""


    Layout.layout
        ctx
        "About"
        []
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
                      div [ Class "card article" ] [
                          div [ Class "card-content" ] [
                              div [ Class "content article-body" ] [
                                  !!about
                              ]
                          ]
                      ]
                  ]
              ]
          ] ]

let generate (ctx: SiteContents) (projectRoot: string) (page: string) = generate' ctx page |> Layout.render ctx
