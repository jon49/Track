namespace Accounts

module Controller =
    open Microsoft.AspNetCore.Http
    open FSharp.Control.Tasks
    open Microsoft.AspNetCore.Http.Authentication
    open Giraffe
    open Microsoft.AspNetCore.Authentication
    open FSharp.Control.Tasks.V2.ContextInsensitive
    open Giraffe
    open Saturn
    open Track.Authentication
    open Track
    open Track.Repository
    open Track.AspNet

    let login : HttpHandler =
        fun next ctx ->
        task {
            do! ctx.ChallengeAsync("Auth0", AuthenticationProperties(RedirectUri = "/"))
            return! next ctx
        }

    let logout : HttpHandler =
        fun next ctx ->
            Cache.remove (User.cacheKey <| AspNet.getAuth0Id ctx)
            let result = signOut "Cookies" next ctx
            redirectTo false "/" next ctx

    let accounts = router {
        get "/login" login
        get "/logout" logout
    }
