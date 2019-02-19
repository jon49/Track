module Index

open Giraffe.GiraffeViewEngine
open Microsoft.AspNetCore.Http
open Saturn
open FSharp.Control.Tasks
open Track.Pages

let indexLayout (ctx : HttpContext) =
    task {
        let isAuthenticated = ctx.User.Identity.IsAuthenticated
        let options = {
            App.IsAuthenticated = isAuthenticated
        }
        let body =
            if isAuthenticated then
                Welcome.View
            else []
        return App.layout options body
    }

let indexRouter = controller {
    index indexLayout
}
