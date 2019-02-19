module Index

open Giraffe.GiraffeViewEngine
open Microsoft.AspNetCore.Http
open Saturn
open FSharp.Control.Tasks
open Track.Pages
open Track.AspNet
open Track.Repository

let indexLayout (ctx : HttpContext) =
    task {
        let isAuthenticated = AspNet.isAuthenticated ctx
        let options = {
            App.IsAuthenticated = isAuthenticated
        }
        match! User.getUserPermissions <| AspNet.getAuth0Id ctx with
        | None -> return Controller.renderHtml ctx (App.layout options Welcome.View)
        | Some user when user.Roles.Contains(User.Role.Coordinator) ->
            return Controller.redirect ctx "/teams"
        | Some user when user.Roles.Contains(User.Role.Coach) ->
            return Controller.redirect ctx <| sprintf "/teams/%i" (user.TeamIds |> Set.maxElement)
        | Some user ->
            return Controller.renderHtml ctx (App.layout options Welcome.View)
    }

let indexRouter = controller {
    index indexLayout
}
