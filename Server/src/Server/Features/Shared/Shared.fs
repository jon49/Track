namespace Track.Repository

open FSharp.Data
open Track.Settings

[<AutoOpen>]
module Shared =

    [<Literal>]
    let CONNECTION_STRING = @"Data Source=.\SQLEXPRESS;Initial Catalog=JNyman;Integrated Security=True"

[<AutoOpen>]
module StoredProcedures =
    
    open Track

    type DB = SqlProgrammabilityProvider<Shared.CONNECTION_STRING>

    let conn = Setting.Database.Connection

    let repoTry f asyncValue =
        async {
        try
            let! result = asyncValue ()
            return Ok <| f result
        with
        | ex ->
            use cmd = new DB.track_api.AddLogEntry(conn)
            let! result = cmd.AsyncExecute(ex.Message, ex.ToString())
            return Error [ Errors.DBError "An error occurred and is being looked into." ]
        }
        |> Async.StartAsTask

    let tryList f = repoTry (fun xs -> xs |> Seq.toList) f

    let tryId f = repoTry id f

module User =

    open Track
    open Track.AspNet
    open Track.AspNet.AspNet
    open Track.User

    let cacheKey (authId : AuthId) = sprintf "user-info-%s" (authId.ToString())

    let addUser ctx =
        tryId (fun x ->
            let (firstName, lastName) = AspNet.getUserNames ctx
            use cmd = new DB.track_api.AddUser(Setting.Database.Connection)
            cmd.AsyncExecuteSingle((AspNet.getAuth0Id ctx).ToString(), firstName, lastName, null) )

    let getUserPermissions (authId : AuthId) =
        Cache.tryOrGetAsync (fun x ->
            async {
                use cmd = new DB.track_api.GetUserInfo(Setting.Database.Connection)
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
                            Auth0Id = authId.ToString()
                            Email = user.Email
                            FirstName = user.FirstName
                            LastName = user.LastName
                            PreferredRegionId = user.PreferredRegionId
                            PreferredTeamId = user.PreferredTeamId
                            RegionIds = regions
                            Roles = roles
                            TeamIds = teams
                            UserId = UserID.ID user.UserId
                            Type =
                                match regions.IsEmpty && teams.IsEmpty with
                                | true -> Authenticated
                                | false -> Registered
                        }
                return user
            }
        ) (authId.ToString())

