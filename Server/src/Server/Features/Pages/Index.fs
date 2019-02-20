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
        let! maybeUser = User.getUserPermissions <| AspNet.getAuth0Id ctx
        let options = {
            App.IsAuthenticated = isAuthenticated
            App.IsRegistered = Option.isSome maybeUser
        }
        match maybeUser with
        | None -> return Controller.renderHtml ctx (App.layout options (Welcome.View options))
        | Some user when user.Roles.Contains(User.Role.Coordinator) && not user.RegionIds.IsEmpty ->
            return Controller.redirect ctx "/teams"
        | Some user when user.Roles.Contains(User.Role.Coach) && not user.TeamIds.IsEmpty ->
            return Controller.redirect ctx <| sprintf "/teams/%i" (Option.defaultValue (Set.maxElement user.TeamIds) user.PreferredTeamId)
        | Some user ->
            return Controller.renderHtml ctx (App.layout options (Welcome.View options))
    }

let indexRouter = controller {
    index indexLayout
}
