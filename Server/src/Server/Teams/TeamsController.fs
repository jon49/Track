namespace Teams

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.ContextInsensitive
open Saturn

module Controller =
    open Giraffe

    let indexAction (ctx : HttpContext) =
        task {
            match! Database.all () with
            | Ok result ->
                return App.layout <| View.all result
            | Error error ->
                return InternalError.layout (raise error)
        }

    let addTeamForm (ctx : HttpContext) =
        task { return View.addTeamForm None }

    //let usersController teamId = controller {
    //    show ()
    //}

    let teamsController = controller {
        index indexAction
        add addTeamForm 
        //show showAction
        //add addAction
        //edit editAction
        //create createAction
        //update updateAction
        //delete deleteAction
    }

    //let editTeamUser (ctx : HttpContext) =
    //    match Database.all () with
    //    | Ok result ->
    //        Views.index ctx (List.ofSeq result)
    //    | Error ex ->
    //        raise ex
