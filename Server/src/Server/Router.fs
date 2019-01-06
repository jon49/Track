module Router

open Saturn
open Giraffe.Core
open Giraffe.ResponseWriters
open Track.Settings
open System.Security.Claims
open System.IdentityModel.Tokens.Jwt
open System
open Saturn
open Microsoft.IdentityModel.Tokens
open Microsoft.AspNetCore.Http
open Giraffe.Core
open Giraffe
open FSharp.Control.Tasks

let browser = pipeline {
    plug (mustAccept ["text/html"; "text/html-partial"])
    plug putSecureBrowserHeaders
    plug fetchSession
    set_header "x-pipeline-type" "Browser"
}

let defaultView = router {
    get "/" (htmlView Index.layout)
    get "/index.html" (redirectTo false "/")
    get "/default.html" (redirectTo false "/")
}

let browserRouter = router {
    not_found_handler (htmlView NotFound.layout) //Use the default 404 webpage
    pipe_through browser //Use the default browser pipeline

    forward "" defaultView //Use the default view
    forward "/teams" Teams.Controller.teamsController
    forward "/teams" Teams.Controller.teamsCustomEndpoints
}

//let app = router {
//    //pipe_through (Auth.requireAuthentication ChallengeType.JWT)
//    forward "/teams" Teams.Controller.teamsController
//    forward "/teams" Teams.Controller.teamsCustomEndpoints
//}

//Other scopes may use different pipelines and error handlers

// let api = pipeline {
//     plug acceptJson
//     set_header "x-pipeline-type" "Api"
// }

// let apiRouter = router {
//     error_handler (text "Api 404")
//     pipe_through api
//
//     forward "/someApi" someScopeOrController
// }

let appRouter = router {
    // forward "/api" apiRouter
    forward "" browserRouter
}