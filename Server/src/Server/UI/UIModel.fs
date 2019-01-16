namespace Track

module UI =
    open Utils
    open Microsoft.FSharp.Quotations
    open Reflection
    open Giraffe.GiraffeViewEngine

    type Model = {
        DisplayName : string
        Validation : Validation[]
    }

    let private getModel (properties : Map<string, Model>) propertyName = 
        match properties.TryFind propertyName with
        | Some model -> model.DisplayName, model.Validation
        | None -> "", [||]

    let field ``type`` (properties : Map<string, Model>) (property : Expr<'a>) attrs =
        let propertyName =
            getPropertyName property
            |> Option.defaultValue ""
        let value =
            getPropertyValue property
            |> Option.defaultValue ""
        let displayName, validation = getModel properties propertyName
        let id = propertyName

        [
        label [ _for id ] [ rawText displayName ]
        input [
            yield _type ``type``
            yield _value value
            yield _name propertyName
            yield _id id
            yield! attrs
            yield! (validation |> Array.map Validation.createAttribute) ]
        ]

    let inputText (properties : Map<string, Model>) (property : Expr<'a>) attrs =
        field "text" properties property attrs

    let inputEmail (properties : Map<string, Model>) (property : Expr<'a>) attrs =
        field "email" properties property attrs

