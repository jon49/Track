namespace TrackTests

module ViewModel =

    open System.ComponentModel

    [<Literal>]
    let FIRST_NAME = "First Name"
    [<Literal>]
    let LAST_NAME = ""

    type Person = {
        [<DisplayName(FIRST_NAME)>]
        FirstName : string
        [<DisplayName(LAST_NAME)>]
        LastName : string
        [<DisplayName(null)>]
        MiddleName : string
        Age : int
    }

    let person = {
        FirstName = "Jon"
        MiddleName = "Yep"
        LastName = "Okay"
        Age = 16
    }

