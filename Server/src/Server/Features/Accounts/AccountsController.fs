namespace Accounts

module Controller =
    open Microsoft.AspNetCore.Http.Authentication
    open Giraffe
    open Microsoft.AspNetCore.Authentication
    open FSharp.Control.Tasks.V2.ContextInsensitive
    open Saturn
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
            if ctx.Request.Host.HasValue && not <| ctx.Request.Host.Value.Contains("/") then
                // TODO this doesn't work. It just shows up as literally '""'
                Response.ok ctx ""
            else redirectTo false "/" next ctx

    let accounts = router {
        get "/login" login
        post "/logout" logout
    }
