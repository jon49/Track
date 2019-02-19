namespace Track.Repository

open FSharp.Data
open Track.Settings

[<AutoOpen>]
module Shared =

    [<Literal>]
    let CONNECTION_STRING = @"Data Source=.\SQLEXPRESS;Initial Catalog=JNyman;Integrated Security=True"

module StoredProcedures =
    
    type Track = SqlProgrammabilityProvider<Shared.CONNECTION_STRING>

module User =

    open Track
    open Track.AspNet
    open Track.AspNet.AspNet

    let cacheKey (authId : AuthId) = sprintf "user-info-%s" (authId.ToString())

    type Role = Coordinator | Coach

    let getRole = function
        | "Coordinator" -> Some Coordinator
        | "Coach" -> Some Coach
        | _ -> None

    type T = {
        UserId : int
        Auth0Id : string
        Email : string option
        FirstName : string
        LastName : string
        TeamIds : Set<int>
        RegionIds : Set<int>
        Roles : Set<Role>
    }

    let addUser ctx =
        async {
            let (firstName, lastName) = AspNet.getUserNames ctx
            use cmd = new StoredProcedures.Track.track.AddUser(Setting.Database.Connection)
            return! cmd.AsyncExecuteSingle((AspNet.getAuth0Id ctx).ToString(), firstName, lastName, null)
        }
        |> Async.StartAsTask

    let getUserPermissions (authId : AuthId) =
        Cache.tryOrGetAsync (fun x ->
            async {
                use cmd = new StoredProcedures.Track.track.GetUserInfo(Setting.Database.Connection)
                let! userInfo = cmd.AsyncExecute(authId.ToString())
                let userInfo = userInfo |> Seq.toList
                let user =
                    match Seq.tryHead userInfo with
                    | None -> None
                    | Some user ->
                        let teams = userInfo |> Seq.choose (fun x -> x.TeamId) |> Set
                        let regions = userInfo |> Seq.choose (fun x -> x.RegionId) |> Set
                        let roles =
                            userInfo
                            |> Seq.choose (fun x ->
                                x.RoleName
                                |> (Option.map getRole >> Option.flatten) )
                            |> Set
                        Some <|
                        {
                            UserId = user.UserId
                            Auth0Id = authId.ToString()
                            Email = user.Email
                            FirstName = user.FirstName
                            LastName = user.LastName
                            TeamIds = teams
                            RegionIds = regions
                            Roles = roles
                        }
                return user
            }
        ) (authId.ToString())

