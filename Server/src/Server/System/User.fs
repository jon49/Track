namespace Track

module User =
    open FSharp.Data.Runtime.WorldBank

    type Role = Coordinator | Coach

    let getRole = function
        | "Coordinator" -> Some Coordinator
        | "Coach" -> Some Coach
        | _ -> None

    type RegionID = ID of int
    type TeamID = ID of int
    type UserID = ID of int

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
        UserId : UserID
        Type : Type
    } with
        member __.getTeamId id =
            if __.TeamIds.Contains id then
                Ok <| TeamID.ID id
            else Error [ Errors.AuthorizationError ]

        member __.getRegionId id =
            if __.RegionIds.Contains id then
                Ok <| RegionID.ID id
            else Error [ Errors.AuthorizationError ]
