namespace Teams

module Controller =

    open Microsoft.AspNetCore.Http
    open Saturn
    open Giraffe
    open FSharp.Control.Tasks
    open Utils.ViewEngine
    open Utils.Client
    open Giraffe
    open Track

    let indexAction (ctx : HttpContext) =
        task {
            return [] // (App.layout User.Type.Authenticated [ GiraffeViewEngine.p [] [ GiraffeViewEngine.rawText "Hello!" ] ])
            //match! Repository.all () with
            //| Ok result ->
            //    return App.layout layoutOptions <| View.all result
            //| Error error ->
            //    return InternalError.layout (error)
        }

//    let addTeamForm (ctx : HttpContext) =
//        task { return View.addEditTeamForm None }

//    let editTeam teamId (ctx : HttpContext) userId =
//        task {
//            match! Repository.find teamId userId with
//            | Ok x -> return View.addEditTeamForm x
//            | Error error -> return InternalError.layout error
//        }

//    let updateTeam teamId (ctx : HttpContext) userId =
//        task {
//            let! team = Controller.getForm<Model.Team> ctx
//            let team = {
//                Id = teamId
//                Team = team
//            }
//            match! Repository.update team with
//            | Ok x ->
//                ctx.SetHttpHeader <|| Header.``IC-Trigger`` (Url.show teamId userId)
//                return View.addTeamButton
//            | Error error -> return InternalError.layout error
//        }

//    let getTeam teamId (ctx : HttpContext) userId =
//        task {
//            match! Repository.find teamId userId with
//            | Ok (Some x) -> return Giraffe.GiraffeViewEngine.renderHtmlNodes (View.team x)
//            | Ok None -> return Giraffe.GiraffeViewEngine.renderHtmlNode NotFound.layout
//            | Error error -> return Giraffe.GiraffeViewEngine.renderHtmlNode (InternalError.layout error)
//        }

//    let getLatestAddedTeam : HttpHandler =
//        fun _ ctx ->
//            task {
//                let! latest = Repository.getLastest ()
//                let result =
//                    match latest with
//                    | Ok x -> View.teamRow x
//                    | Error error ->
//                        InternalError.layout (new System.Exception "Error")
//                let! html = Controller.renderHtml ctx result
//                return html
//            }

//    let createTeam (ctx : HttpContext) =
//        task {
//            let! model = Controller.getForm<Data> ctx
//            let result =
//                model
//                |> validate
//                |> Result.map clean
//            match result with
//            | Ok x ->
//                match! Repository.add x with
//                | Ok _ ->
//                    ctx.Response.StatusCode <- StatusCodes.Status201Created
//                    return View.addTeamButton
//                | Error error ->
//                    ctx.Response.StatusCode <- StatusCodes.Status500InternalServerError
//                    return (str error)
//            | Error error ->
//                ctx.Response.StatusCode <- StatusCodes.Status400BadRequest
//                return (str error)
//        }

//    let usersController teamId = controller {
//        show (getTeam teamId)
//        edit (editTeam teamId)
//        update (updateTeam teamId)
//    }

//    let teamsCustomEndpoints = router {
//        get "/add-button" (htmlView View.addTeamButton)
//        get "/latest" getLatestAddedTeam
//    }

    let teamsController = controller {
        //subController "/users" usersController
        index indexAction
        //add addTeamForm 
        //create createTeam
    }
