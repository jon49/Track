namespace Utils

module Client =
    open Giraffe
    open Giraffe.GiraffeViewEngine
    open System.ComponentModel.DataAnnotations
    open Saturn

    type IModelValidation<'a> =
        abstract Validate : unit -> Result<'a, string>

    let validate (x : IModelValidation<'a>) =
        x.Validate ()

    type IModelClean<'a> =
        abstract Clean : unit -> 'a

    let clean (x : IModelClean<'a>) =
        x.Clean ()

    type Validation<'a> = {
        Attribute : XmlAttribute option
        FriendlyMessage : string option
        Validate : string -> 'a -> string option
    }

    let max maximum = {
        Attribute = Some (maximum |> string |> _max)
        FriendlyMessage = Some <| sprintf "At most %i" maximum
        Validate = fun name x ->
            if x <= maximum
                then None
            else Some <| sprintf "%s: '%i' must be less than '%i'." name x maximum
    }

    let min minimum = {
        Attribute = Some (minimum |> string |> _min)
        FriendlyMessage = Some <| sprintf "At least %i" minimum
        Validate = fun name x ->
            if x >= minimum
                then None
            else Some <| sprintf "%s: '%i' must be greater than '%i'" name x minimum
    }

    let minLength minimum = {
        Attribute = Some (_minlength <| string minimum)
        FriendlyMessage = Some <| sprintf "At least %i characters" minimum
        Validate = fun name (x : string) ->
            if x.Length >= minimum
                then None
            else Some <| sprintf "%s: '%s' must have a length greater than %i." name x minimum
    }

    let maxLength maximum = {
        Attribute = Some (_maxlength <| string maximum)
        FriendlyMessage = Some <| sprintf "At most %i characters" maximum
        Validate = fun name (x : string) ->
            if x.Length <= maximum
                then None
            else Some <| sprintf "%s: '%s' must have a length less than %i." name x maximum
    }

    let email = {
        Attribute = None
        FriendlyMessage = None
        Validate = fun name x ->
            let value = String.trim x
            if (new EmailAddressAttribute()).IsValid(value)
                then None
            else Some <| sprintf "%s: '%s' is not a valid email." name value
    }

    let identity = {
        Attribute = None
        FriendlyMessage = None
        Validate = fun name x ->
            if x > 0
                then None
            else Some <| sprintf "%s: '%i' must be greater than 0." name x
    }

    let friendlyValidationMessage validations =
        validations
        |> Array.choose (fun x -> x.FriendlyMessage)
        |> String.concat " &ndash; "

module ClientDefinition =
    open Microsoft.FSharp.Quotations
    open Client

    type Client<'a, 'b> = {
        DisplayName : string
        Get : 'a -> 'b
        Name : Expr<'a -> 'b>
        Validations : Validation<'b>[]
    }

    let prove a info =
        let value = info.Get a
        info.Validations
        |> Array.tryPick (fun f -> f.Validate info.DisplayName value)

    let proof provedItems =
        provedItems
        |> List.choose id
        |> String.concat "\n"
        |> fun x -> if x.Length = 0 then None else Some x

module UI =
    open Utils
    open Client
    open Reflection
    open Giraffe.GiraffeViewEngine
    open Utils.ViewEngine
    open ClientDefinition

    let getModel client =
        client.DisplayName, client.Validations

    let input ``type`` (client : Client<'a, 'b>) a attrs =
        let propertyName = propertyName client.Name
        let value = client.Get a
        let required = not <| Reflection.isOption value
        let displayName, validations =
            getModel client
            
        let id = propertyName

        div [] [
            label [ _for id; _title (friendlyValidationMessage validations) ] [ rawText displayName ]
            input [
                yield _type ``type``
                yield _value <| string value
                yield _name propertyName
                yield _id id
                yield! (if required then [ _required; _placeholder "Required" ] else [ _empty ])
                yield! attrs
                yield! (validations |> Array.choose (fun x -> x.Attribute)) ]
            ]

    let inputText (client : Client<'a, string>) a attrs =
        input "text" client a attrs

    let inputEmail (client : Client<'a, string>) a attrs =
        input "email" client a attrs

