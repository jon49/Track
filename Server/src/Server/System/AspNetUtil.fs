namespace Track.AspNet

module AspNet =
    open Microsoft.AspNetCore.Http
    open System.Security.Claims
    open Saturn
    open FSharp.Control.Tasks
    open Utils
    open Track
    open System.Net
    open Giraffe.GiraffeViewEngine
    open System.Threading.Tasks

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

    let getForm<'a> (ctx : HttpContext) =
        task {
            let! a = Controller.getForm<'a> ctx
            match Client.validate a with
            | Error xs ->
                ctx.Response.StatusCode <- int HttpStatusCode.BadRequest
                let messages =
                    xs
                    |> List.map (fun (msg, x) ->
                        let withValue =
                            match x with
                            | Some x ->
                                let object = (str <| string x).ToString()
                                sprintf " but has value '%s'." object
                            | None -> ""
                        msg + withValue )
                return Error <| Errors.ValidationError messages
            | Ok a -> return Ok a
        }

    // Return  HttpFunc here instead of the xmlnode list to make it more generic
    let toHttpResult (ctx : HttpContext) f (result : Task<Result<'a, Errors>>) =
        task {
        let! result = result
        match result with
        | Error (Errors.ValidationError xs) ->
            ctx.Response.StatusCode <- int HttpStatusCode.BadRequest
            return xs |> List.map (fun x -> p [] [ rawText x ] )
        | Error (Errors.DBError xs) ->
            ctx.Response.StatusCode <- int HttpStatusCode.InternalServerError
            return xs |> List.map (fun x -> p [] [ rawText x ])
        | Ok x -> return f x
        }
            
