namespace Utils

module Class = 
    open Giraffe.GiraffeViewEngine

    [<Literal>]
    let revealIfActive = "reveal-if-active "
    //[<Literal>]
    //let backgroundNone = "bg-none "
    [<Literal>]
    let moveRight = "move-right "
    [<Literal>]
    let modal = "modal "
    [<Literal>]
    let modalTarget = "modal-target "

    module Icon =
        [<Literal>]
        let checkCircle = "icon-check-circle "
        [<Literal>]
        let slash = "icon-slash "
        [<Literal>]
        let edit = "icon-edit "

    /// Picnic CSS
    module P =
        [<Literal>]
        let flex = "flex "
        [<Literal>]
        let brand = "brand "
        [<Literal>]
        let show = "show "
        [<Literal>]
        let burger = "burger "
        [<Literal>]
        let toggle = "toggle "
        [<Literal>]
        let pseudo = "pseudo "
        [<Literal>]
        let button = "button "
        [<Literal>]
        let menu = "menu "
        [<Literal>]
        let checkable = "checkable "

        module Flex =

            /// 100px increments starting at 500 up to 2000px
            [<Struct>]
            type Width =
            | S800
            with
                override __.ToString() =
                    match __ with
                    | S800 -> "800"

            [<Literal>]
            let one = "one "
            [<Literal>]
            let two = "two "
            let two_ (width : Width) = sprintf "two-%s " (width.ToString())

module UI =
    open Reflection
    open Giraffe.GiraffeViewEngine
    open Utils.ViewEngine
    open Microsoft.FSharp.Quotations
    open System.ComponentModel.DataAnnotations
    open Giraffe
    module P = Class.P

    [<Struct>]
    type InputSettings = {
        Attrs : seq<XmlAttribute>
        Label : string -> string option
    } with
        static member Init = {
            Attrs = []
            Label = Option.Some
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

    let customInput<'a, 'b when 'a :> obj and 'b :> obj> ``type`` settings (a : 'a) (expr : Expr<'a -> 'b>) =
        let pi = getPropertyInfo expr
        let value = getValue pi a
        let required = not <| Reflection.isOption value
        let displayName = getDisplayName pi
        let validationAttributes = Reflection.getCustomAttributes<ValidationAttribute> pi
        let friendlyMessage = Client.getFriendlyMessages validationAttributes
        let validationAttributes = Client.getHtmlValidationAttributes validationAttributes

        let label =
            match settings.Label displayName with
            | Some displayName -> label [ _for pi.Name; _title friendlyMessage; _style "order:-1;" ] [ rawText displayName ]
            | None -> GiraffeViewEngine.emptyText

        div [ _class (P.flex) ] [
            input [
                yield _type ``type``
                yield _value <| string value
                yield _name pi.Name
                yield _id pi.Name
                yield! (if required then [ _required; _placeholder "Required" ] else [ _empty ])
                yield! settings.Attrs
                yield! validationAttributes ]
            label ]

    let inputText expr a = customInput "text" expr a

    let inputEmail  expr a = customInput "email" expr a

    let editButton =
        let editClass = P.pseudo + P.button + Class.Icon.edit
        fun editUrl targetId ->
        let target =
            targetId
            |> Option.map(fun x -> _icTarget <| "#" + x)
            |> Option.defaultValue _empty
                
        span [] [
            a [
                _icGetFrom editUrl
                target
                _icIndicator "#edit-spinner"
                _class editClass ] []
            span [ _id "edit-spinner"; _alt "Loading\u2026"; _class "icon-loading"; ] []
        ]

