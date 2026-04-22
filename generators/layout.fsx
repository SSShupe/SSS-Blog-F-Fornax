#r "../_lib/Fornax.Core.dll"
#if !FORNAX
#load "../loaders/postloader.fsx"
#load "../loaders/pageloader.fsx"
#load "../loaders/globalloader.fsx"
#endif

open Html

let injectWebsocketCode (webpage:string) =
    let websocketScript =
        """
        <script type="text/javascript">
          var wsUri = "ws://localhost:8080/websocket";
      function init()
      {
        websocket = new WebSocket(wsUri);
        websocket.onclose = function(evt) { onClose(evt) };
      }
      function onClose(evt)
      {
        console.log('closing');
        websocket.close();
        document.location.reload();
      }
      window.addEventListener("load", init, false);
      </script>
        """
    let head = "<head>"
    let index = webpage.IndexOf head
    webpage.Insert ( (index + head.Length + 1),websocketScript)

let layout (ctx : SiteContents) active (ogMeta: (string * string * string) list) bodyCnt =
    let pages = ctx.TryGetValues<Pageloader.Page> () |> Option.defaultValue Seq.empty
    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo> ()
    let ttl =
      siteInfo
      |> Option.map (fun si -> si.title)
      |> Option.defaultValue ""

    let menuEntries =
      pages
      |> Seq.map (fun p ->
        let cls = if p.title = active then "navbar-item is-active" else "navbar-item"
        a [Class cls; Href p.link] [!! p.title ])
      |> Seq.toList

    html [] [
        head [] [
            meta [CharSet "utf-8"]
            meta [Name "viewport"; Content "width=device-width, initial-scale=1"]
            title [] [!! ttl]
            link [Rel "icon"; Type "image/png"; Sizes "32x32"; Href "/images/favicon.png"]
            link [Rel "stylesheet"; Href "https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css"]
            link [Rel "stylesheet"; Href "https://fonts.googleapis.com/css?family=Open+Sans"]
            link [Rel "stylesheet"; Href "https://fonts.googleapis.com/css2?family=Lora:ital,wght@0,400..700;1,400..700&display=swap"]
            link [Rel "stylesheet"; Href "https://unpkg.com/bulma@0.8.0/css/bulma.min.css"]
            link [Rel "stylesheet"; Type "text/css"; Href "/style/style.css"]
            yield! ogMeta |> List.map (fun (attrName, attrVal, content) ->
                meta [HtmlProperties.Custom(attrName, attrVal); Content content])
        ]
        body [] [
          nav [Class "navbar"] [
            div [Class "container"] [
              div [Class "navbar-brand"] [
                a [Class "navbar-item"; Href "/"] [
                  img [Src "/images/bulma.png"; Alt "Logo"]
                ]
                span [Class "navbar-burger burger"; HtmlProperties.Custom ("data-target", "navbarMenu")] [
                  span [] []
                  span [] []
                  span [] []
                ]
              ]
              div [Id "navbarMenu"; Class "navbar-menu"] menuEntries
            ]
          ]
          yield! bodyCnt
          footer [Class "site-footer"] [
              div [Class "container has-text-centered"] [
                  p [] [!! "Copyright Steve Shupe — All Rights Reserved"]
                  p [] [
                      !! "Built with "
                      a [Href "https://fsharp.org"; HtmlProperties.Custom("target","_blank"); HtmlProperties.Custom("rel","noopener noreferrer")] [!! "F#"]
                      !! " and "
                      a [Href "https://ionide.io/Tools/fornax.html"; HtmlProperties.Custom("target","_blank"); HtmlProperties.Custom("rel","noopener noreferrer")] [!! "Fornax"]
                  ]
              ]
          ]
        ]
    ]

let render (ctx : SiteContents) cnt =
  let disableLiveRefresh = ctx.TryGetValue<Postloader.PostConfig> () |> Option.map (fun n -> n.disableLiveRefresh) |> Option.defaultValue false
  cnt
  |> HtmlElement.ToString
  |> fun n -> if disableLiveRefresh then n else injectWebsocketCode n

let published (post: Postloader.Post) =
    post.published
    |> Option.defaultValue System.DateTime.Now
    |> fun n -> n.ToString("yyyy-MM-dd")

let shareSection (pageUrl: string) (pageTitle: string) =
    let enc (s: string) = System.Uri.EscapeDataString s
    let fbUrl      = sprintf "https://www.facebook.com/sharer/sharer.php?u=%s" (enc pageUrl)
    let twUrl      = sprintf "https://twitter.com/intent/tweet?url=%s&text=%s" (enc pageUrl) (enc pageTitle)
    let bskyUrl    = sprintf "https://bsky.app/intent/compose?text=%s%%20%s" (enc pageTitle) (enc pageUrl)
    let threadsUrl = sprintf "https://www.threads.net/intent/post?text=%s%%20%s" (enc pageTitle) (enc pageUrl)
    let emailUrl   = sprintf "mailto:?subject=%s&body=%s" (enc pageTitle) (enc pageUrl)
    div [Class "share-section"] [
        p [Class "share-label"] [!! "Share this:"]
        a [Href fbUrl;      Class "share-btn"; HtmlProperties.Custom("target","_blank"); HtmlProperties.Custom("rel","noopener noreferrer")] [!! "Facebook"]
        a [Href bskyUrl;    Class "share-btn"; HtmlProperties.Custom("target","_blank"); HtmlProperties.Custom("rel","noopener noreferrer")] [!! "BlueSky"]
        a [Href threadsUrl; Class "share-btn"; HtmlProperties.Custom("target","_blank"); HtmlProperties.Custom("rel","noopener noreferrer")] [!! "Threads"]
        a [Href twUrl;      Class "share-btn"; HtmlProperties.Custom("target","_blank"); HtmlProperties.Custom("rel","noopener noreferrer")] [!! "Twitter"]
        a [Href emailUrl;   Class "share-btn"] [!! "Email"]
        button [Class "share-btn";
                HtmlProperties.Custom("onclick", "navigator.clipboard.writeText(window.location.href).then(function(){var b=event.target;b.textContent='Copied!';setTimeout(function(){b.textContent='Copy Link'},2000)})")] [
            !! "Copy Link"
        ]
    ]

let postLayout (useSummary: bool) (post: Postloader.Post) =
    div [Class "card article"] [
        div [Class "card-content"] [
            div [Class "media-content has-text-centered"] [
                p [Class "title article-title"; ] [ a [Href post.link] [!! post.title]]
                p [Class "subtitle is-6 article-subtitle"] [
                a [Href "#"] [!! (defaultArg post.author "")]
                !! (sprintf "on %s" (published post))
                ]
            ]
            match post.image with                                                                                                                                      
              | Some imgSrc ->
                  div [Class "article-image"] [
                      img [Src imgSrc; Alt post.title]                                                                                                                   
                      match post.imageCaption with
                      | Some caption -> p [Class "image-caption"] [!! caption]                                                                                           
                      | None         -> div [] []
                  ]                                                                                                                                                      
              | None -> div [] []
            div [Class "content article-body"] [
                !! (if useSummary then post.summary else post.content)

            ]
        ]
    ]
