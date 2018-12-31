namespace Track

module ViewEngine =

    open Giraffe.GiraffeViewEngine
    open Microsoft.FSharp.Quotations
    open Reflection

    let str = encodedText

    let _icActionTarget = attr "ic-action-target"
    let _icAction = attr "ic-action"
    let _icTarget = attr "ic-target"
    let _icPostTo = attr "ic-post-to"
    let _icGetFrom = attr "ic-get-from"
    let _icAppendFrom = attr "ic-append-from"
    let _icSrc = attr "ic-src"

    let field ``type`` attrs (field : Expr<'a>) =
        let propertyName =
            getPropertyName field
            |> Option.defaultValue ""
        let value =
            getPropertyValue field
            |> Option.defaultValue ""
        let displayName = getDisplayName field
        let id = propertyName

        [ label [ _for id ] [ rawText displayName ]
          input ([ _type ``type``; _value value; _name propertyName; _id id ] @ attrs) ]

