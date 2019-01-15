namespace Track

[<AutoOpen>]
module UI =
    open Utils

    type Model = {
        DisplayName : string
        Validation : Validation[]
    }
