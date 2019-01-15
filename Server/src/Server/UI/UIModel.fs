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

    let field ``type`` attrs (properties : Map<string, Model>) (field : Expr<'a>) =
        let propertyName =
            getPropertyName field
            |> Option.defaultValue ""
        let value =
            getPropertyValue field
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
