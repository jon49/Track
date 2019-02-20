namespace Track

module User =

    type Role = Coordinator | Coach

    let getRole = function
        | "Coordinator" -> Some Coordinator
        | "Coach" -> Some Coach
        | _ -> None

    type Type =
    | Registered
    | Authenticated
    | UnknownUser

    type T = {
        Auth0Id : string
        Email : string option
        FirstName : string
        LastName : string
        PreferredRegionId : int option
        PreferredTeamId : int option
        RegionIds : Set<int>
        Roles : Set<Role>
        TeamIds : Set<int>
        UserId : int
        Type : Type
    }
