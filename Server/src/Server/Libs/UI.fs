﻿namespace Utils

module UI =
    open Reflection
    open Giraffe.GiraffeViewEngine
    open Utils.ViewEngine
    open Microsoft.FSharp.Quotations
    open System.ComponentModel.DataAnnotations
    open Giraffe

    [<Struct>]
    type InputSettings = {
        Attrs : seq<XmlAttribute>
        AddLabel : bool
    } with
        static member Init = {
            Attrs = []
            AddLabel = true
        }

    let radio name (a : 'a) (expr : Expr<'a -> int>) attrs =
        let pi = getPropertyInfo expr
        let value = getValue pi a |> Option.ofObj |> Option.map (fun x -> x :?> int) |> Option.defaultValue 0 |> string
        let displayName = getDisplayName pi
        [
        input [
            yield _type "radio"
            yield _name name
            yield _id pi.Name
            yield _value value
            yield! attrs ]
        label [ _for pi.Name ] [ rawText displayName ]
        ]

    let input<'a, 'b when 'a :> obj and 'b :> obj> ``type`` settings (a : 'a) (expr : Expr<'a -> 'b>) =
        let pi = getPropertyInfo expr
        let value = getValue pi a
        let required = not <| Reflection.isOption value
        let displayName = getDisplayName pi
        let validationAttributes = Reflection.getCustomAttributes<ValidationAttribute> pi
        let friendlyMessage = Client.getFriendlyMessage validationAttributes
        let validationAttributes = Client.getHtmlValidationAttributes validationAttributes

        let label =
            if settings.AddLabel then
                label [ _for pi.Name; _title friendlyMessage ] [ rawText displayName ]
            else GiraffeViewEngine.emptyText

        [
        label
        input [
            yield _type ``type``
            yield _value <| string value
            yield _name pi.Name
            yield _id pi.Name
            yield! (if required then [ _required; _placeholder "Required" ] else [ _empty ])
            yield! settings.Attrs
            yield! validationAttributes ]
        ]

    let inputText expr attrs = input "text" expr attrs

    let inputEmail  expr attrs = input "email" expr attrs

module Class = 

    [<Literal>]
    let revealIfActive = "reveal-if-active"
