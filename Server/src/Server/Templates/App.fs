module App

open Giraffe.GiraffeViewEngine
open Track.User
open Utils
open ViewEngine
module P = Class.P

[<Struct>]
type UserLayout = {
    User : T option
    Type : Type
} with
    static member Init(user, authenticated) =
        { User = user
          Type = authenticated }
    static member Authenticated =
        { User = None
          Type = Authenticated }
    static member Unknown =
        { User = None
          Type = UnknownUser }
    static member Init(user) =
        { User = Some user
          Type = Registered }

let layout  =
    let pseudoButtonClass = P.pseudo + P.button

    fun user (content: XmlNode list) ->
    let authenticatedNavigation =
        match user with
        | { User = None; Type = UnknownUser } ->
            a [ _id "btn-login"; _class pseudoButtonClass; _href "/login"; ] [ rawText "Login" ]
        | { User = None; Type = Authenticated } ->
            a [ _id "btn-logout"; _class pseudoButtonClass; _icPostTo "/logout"; ] [ rawText "Logout" ]
        | { User = Some user; Type = Registered } ->
            a [ _id "btn-logout"; _class pseudoButtonClass; _icPostTo "/logout"; ] [ rawText "Logout" ]
        | _ -> p [] [ rawText "How did you get here?" ]

    html [] [
        head [] [
            meta [_charset "utf-8"]
            meta [_name "viewport"; _content "width=device-width, initial-scale=1.0" ]
            title [] [ rawText "Track" ]
            link [ _rel "stylesheet"; _href "/styles/picnic.css" ]
            link [ _href "/styles/site.css"; _rel "stylesheet" ]
        ]
        body [] [
            header [] [
                nav [][
                    a [ _href "/"; _class (P.brand) ] [ rawText "Track &amp; Field" ]
                    input [ _id "bmenu"; _type "checkbox"; _class P.show ]
                    label [ _for "bmenu"; _class (P.burger + P.toggle + P.pseudo + P.button) ] [ rawText "menu" ]
                    div [ _class P.menu ] [
                        authenticatedNavigation ] ]
                ]
            main [] content
            
            script [ _src "/scripts/app.js" ] []
        ]
    ]
