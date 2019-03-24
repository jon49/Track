namespace Track.Tests

module View =
    open Giraffe.GiraffeViewEngine

    let editTests id =
        form [ _method "get"; _action (sprintf "/tests/%i" id) ] [
            button [ _type "submit" ] [ rawText "Submit" ]
        ]

    let get id =
        div [] [ rawText (string id) ]

module Controller =
    open Saturn
    open Microsoft.AspNetCore.Http
    open FSharp.Control.Tasks

    let editValue (ctx : HttpContext) id = 
        task {
        return View.editTests id
        }

    let showValue (ctx : HttpContext) id =
        task {
        return View.get id
        }

    let endpoints = controller {
        edit editValue
        show showValue
    }
