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
open Track.Authentication

let browser = pipeline {
    plug (mustAccept ["text/html"; "text/html-partial"])
    plug putSecureBrowserHeaders
    plug fetchSession
    set_header "x-pipeline-type" "Browser"
}

let insecureRoutes = router {
    forward "" Index.indexRouter
    forward "" Accounts.Controller.accounts
    not_found_handler (htmlView NotFound.layout) //Use the default 404 webpage
    get "/index.html" (redirectTo false "/")
    get "/default.html" (redirectTo false "/")
}

let authenticated = pipeline {
    requires_authentication (Giraffe.Auth.challenge "Auth0")
}

let securedRoutes = router {
    pipe_through browser //Use the default browser pipeline
    pipe_through authenticated

    forward "/teams" Teams.Controller.teamsController
    forward "/teams" Teams.Controller.teamsCustomEndpoints
}

let allRouters = router {
    forward "" insecureRoutes
    forward "" securedRoutes
}
