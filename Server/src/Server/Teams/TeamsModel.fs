namespace Teams

module Model =

    type Base = {
        TeamName : string
        FirstName : string
        LastName : string
        Email : string
    }

    type Team = {
        Data : Base
        TeamId : int
        UserId : int
    }
