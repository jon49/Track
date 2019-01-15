namespace Teams

module Controller =

    open Microsoft.AspNetCore.Http
    open Saturn
    open Giraffe
    open FSharp.Control.Tasks
    open Model
    open Utils.ViewEngine
    open Track.Authentication

    let indexAction (ctx : HttpContext) =
        let layoutOptions = {
            App.IsAuthenticated = true
        }
        task {
            match! Database.all () with
            | Ok result ->
                return App.layout layoutOptions <| View.all result
            | Error error ->
                return InternalError.layout (error)
        }

    let addTeamForm (ctx : HttpContext) =
        task { return View.addEditTeamForm None }

    let createTeam (ctx : HttpContext) =
        task {
            let! data = Controller.getForm<Model.Base> ctx
            match! Database.add data with
            | Ok _ -> return View.addTeamButton
            | Error error -> return InternalError.layout error
        }

    let editTeam teamId (ctx : HttpContext) userId =
        task {
            match! Database.find teamId userId with
            | Ok x -> return View.addEditTeamForm x
            | Error error -> return InternalError.layout error
        }

    let updateTeam teamId (ctx : HttpContext) userId =
        task {
            //let! teamData = fetchModel< ctx.BindFormAsync<Model.Base>()
            let! teamData = Controller.getForm<Model.Base> ctx
            let team = {
                TeamId = teamId
                UserId = userId
                Data = teamData
            }
            match! Database.update team with
            | Ok x ->
                ctx.SetHttpHeader <|| Header.``IC-Trigger`` (Url.show teamId userId)
                return View.addTeamButton
            | Error error -> return InternalError.layout error
        }

    let getTeam teamId (ctx : HttpContext) userId =
        task {
            match! Database.find teamId userId with
            | Ok (Some x) -> return Giraffe.GiraffeViewEngine.renderHtmlNodes (View.team x)
            | Ok None -> return Giraffe.GiraffeViewEngine.renderHtmlNode NotFound.layout
            | Error error -> return Giraffe.GiraffeViewEngine.renderHtmlNode (InternalError.layout error)
        }

    let getLatestAddedTeam : HttpHandler =
        fun _ ctx ->
            task {
                let! latest = Database.getLastest ()
                let result =
                    match latest with
                    | Ok x -> View.teamRow x
                    | Error error ->
                        InternalError.layout (new System.Exception "Error")
                let! html = Controller.renderHtml ctx result
                return html
            }

    let usersController teamId = controller {
        show (getTeam teamId)
        edit (editTeam teamId)
        update (updateTeam teamId)
    }

    let teamsCustomEndpoints = router {
        get "/add-button" (htmlView View.addTeamButton)
        get "/latest" getLatestAddedTeam
    }

    let teamsController = controller {
        subController "/users" usersController
        index indexAction
        add addTeamForm 
        create createTeam
    }
