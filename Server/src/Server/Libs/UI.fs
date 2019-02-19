namespace Utils

module UI =
    open Reflection
    open Giraffe.GiraffeViewEngine
    open Utils.ViewEngine
    open Microsoft.FSharp.Quotations
    open System.ComponentModel.DataAnnotations

    let radio<'a when 'a :> obj> name (expr : Expr<'a>) attrs =
        let propertyName = propertyName expr
        let value = getPropertyValue expr |> Option.defaultValue 0 |> int |> string
        let displayName = getDisplayName expr
        [
        input [
            yield _type "radio"
            yield _name name
            yield _id propertyName
            yield _value value
            yield! attrs ]
        label [ _for propertyName ] [ rawText displayName ]
        ]

    let input<'a when 'a :> obj> ``type`` (expr : Expr<'a>) attrs =
        let propertyName = propertyName expr
        let value = getPropertyValue expr
        let required = not <| Reflection.isOption value
        let displayName = getDisplayName expr
        let validationAttributes = Reflection.getCustomAttributes<ValidationAttribute, 'a> expr
        let friendlyMessage = Client.getFriendlyMessage validationAttributes
        let validationAttributes = Client.getHtmlValidationAttributes validationAttributes
            
        let id = propertyName

        [
        label [ _for id; _title (Option.defaultValue "" friendlyMessage) ] [ rawText displayName ]
        input [
            yield _type ``type``
            yield _value <| string value
            yield _name propertyName
            yield _id id
            yield! (if required then [ _required; _placeholder "Required" ] else [ _empty ])
            yield! attrs
            yield! Option.defaultValue [] validationAttributes ]
        ]

    let inputText expr attrs = input "text" expr attrs

    let inputEmail  expr attrs = input "email" expr attrs

module Class = 

    [<Literal>]
    let revealIfActive = "reveal-if-active"

