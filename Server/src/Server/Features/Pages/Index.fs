module Index

open Giraffe.GiraffeViewEngine
open Microsoft.AspNetCore.Http
open Saturn
open FSharp.Control.Tasks
open Track.Pages
open Track.AspNet
open Track.Repository
open Track
open App

let indexLayout (ctx : HttpContext) =
    task {
        let isAuthenticated = AspNet.isAuthenticated ctx
        let! userResult = User.getUserPermissions <| AspNet.getAuth0Id ctx
        match userResult, isAuthenticated with
        | Ok user, true when user.Roles.Contains(User.Role.Coordinator) && not user.RegionIds.IsEmpty ->
            return Controller.redirect ctx "/teams"
        | Ok user, true when user.Roles.Contains(User.Role.Coach) && not user.TeamIds.IsEmpty ->
            return Controller.redirect ctx <| sprintf "/teams/%i" (Option.defaultValue (Set.maxElement user.TeamIds) user.PreferredTeamId)
        | _, true ->
            return Controller.renderHtml ctx (App.layout UserLayout.Authenticated (Welcome.View User.Authenticated))
        | _, false -> return Controller.renderHtml ctx (App.layout UserLayout.Unknown (Welcome.View User.UnknownUser))
    }

let indexRouter = controller {
    index indexLayout
}
