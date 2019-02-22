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

    let htmlPipeline (action : HttpContext -> Track.User.T -> Task<XmlNode list>) (ctx : HttpContext) =
        task {
            match! User.getUserPermissions <| AspNet.getAuth0Id ctx with
            | Some user ->
                let! actionResult = action ctx user
                return! Controller.renderHtml ctx <| (App.layout user.Type actionResult)
            | None -> return! Controller.redirect ctx "/"
        }


