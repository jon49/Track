namespace Utils

module Class = 
    open Giraffe.GiraffeViewEngine

    let join xs = xs |> String.concat " "

    [<Literal>]
    let revealIfActive = "reveal-if-active "

    [<Literal>]
    let backgroundNone = "bg-none "

    /// Mini CSS
    module M =
        [<Literal>]
        let container = "container "
        [<Literal>]
        let row = "row "
        [<Literal>]
        let logo = "logo "
        [<Literal>]
        let button = "button "
        [<Literal>]
        let card = "card "
        [<Literal>]
        let section = "section "

        module Color =
            [<Literal>]
            let primary = "primary "

            [<Literal>]
            let secondary = "secondary "
            

        module Animation =

            [<Literal>]
            let spinner = "spinner "

        module Icon =
            [<Literal>]
            let alert = "icon-alert "
            [<Literal>]
            let bookmark = "icon-bookmark "
            [<Literal>]
            let calendar = "icon-calendar "
            [<Literal>]
            let cart = "icon-cart "
            [<Literal>]
            let credit = "icon-credit "
            [<Literal>]
            let edit = "icon-edit "
            [<Literal>]
            let help = "icon-help "
            [<Literal>]
            let home = "icon-home "
            [<Literal>]
            let info = "icon-info "
            [<Literal>]
            let link = "icon-link "
            [<Literal>]
            let location = "icon-location "
            [<Literal>]
            let lock = "icon-lock "
            [<Literal>]
            let mail = "icon-mail "
            [<Literal>]
            let phone = "icon-phone "
            [<Literal>]
            let rss = "icon-rss "
            [<Literal>]
            let search = "icon-search "
            [<Literal>]
            let settings = "icon-settings "
            [<Literal>]
            let share = "icon-share "
            [<Literal>]
            let upload = "icon-upload "
            [<Literal>]
            let user = "icon-user "

        module Col =
            [<Literal>]
            let md = "col-md "
            let md_ = sprintf "col-md-%i "
            [<Literal>]
            let sm = "col-sm "
            let offset_sm = sprintf "col-sm-offset-%i "
            let sm_ = sprintf "col-sm-%i "

module UI =
    open Reflection
    open Giraffe.GiraffeViewEngine
    open Utils.ViewEngine
    open Microsoft.FSharp.Quotations
    open System.ComponentModel.DataAnnotations
    open Giraffe
    module M = Class.M

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

    let input<'a, 'b when 'a :> obj and 'b :> obj> ``type`` settings (a : 'a) (expr : Expr<'a -> 'b>) =
        let pi = getPropertyInfo expr
        let value = getValue pi a
        let required = not <| Reflection.isOption value
        let displayName = getDisplayName pi
        let validationAttributes = Reflection.getCustomAttributes<ValidationAttribute> pi
        let friendlyMessage = Client.getFriendlyMessages validationAttributes
        let validationAttributes = Client.getHtmlValidationAttributes validationAttributes

        let label =
            match settings.Label displayName with
            | Some displayName -> label [ _for pi.Name; _title friendlyMessage ] [ rawText displayName ]
            | None -> GiraffeViewEngine.emptyText

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

    let inputText expr a = input "text" expr a

    let inputEmail  expr a = input "email" expr a

    let editButton editUrl targetId =
        let target =
            targetId
            |> Option.map(fun x -> _icTarget <| "#" + x)
            |> Option.defaultValue _empty
                
        span [] [
            a [ _icGetFrom editUrl; target; _class (M.button + Class.backgroundNone); _icIndicator "#edit-spinner" ] [
                span [ _class M.Icon.edit ] [] ]
            span [ _id "edit-spinner"; _class M.Animation.spinner; _style "display:none;" ] []
        ]

