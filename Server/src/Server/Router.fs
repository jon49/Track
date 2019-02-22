module Router

open Saturn
open Giraffe.Core
open Giraffe.ResponseWriters
open Track
open Track.Controller

let browser = pipeline {
    plug (mustAccept ["text/html"; "text/html-partial"])
    plug putSecureBrowserHeaders
    plug fetchSession
    set_header "x-pipeline-type" "Browser"
}

let insecureRoutes = router {
    forward "" Index.indexRouter
    forward "" Accounts.Controller.accounts
    get "/index.html" (redirectTo false "/")
    get "/default.html" (redirectTo false "/")
}

let ``404`` = router {
    not_found_handler (htmlView NotFound.layout) //Use the default 404 webpage
}

let authenticated = pipeline {
    requires_authentication (Giraffe.Auth.challenge "Auth0")
    plug Register.user
}

let securedRoutes = router {
    pipe_through browser //Use the default browser pipeline
    pipe_through authenticated

    forward "/first-time" FirstTime.coordinator
    forward "/regions" Regions.Controller.regionsController
    forward "/teams" Teams.Controller.teamsController
    //forward "/teams" Teams.Controller.teamsCustomEndpoints
}

let allRouters = router {
    forward "" insecureRoutes
    forward "" securedRoutes
    forward "" ``404``
}
