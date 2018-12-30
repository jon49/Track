namespace Track

module ViewEngine =

    open Giraffe.GiraffeViewEngine

    let str = encodedText

    let _icActionTarget = attr "ic-action-target"
    let _icAction = attr "ic-action"
    let _icTarget = attr "ic-target"
    let _icPostTo = attr "ic-post-to"
    let _icGetFrom = attr "ic-get-from"
    let _icAppendFrom = attr "ic-append-from"
    let _icSrc = attr "ic-src"
