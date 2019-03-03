module App

open Giraffe.GiraffeViewEngine
open Utils.ViewEngine
open Track.User

let layout user (content: XmlNode list) =

    let authenticatedNavigation =
        match user |> Option.map (fun x -> x.Type) |> Option.defaultValue UnknownUser with
        | Registered | Authenticated ->
            li [ _id "btn-logout" ] [ button [ _icPostTo "/logout" ] [ rawText "Logout" ] ]
        | UnknownUser ->
            li [ _id "btn-login" ] [ a [ _href "/login" ] [ rawText "Login" ] ]

    html [] [
        head [] [
            meta [_charset "utf-8"]
            meta [_name "viewport"; _content "width=device-width, initial-scale=1.0" ]
            title [] [ rawText "Track" ]
            link [ _rel "stylesheet"; _href "https://unpkg.com/picnic" ]
            link [ _href "/site.css"; _rel "stylesheet" ]
        ]
        body [] [
            header [] [
                h1 [] [ rawText "Track &amp; Field" ]
                nav [] [
                    ul [] [
                        authenticatedNavigation
                    ]
                ]
            ]
            main [] content
            script [ _src "https://code.jquery.com/jquery-3.1.1.min.js" ] []
            script [ _src "https://intercoolerreleases-leaddynocom.netdna-ssl.com/intercooler-1.2.1.min.js" ] []
            script [ _src "/app.js" ] []
        ]
    ]
