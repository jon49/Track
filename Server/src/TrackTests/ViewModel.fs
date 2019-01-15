namespace TrackTests

module ViewModel =

    open Track.UI
    open Utils
    open Utils.Reflection

    [<Literal>]
    let FIRST_NAME = "First Name"
    [<Literal>]
    let LAST_NAME = ""

    type Person = {
        FirstName : string
        LastName : string
        MiddleName : string
        Age : int
    }

    let person = {
        FirstName = "Jon"
        MiddleName = "Yep"
        LastName = "Okay"
        Age = 16
    }

    let UI =
        Map.empty<string, Model>
        |> Map.add
            (propertyName <@ fun x -> x.FirstName @>)
            { DisplayName = FIRST_NAME; Validation = [| Required; MaxLength 128 |] }
        |> Map.add
            (propertyName <@ fun x -> x.LastName @>)
            { DisplayName = LAST_NAME; Validation = [| Required; MaxLength 128 |] }
        |> Map.add
            (propertyName <@ fun x -> x.MiddleName @>)
            { DisplayName = ""; Validation = [| MinLength 1 |] }

