namespace Utils

module Client =
    open System.ComponentModel.DataAnnotations
    open Giraffe.GiraffeViewEngine
    open System.Configuration

    let getDisplayName = Reflection.getDisplayName

    let getFriendlyMessage (validationAttribute : ValidationAttribute) =
        match validationAttribute with
        | :? StringLengthAttribute as x -> Some <| sprintf "must be at least %i or at most %i characters" x.MinimumLength x.MaximumLength
        | :? EmailAddressAttribute -> Some "must be an email address"
        | :? MinLengthAttribute as x -> Some <| sprintf "must be at least %i characters" x.Length
        | :? MaxLengthAttribute as x -> Some <| sprintf "must be at most %i characters" x.Length
        | :? RangeAttribute as x -> Some <| sprintf "must be between %O and %O" x.Minimum x.Maximum
        | _ -> None

    let getFriendlyMessages validationAttributes =
        validationAttributes
        |> Array.choose getFriendlyMessage
        |> String.concat " &ndash; "

    let getFriendlyMessageWithValue (validationAttribute : ValidationAttribute, friendlyName, a) =
        match validationAttribute with
        | :? RequiredAttribute -> sprintf "'%s' value is required." friendlyName
        | _ ->
            match getFriendlyMessage validationAttribute with
            | Some message -> sprintf "'%s' %s but given '%O'" friendlyName message a
            | None -> ""
            

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

    let validate2 a =
        let properties = a.GetType().GetProperties()
        properties
        |> Array.toList
        |> List.choose (fun propertyInfo ->
            let validationAttributes = Reflection.getCustomAttributes<ValidationAttribute> propertyInfo
            let value = Reflection.getValue propertyInfo
            match Reflection.SomeObj value with
            | true, Some value
            | false, Some value ->
                validationAttributes
                |> Array.tryFind ( fun x -> x.IsValid(value) )
                |> Option.map (fun x -> x, Reflection.getDisplayName propertyInfo, Some value )
            | false, None -> Some (RequiredAttribute() :> ValidationAttribute, Reflection.getDisplayName propertyInfo, None)
            | true, None -> None
        )
        |> List.map getFriendlyMessageWithValue
        |> function
           | [] -> Ok a
           | xs -> Error xs
