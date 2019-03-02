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
    open Utils
    open Microsoft.Extensions.Primitives

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
        match Reflection.SomeObj a with
        | true, None -> return Ok a
        | true, Some _ | false, Some _ ->
            match Client.validate a with
            | Error xs ->
                ctx.Response.StatusCode <- int HttpStatusCode.BadRequest
                let messages =
                    xs
                    |> List.map (fun (msg, x) ->
                        let withValue =
                            match x with
                            | Some x ->
                                let object =
                                    x |> string |> str |> renderHtmlNode
                                sprintf " but has value '%s'." object
                            | None -> ""
                        Errors.ValidationError (msg + withValue) )
                return Error messages
            | Ok a -> return Ok a
        | false, None -> return Error [ Errors.ValidationError "Object is required." ]
        }

    let toHtmlErrorResult (ctx : HttpContext) (errors : Errors list) =
            errors
            |> List.map (
                function
                | Errors.ValidationError x ->
                    ctx.Response.StatusCode <- int HttpStatusCode.BadRequest
                    p [ _class "error" ] [ rawText x ]
                | Errors.DBError x ->
                    ctx.Response.StatusCode <- int HttpStatusCode.InternalServerError
                    p [ _class "error" ] [ rawText x ]
                | Errors.AuthorizationError ->
                    ctx.Response.StatusCode <- int HttpStatusCode.Forbidden
                    p [ _class "error" ] [ rawText "You are not authorized to view this." ]
                | Errors.NotAuthenticated ->
                    ctx.Response.StatusCode <- int HttpStatusCode.Unauthorized
                    p [ _class "error" ] [ rawText "User must be authenticated to access this." ]
                | Errors.NotFound ->
                    ctx.Response.StatusCode <- int HttpStatusCode.NotFound
                    p [ _class "error" ] [ rawText "Item not found." ]
                | Errors.InternalServerError ->
                    ctx.Response.StatusCode <- int HttpStatusCode.InternalServerError
                    p [ _class "error" ] [ rawText "Something happened which shouldn't have." ]
            )
            |> div []
            |> renderHtmlNode

    let html (ctx : HttpContext) (result : TaskResult<XmlNode, Errors list>) =
        task {
        match! result with
        | Ok x -> return! Controller.renderHtml ctx x
        | Error errors ->
            return! Controller.html ctx (toHtmlErrorResult ctx errors)
        }

    let partial (ctx : HttpContext) (result : TaskResult<XmlNode, Errors list>) =
        task {
        match! result with
        | Ok x ->
            return! Controller.html ctx <| renderHtmlNode x
        | Error errors ->
            return! Controller.html ctx (toHtmlErrorResult ctx errors)
        }

    let created f (ctx : HttpContext) (location : 'a -> string) (result : TaskResult<_, _>) =
        task {
        match! result with
        | Ok x when f x ->
            ctx.Response.StatusCode <- int HttpStatusCode.Created
            ctx.Response.Headers.Add("Location", new StringValues(location x))
            return! Controller.response ctx ""
        | Ok _ -> return! Controller.html ctx (toHtmlErrorResult ctx [ Errors.InternalServerError ])
        | Error errors ->
            return! Controller.html ctx (toHtmlErrorResult ctx errors)
        }

    let createdId ctx location success result =
        success ctx
        created (fun x -> x > 0) ctx location result

    let noContent (ctx : HttpContext) (result : TaskResult<unit, Errors list>) =
        task {
        match! result with
        | Ok _ ->
            ctx.Response.StatusCode <- int HttpStatusCode.NoContent
            return Some ctx
        | Error errors ->
            return! Controller.html ctx (toHtmlErrorResult ctx errors)
        }
            
