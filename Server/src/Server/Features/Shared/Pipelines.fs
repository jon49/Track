namespace Track

[<AutoOpen>]
module Pipelines =
    open Microsoft.AspNetCore.Http
    open FSharp.Control.Tasks
    open Track.AspNet
    open Track.Repository
    open Saturn
    open System.Threading.Tasks
    open Giraffe

    type UserContext = User.T -> HttpContext -> Task<HttpContext option>

    let userContext (action : UserContext) (ctx : HttpContext) =
        task {
            match! User.getUserPermissions <| AspNet.getAuth0Id ctx with
            | Ok user -> return! action user ctx
            | Error _ -> return! Controller.redirect ctx "/"
        }

    let userContextFunc action : HttpHandler =
        fun _ ctx -> userContext action ctx
