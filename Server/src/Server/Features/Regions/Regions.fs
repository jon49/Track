namespace Track.Regions

module Repository =

    open Track.Repository
    open Track.User
    open Track.Settings
    open FSharp.Data

    let conn = Setting.Database.Connection

    let getAllTeams (RegionID.ID regionId) =
        async {
        use cmd = new SqlCommandProvider<"
            --DECLARE @RegionId int = 1;
            SELECT
                  t.[Name]
                , t.TeamId
                , rt.RegionId
                , u.Email
                , u.FirstName
                , u.LastName
                , u.UserId
                , u.PreferredTeamId
            FROM track.Team t
            JOIN track.RegionTeam rt
                ON rt.TeamId = t.TeamId
            JOIN track.UserTeam ut
                ON ut.TeamId = t.TeamId
                AND ut.Active = 1
            JOIN track.[User] u
                ON u.UserId = ut.UserId
            WHERE rt.RegionId = @RegionId
              AND rt.Active = 1
            ORDER BY t.[Name]
            ", CONNECTION_STRING>(conn)
        let! result = cmd.AsyncExecute(regionId)
        return Seq.toList result
        } |> list

    let getAll (UserID.ID userId) =
        async {
        use cmd = new DB.track_api.GetRegions(conn)
        let! result = cmd.AsyncExecute(userId)
        return Seq.toList result
        } |> list

    let update (RegionID.ID regionId) name =
        async {
        use cmd = new DB.track_api.EditRegion(conn)
        return! cmd.AsyncExecuteSingle(regionId, name)
        }
        |> single

    let get (RegionID.ID regionId) =
        async {
        use cmd = new DB.track_api.GetRegion(conn)
        return! cmd.AsyncExecuteSingle(regionId)
        }
        |> single

    let add (UserID.ID userId) regionName =
        async {
        use cmd = new DB.track_api.AddRegion(conn)
        return! cmd.AsyncExecuteSingle(userId, regionName)
        }
        |> single

    let getLastest (UserID.ID userId) =
        async {
        use cmd = new DB.track_api.GetRegionLatest(conn)
        return! cmd.AsyncExecuteSingle(userId)
        }
        |> single

module Model =
    open System.ComponentModel.DataAnnotations

    [<CLIMutable>]
    type Region = {
        [<MaxLength(256)>]
        Name : string
    } with
        static member Init = { Name = "" }

    [<Struct>]
    type RegionEdit = {
        Region : Region
        Id : int
    } with
        static member Create (id, name) =
            { Region = { Name = name }
              Id = id }

module Url =

    let index = "/regions"
    let show = sprintf "/regions/%i"
    let latest = "/regions/latest"
    let edit = sprintf "/regions/%i/edit"

module View =
    open Giraffe.GiraffeViewEngine
    open Utils.ViewEngine
    open Utils
    open Utils.UI
    open Model
    open Track.User
    open Track
    open Track.Teams.Model
    open Track.Coaches.Model

    let edit (x : RegionEdit) =
        form [ _method "post"; _icPostTo (Url.show x.Id) ] [
            yield! UI.inputText InputSettings.Init x.Region <@ fun x -> x.Name @>
            yield button [ _type "submit" ] [ rawText "Save" ] ]

    let regionRow (x : RegionEdit) =
        let regionShowId = sprintf "region-%i" x.Id
        header [ _id regionShowId ] [
            h2 [] [ str x.Region.Name ]
            UI.editButton (Url.edit x.Id) (Some <| regionShowId) ]

    let show (RegionID.ID regionId) (regions : RegionEdit list) (teams : (TeamEdit * (bool * CoachEdit) list) list) =
        let region = regions |> List.find (fun x -> x.Id = regionId)
        [
        regionRow region
        article [ _class Class.P.card ] [
            header [ _class <| Class.join [| Class.P.flex |] ] [
                h3 [] [ rawText "Teams" ]
                h3 [] [ rawText "Coaches" ] ]
            div [ _class Class.P.flex ] (
                teams
                |> List.map (fun (team, coaches) ->
                    div [ _class Class.P.card ] [
                        Teams.View.show team
                        div [] (coaches |> List.map (fun x -> x ||> Coaches.View.show))
                    ]
                ))
        ]
        ]


    let index =
        div []
            [
            h2 [] [ rawText "Regions" ]
            div [] [
                p [] [ rawText "Please enter the name of your new region." ]
                form [ _method "post"; _icPostTo Url.index; _id "new-region" ] [
                    yield! UI.inputText { InputSettings.Init with Attrs = [ _autofocus ]; Label = sprintf "New %s" >> Some } Region.Init <@ fun x -> x.Name @>
                    yield button [ _type "submit"; _icSuccessAction "[0]reset"; _icActionTarget "#new-region"  ] [ rawText "Submit" ] ]
                ]]

