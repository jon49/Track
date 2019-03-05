namespace Track

open FSharp.Data
open Track.Settings

module Repository =

    [<AutoOpen>]
    module Shared =
        open Track

        [<Literal>]
        let CONNECTION_STRING = @"Data Source=.\SQLEXPRESS;Initial Catalog=JNyman;Integrated Security=True"

        type DB = SqlProgrammabilityProvider<CONNECTION_STRING>
        let conn = Setting.Database.Connection
        
        let noContent result =
            async {
            let! result = result
            return
                match result with
                | Some x when x > 0 -> Ok ()
                | _ -> Error [ Errors.InternalServerError ]
            }
            |> Async.StartAsTask

        let single result =
            async {
            let! result = result
            return
                match result with
                | Some x -> Ok x
                | None -> Error [ Errors.NotFound ]
            }
            |> Async.StartAsTask

        let list result =
            async {
            let! result = result
            return Ok result
            }
            |> Async.StartAsTask

    module User =

        open Track
        open Track.AspNet
        open Track.AspNet.AspNet
        open Track.User

        let cacheKey (authId : AuthId) = sprintf "user-info-%s" (authId.ToString())

        let addUser ctx =
            async {
                let (firstName, lastName) = AspNet.getUserNames ctx
                use cmd = new DB.track_api.CreateUser(Setting.Database.Connection)
                return! cmd.AsyncExecuteSingle((AspNet.getAuth0Id ctx).ToString(), firstName, lastName, null)
            }
            |> single

        let getUserPermissions (authId : AuthId) =
            Cache.tryOrGetAsync (fun x ->
                async {
                    use cmd = new DB.track_api.GetUserInfo(Setting.Database.Connection)
                    let! userInfo = cmd.AsyncExecute(authId.ToString())
                    let userInfo = userInfo |> Seq.toList
                    let user =
                        match Seq.tryHead userInfo with
                        | None -> Error [ Errors.AuthorizationError ]
                        | Some user ->
                            let teams = userInfo |> Seq.choose (fun x -> x.TeamId) |> Set
                            let regions = userInfo |> Seq.choose (fun x -> x.RegionId) |> Set
                            let roles =
                                userInfo
                                |> Seq.choose (fun x ->
                                    x.RoleName
                                    |> (Option.map getRole >> Option.flatten) )
                                |> Set
                            Ok <|
                            {
                                AuthId = AuthID.ID (authId.ToString())
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

        let reset ({ AuthId = AuthID.ID authId }) =
            Cache.remove <| authId.ToString()

