namespace Teams

module Model =
    open System.ComponentModel.DataAnnotations

    [<CLIMutable>]
    type Team = {
        [<Display(Name = "Team"); Required; MinLength(1); MaxLength(128)>]
        Name : string
    }

    [<CLIMutable>]
    type TeamEdit = {
        Team : Team
        [<Range(1, System.Int32.MaxValue)>]
        Id : int
    }

    [<CLIMutable>]
    type Coach = {
        [<Display(Name = "First Name"); Required; StringLength(128, MinimumLength = 1)>]
        FirstName : string
        [<Display(Name = "Last Name"); Required; StringLength(128, MinimumLength = 1)>]
        LastName : string
        [<Display(Name = "Email"); Required; EmailAddress; StringLength(256, MinimumLength = 1)>]
        Email : string
    }

    [<CLIMutable>]
    type CoachEditNew = {
        Coach : Coach
        /// When new refers to the team ID.
        /// When editing it refers to the coach ID.
        [<Range(1, System.Int32.MaxValue)>]
        Id : int
    }
