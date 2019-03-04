﻿namespace Utils

module ViewEngine =

    open Giraffe.GiraffeViewEngine

    let str = encodedText

    // Intercooler attributes
    let _icActionTarget = attr "ic-action-target"
    let _icAction = attr "ic-action"
    let _icSuccessAction = attr "ic-success-action"
    let _icTarget = attr "ic-target"
    let _icPostTo = attr "ic-post-to"
    let _icGetFrom = attr "ic-get-from"
    let _icAppendFrom = attr "ic-append-from"
    let _icSrc = attr "ic-src"
    let _icDeps = attr "ic-deps"
    let _icIndicator = attr "ic-indicator"

    // Custom
    let _empty = flag ""

    module Header =
        open Microsoft.AspNetCore.Http
        open Microsoft.Extensions.Primitives

        let ``IC-Trigger`` url = "X-IC-Trigger", url

        /// reset the target form, like `#my-form`
        let resetForm (ctx : HttpContext) (target : string) =
            ctx.Response.Headers.Add("X-IC-Trigger", new StringValues(sprintf """{"resetForm":"%s"}""" target))

