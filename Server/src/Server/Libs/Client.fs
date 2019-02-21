namespace Utils

module Client =
    open System.ComponentModel.DataAnnotations
    open Giraffe.GiraffeViewEngine

    let validate a =
        let mutable _list = new System.Collections.Generic.List<ValidationResult>()
        match Validator.TryValidateObject(a, new ValidationContext(a), _list) with
        | true -> Ok a
        | false -> Error <| System.String.Join ("\n", _list |> Seq.map (fun x -> x.ErrorMessage))

    let getDisplayName = Reflection.getDisplayName

    let getFriendlyMessage (validationAttributes : ValidationAttribute[]) =
        validationAttributes
        |> fun xs ->
            xs
            |> Seq.choose (fun x ->
                match x with
                | :? StringLengthAttribute as x -> Some <| sprintf "Must be at least %i or at most %i characters" x.MinimumLength x.MaximumLength
                | :? EmailAddressAttribute -> Some "Must be an email address"
                | :? MinLengthAttribute as x -> Some <| sprintf "Must be at least %i characters" x.Length
                | :? MaxLengthAttribute as x -> Some <| sprintf "Must be at most %i characters" x.Length
                | :? RangeAttribute as x -> Some <| sprintf "Must be between %O and %O" x.Minimum x.Maximum
                | _ -> None
            )
            |> String.concat " &ndash; "

    let getHtmlValidationAttributes (validationAttributes : ValidationAttribute[]) =
        validationAttributes
        |> fun xs ->
            xs
            |> Array.map (fun x ->
                match x with
                | :? StringLengthAttribute as x -> [ _minlength (string x.MinimumLength); _maxlength (string x.MaximumLength) ]
                | :? MinLengthAttribute as x -> [ _minlength (string x.Length) ]
                | :? MaxLengthAttribute as x -> [ _maxlength (string x.Length) ]
                | :? RangeAttribute as x -> [ _min (string x.Minimum); _max (string x.Maximum) ]
                | _ -> []
            )
            |> List.concat
