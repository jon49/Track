namespace Track

[<AutoOpen>]
module Pipelines =
    open Microsoft.AspNetCore.Http
    open System.Threading.Tasks
    open Giraffe.GiraffeViewEngine
    open FSharp.Control.Tasks
    open Track.AspNet
    open Track.Repository
    open Saturn

    type HtmlFunc = HttpContext -> Track.User.T -> (XmlNode list -> Task<HttpContext option>) -> Task<HttpContext option>

    let htmlPipeline (action : HtmlFunc) (ctx : HttpContext) =
        task {
            match! User.getUserPermissions <| AspNet.getAuth0Id ctx with
            | Some user ->
                let next list = Controller.renderHtml ctx (App.layout user.Type list)
                return! action ctx user next
            | None -> return! Controller.redirect ctx "/"
        }


