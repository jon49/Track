namespace Utils

[<AutoOpen>]
module Validation =

    open Giraffe.GiraffeViewEngine
    open System.ComponentModel.DataAnnotations
    open Utils.ViewEngine

    type Validation =
    | Min of minimum : int
    | Max of maximum : int
    | MinLength of length : int
    | MaxLength of length : int
    | Required
    | Email

    let createAttribute = function
        | Min minimum -> _min <| string minimum
        | Max maximum -> _max <| string maximum
        | MinLength length -> _minlength <| string length
        | MaxLength length -> _maxlength <| string length
        | Required -> _required
        | Email -> _empty

    let isValidString validation displayName value =
        let testString predicate (message : string -> string) s =
            s
            |> Option.ofObj
            |> Option.map (fun s ->
                let s = String.trim s
                if predicate s
                    then Ok s
                else Error (message s) )
            |> Option.defaultValue (Ok s)

        match validation with
        | Required ->
            match System.String.IsNullOrWhiteSpace(value) with
            | true -> Error <| sprintf "'%s' is required." displayName
            | false -> Ok <| String.trim value
        | MinLength minLength ->
            testString
                (fun x -> x.Length >= minLength)
                (fun x -> sprintf "'%s' must have a length greater than %i." x minLength)
                value
        | MaxLength maxLength ->
            testString
                (fun x -> x.Length <= maxLength)
                (fun x -> sprintf "'%s' must have a length less than %i." x maxLength)
                value
        | Min _ | Max _ | Email -> Ok value

    let isValidEmail validation value =
        match validation with
        | Required ->
            match Option.isSome <| Option.ofObj value with
            | true -> Ok <| String.trim value
            | false -> Error "Email is required."
        | Email ->
            value
            |> Option.ofObj
            |> Option.map (fun x ->
                let x = String.trim x
                match (new EmailAddressAttribute()).IsValid(x) with
                | true -> Ok x
                | false -> Error <| sprintf "'%s' is not a valid email." value )
            |> Option.defaultValue (Ok value)
        | Min _ | Max _ | MinLength _ | MaxLength _ -> Ok value

    let isValidInteger validation value =
        match validation with
        | Min minimum ->
            if value > minimum
                then Ok value
            else Error <| sprintf "'%i' must be greater than '%i'." value minimum
        | Max maximum ->
            if value < maximum
                then Ok value
            else Error <| sprintf "'%i' must be greater than '%i'" value maximum
        | MinLength _ | MaxLength _ | Required | Email -> Ok value
