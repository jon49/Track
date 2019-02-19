namespace Track.AspNet

module AspNet =
    open Microsoft.AspNetCore.Http
    open System.Security.Claims

    let getClaim (ctx : HttpContext) (claim : string) =
        ctx.User.FindFirst(claim)
        |> Option.ofObj
        |> Option.map(fun x -> x.Value)
        |> Option.defaultValue ""

    type AuthId =
        | AuthId of string
        override __.ToString() =
            match __ with
            | AuthId s -> s

    let getAuth0Id (ctx : HttpContext) =
        AuthId <| getClaim ctx ClaimTypes.NameIdentifier
    
    let getUserNames =
        fun (ctx : HttpContext) ->
        let firstName = getClaim ctx ClaimTypes.GivenName
        let lastName = getClaim ctx ClaimTypes.Surname
        firstName, lastName

    let isAuthenticated (ctx : HttpContext) =
        ctx.User.Identity.IsAuthenticated

