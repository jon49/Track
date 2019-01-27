module App

open Giraffe.GiraffeViewEngine

type LayoutOptions = {
    IsAuthenticated : bool
}

let layout option (content: XmlNode list) =

    let authenticatedNavigation =
        if option.IsAuthenticated
            then li [ _id "btn-logout" ] [ a [ _href "/logout" ] [ rawText "Logout" ] ]
        else li [ _id "btn-login" ] [ a [ _href "/login" ] [ rawText "Login" ] ]

    html [] [
        head [] [
            meta [_charset "utf-8"]
            meta [_name "viewport"; _content "width=device-width, initial-scale=1.0" ]
            title [] [ encodedText "Track" ]
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
