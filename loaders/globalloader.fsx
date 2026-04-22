#r "../_lib/Fornax.Core.dll"

type SiteInfo =
    { title: string
      description: string
      postPageSize: int
      siteUrl: string }

let loader (projectRoot: string) (siteContent: SiteContents) =
    let siteInfo =
        { title = "SSS Blog (New Version)"
          description = "SSS Blog (New Version)"
          postPageSize = 5
          siteUrl = "https://new.ssshupe.com" }

    siteContent.Add(siteInfo)

    siteContent