module Controller =
    open Saturn
    open Track
    open Track.AspNet
    open Utils
    open Utils.TaskResult
    open Utils.ViewEngine
    open Model
    open Track.Repository
    open FSharp.Control.Tasks
    open Track.Teams.Model
    open Track.Coaches.Model

    let indexAction : UserContext =
        fun user ctx ->
        match user.PreferredRegionId, user.Roles.Contains(User.Role.Coordinator) with
        | Some id, true -> Controller.redirect ctx (Url.show id)
        | _, false -> AspNet.html ctx <| (Task.lift <| Error [ Errors.AuthorizationError ])
        | None, true ->
            AspNet.html ctx <| (Task.lift <| Ok View.index)

    let showRegion regionId : UserContext =
        fun user ctx ->
        taskResult {
        let! regionId = Task.lift <| user.getRegionId regionId
        let! regions, teams =
            TaskResult.T.Parallel(
              fun () -> Repository.getAll user.UserId
            , fun () -> Repository.getAllTeams regionId)
        let teams =
            teams
            |> List.groupBy (fun x -> x.TeamId)
            |> List.map (fun (_, xs) ->
                let team = List.head xs
                let team = TeamEdit.Create(team.TeamId, team.Name)
                let coaches =
                    xs
                    |> List.map (fun x ->
                        let verifiedCoach =
                            match x.PreferredTeamId with
                            | Some p when p = team.Id -> true
                            | _ -> false
                        (verifiedCoach, CoachEdit.Create(x.UserId, x.FirstName, x.LastName, x.Email)) )
                team, coaches
            )
        let regions = regions |> List.map (fun x -> RegionEdit.Create(x.RegionId, x.Name))
        return App.layout (Some user) <| View.show regionId regions teams
        } |> AspNet.html ctx

    let updateRegion regionId : UserContext =
        fun user ctx ->
        taskResult {
        let! regionId = Task.lift <| user.getRegionId regionId
        let! region = AspNet.getForm<Region> ctx
        let! result = Repository.update regionId region.Name
        return View.regionRow (RegionEdit.Create (result.RegionId, result.Name))
        }
        |> AspNet.partial ctx

    let editRegion regionId : UserContext =
        fun user ctx ->
        taskResult {
        let! regionId = Task.lift <| user.getRegionId regionId
        let! region = Repository.get regionId
        return View.edit (RegionEdit.Create (region.RegionId, region.Name))
        }
        |> AspNet.partial ctx

    let addRegion : UserContext =
        fun user ctx ->
        taskResult {
        let! region = AspNet.getForm<Region> ctx
        let! added = Repository.add user.UserId region.Name
        User.reset user
        return added
        }
        |> AspNet.createdId ctx Url.show (fun ctx -> Header.resetForm ctx "#new-region")

    let latestRegion (user: User.T) ctx =
        taskResult {
        let! region = Repository.getLastest user.UserId
        return View.regionRow (RegionEdit.Create (region.RegionId, region.Name))
        }
        |> AspNet.partial ctx

    let customEndpoints = router {
        get "/latest" (userContextFunc latestRegion)
    }

    let endpoints = controller {
        index (userContext indexAction)
        show (fun ctx id -> userContext (showRegion id) ctx)
        update (fun ctx id -> userContext (updateRegion id) ctx)
        edit (fun ctx id -> userContext (editRegion id) ctx)
        create (userContext addRegion)
    }

