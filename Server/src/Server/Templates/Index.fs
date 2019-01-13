module Index

open Giraffe.GiraffeViewEngine
open Microsoft.AspNetCore.Http
open Saturn
open FSharp.Control.Tasks

let indexLayout (ctx : HttpContext) =
    task {
        let options = {
            App.IsAuthenticated = ctx.User.Identity.IsAuthenticated
        }
        return App.layout options []
    }

let indexRouter = controller {
    index indexLayout
}
