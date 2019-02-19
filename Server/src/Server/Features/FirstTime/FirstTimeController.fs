namespace Track.Controller

module FirstTime =
    open FSharp.Control.Tasks
    open Giraffe
    open Giraffe.GiraffeViewEngine
    open Saturn
    open Track.FirstTime
    open System
    open Track.Repository
    open Track.AspNet

    let newCoordinator (guid : Guid) : HttpHandler =
        fun _ ctx ->
        task {
            match! User.getUserPermissions <| AspNet.getAuth0Id ctx with
            | Some x ->
                match! Model.getNewCoordinatorRegionId guid x.UserId with
                | Some x ->
                    match x.HttpCode with
                    | 200 -> return! (Controller.redirect ctx "/teams")
                    | _ ->
                        ctx.Response.StatusCode <- x.HttpCode
                        return! Controller.renderHtml ctx (div [] [ rawText x.Message ])
                | None -> return! Controller.renderHtml ctx (NotFound.layout)
            | None -> return! Controller.renderHtml ctx (div [] [ rawText "Welcome! You haven't been registered. Something wrong happened; Consider emailing me at nyman.jon+track@gmail.com" ])
        }

    let newCoach (guid : Guid) : HttpHandler =
        fun _ ctx ->
        task {
            match! User.getUserPermissions <| AspNet.getAuth0Id ctx with
            | Some x ->
                match! Model.getNewCoacTeamId guid x.UserId with
                | Some x ->
                    match x.HttpCode with
                    | 200 ->
                        return! (Controller.redirect ctx <| sprintf "/teams/%i" x.TeamId)
                    | _ ->
                        ctx.Response.StatusCode <- x.HttpCode
                        return! Controller.renderHtml ctx (div [] [ rawText x.Message ])
                | None -> return! Controller.renderHtml ctx (NotFound.layout)
            | None -> return! Controller.renderHtml ctx (div [] [ rawText "Welcome! You haven't been registered. Something wrong happened; Consider emailing me at nyman.jon+track@gmail.com" ])
        }

    let coordinator = router {
        getf "/coordinator/%O" newCoordinator
        getf "/coach/%O" newCoach
    }

