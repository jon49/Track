module App

open Giraffe.GiraffeViewEngine
open Track.User
open Utils
open ViewEngine

let layout  =
    fun user (content: XmlNode list) ->
    let authenticatedNavigation =
        match user |> Option.map (fun x -> x.Type) |> Option.defaultValue UnknownUser with
        | Registered | Authenticated ->
            a [ _id "btn-logout"; _class (Class.M.Col.sm + Class.M.Col.md+ Class.M.button); _icPostTo "/logout"; ] [ rawText "Logout" ]
        | UnknownUser ->
            a [ _id "btn-login"; _class (Class.M.Col.sm + Class.M.Col.md + Class.M.button); _href "/login"; ] [ rawText "Login" ]

    html [] [
        head [] [
            meta [_charset "utf-8"]
            meta [_name "viewport"; _content "width=device-width, initial-scale=1.0" ]
            title [] [ rawText "Track" ]
            // link [ _rel "stylesheet"; _href "https://unpkg.com/picnic" ]
            link [ _rel "stylesheet"; _href "https://cdnjs.cloudflare.com/ajax/libs/mini.css/3.0.1/mini-default.min.css" ]
            link [ _href "/site.css"; _rel "stylesheet" ]
        ]
        body [] [
            header [ _class Class.M.row ] [
                a [ _href "/"; _class ( Class.M.logo + Class.M.Col.sm + Class.M.Col.md) ] [ rawText "Track &amp; Field" ]
                authenticatedNavigation ]
            main [] content
            
            script [ _src "https://code.jquery.com/jquery-3.1.1.min.js" ] []
            script [ _src "https://intercoolerreleases-leaddynocom.netdna-ssl.com/intercooler-1.2.1.min.js" ] []
            script [ _src "/app.js" ] []
        ]
    ]
