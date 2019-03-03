module Index

open Giraffe.GiraffeViewEngine
open Microsoft.AspNetCore.Http
open Saturn
open FSharp.Control.Tasks
open Track.Pages
open Track.AspNet
open Track.Repository
open Track

let indexLayout (ctx : HttpContext) =
    task {
        let isAuthenticated = AspNet.isAuthenticated ctx
        let! userResult = User.getUserPermissions <| AspNet.getAuth0Id ctx
        match userResult with
        | Error _ -> return Controller.renderHtml ctx (App.layout None (Welcome.View User.UnknownUser))
        | Ok user when user.Roles.Contains(User.Role.Coordinator) && not user.RegionIds.IsEmpty ->
            return Controller.redirect ctx "/teams"
        | Ok user when user.Roles.Contains(User.Role.Coach) && not user.TeamIds.IsEmpty ->
            return Controller.redirect ctx <| sprintf "/teams/%i" (Option.defaultValue (Set.maxElement user.TeamIds) user.PreferredTeamId)
        | Ok user ->
            return Controller.renderHtml ctx (App.layout (Some user) (Welcome.View User.Authenticated))
    }

let indexRouter = controller {
    index indexLayout
}
