namespace Utils

module ViewEngine =

    open Giraffe.GiraffeViewEngine
    open Microsoft.FSharp.Quotations
    open Reflection

    let str = encodedText

    // Intercooler attributes
    let _icActionTarget = attr "ic-action-target"
    let _icAction = attr "ic-action"
    let _icTarget = attr "ic-target"
    let _icPostTo = attr "ic-post-to"
    let _icGetFrom = attr "ic-get-from"
    let _icAppendFrom = attr "ic-append-from"
    let _icSrc = attr "ic-src"
    let _icDeps = attr "ic-deps"

    // Custom
    let _empty = flag ""

    module Header =

        let ``IC-Trigger`` url = "X-IC-Trigger", url

