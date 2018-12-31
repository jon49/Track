namespace Teams

module Model =
    open System.ComponentModel

    [<CLIMutable>]
    type Base = {
        [<DisplayName("Team")>]
        TeamName : string
        [<DisplayName("First Name")>]
        FirstName : string
        [<DisplayName("Last Name")>]
        LastName : string
        [<DisplayName("Email")>]
        Email : string
    }

    type Team = {
        Data : Base
        TeamId : int
        UserId : int
    }
