namespace Track.FirstTime

module Model =

    open Track.Repository
    open Track.User

    let getNewCoordinatorRegionId guid (UserID.ID userId) =
        async {
        use cmd = new DB.track_api.AddCoordinator(conn)
        return! cmd.AsyncExecuteSingle(guid, userId)
        }
        |> single

    let getNewCoachTeamId guid (UserID.ID userId) =
        async {
        use cmd = new DB.track_api.NewCoach(conn)
        return! cmd.AsyncExecuteSingle(guid, userId)
        }
        |> single

module Url =

    let index = "/first-time"

//module View =

module Controller =
    open FSharp.Control.Tasks
    open Giraffe
    open Giraffe.GiraffeViewEngine
    open Saturn
    open System
    open Track.Repository
    open Track.AspNet
    open Track

    let newCoordinator (guid : Guid) : HttpHandler =
        fun _ ctx ->
        task {
        match! User.getUserPermissions <| AspNet.getAuth0Id ctx with
        | Ok user ->
            match! Model.getNewCoordinatorRegionId guid user.UserId with
            | Ok x when x.HttpCode = 200 ->
                User.reset user
                return! (Controller.redirect ctx "/teams")
            | Ok x ->
                ctx.Response.StatusCode <- x.HttpCode
                return! Controller.renderHtml ctx (p [] [ rawText x.Message ])
            | _ -> return! Controller.renderHtml ctx (NotFound.layout)
        | Error _ -> return! Controller.renderHtml ctx (p [] [ rawText "Welcome! You haven't been registered. Something wrong happened; Consider emailing me at nyman.jon+track@gmail.com" ])
        }

    let newCoach (guid : Guid) : HttpHandler =
        fun _ ctx ->
        task {
            match! User.getUserPermissions <| AspNet.getAuth0Id ctx with
            | Ok x ->
                match! Model.getNewCoachTeamId guid x.UserId with
                | Ok x when x.HttpCode = 200 ->
                    return! (Controller.redirect ctx <| sprintf "/teams/%i" x.TeamId)
                | Ok x ->
                    ctx.Response.StatusCode <- x.HttpCode
                    return! Controller.renderHtml ctx (p [] [ rawText x.Message ])
                | _ -> return! Controller.renderHtml ctx (NotFound.layout)
            | Error _ -> return! Controller.renderHtml ctx (p [] [ rawText "Welcome! You haven't been registered. Something wrong happened; Consider emailing me at nyman.jon+track@gmail.com" ])
        }

    let coordinatorController = router {
        getf "/coordinator/%O" newCoordinator
        getf "/coach/%O" newCoach
    }

