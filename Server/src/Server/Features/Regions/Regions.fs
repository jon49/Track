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
        use cmd = new DB.track_api.UpdateRegion(conn)
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
        use cmd = new DB.track_api.CreateRegion(conn)
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

    [<CLIMutable>]
    type RegionTeam = {
        [<MaxLength(256)>]
        Name : string
        [<StringLength(128)>]
        FirstName : string[]
        [<StringLength(128)>]
        LastName : string[]
        [<EmailAddress; StringLength(256)>]
        Email : string[]
    }

module Url =

    let index = "/regions"
    let show = sprintf "/regions/%i"
    let latest = "/regions/latest"
    let edit = sprintf "/regions/%i/edit"

    module Team =
        let index = sprintf "/regions/%i/teams"
        let add = sprintf "/regions/%i/teams/add"

    module Coach =
        let add = sprintf "/regions/%i/coaches/add"

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
    open Giraffe

    module M = Utils.Class.M

    let regionAttrId = sprintf "region-%i"

    let addCoach (RegionID.ID regionId) =
        let coach = Coach.Init
        div [] [
        UI.inputText InputSettings.Init coach <@ fun x -> x.FirstName @>
        UI.inputText InputSettings.Init coach <@ fun x -> x.LastName @>
        UI.inputEmail InputSettings.Init coach <@ fun x -> x.Email @>
        hr []
        ]

    let addTeam regionID =
        let (RegionID.ID regionId) = regionID
        let team = Team.Init
        let addAnotherCoachId = "add-another-coach"
        div [ _class M.row; ] [
            label [ _class (M.button + M.Color.primary + M.Col.sm); _for "add-team-modal" ] [ rawText "Add Another Team" ]
            GiraffeViewEngine.input [ _type "checkbox"; _checked ; _id "add-team-modal"; _class "modal" ]
            div [ Accessibility._roleDialog; Accessibility._ariaLabelledBy "add-team-title" ] [
                div [ _style "margin:auto;" ] [
                    form [ _id "new-region-team"; _method "post"; _icPostTo (Url.Team.index regionId); _class (M.container + M.section + Class.scrollable) ] [
                        h3 [ _id "add-team-title" ] [ rawText "New Team" ]
                        div [ _class M.row ] [
                            div [ _class M.Col.sm ] [ UI.inputText InputSettings.Init team <@ fun x -> x.Name @> ]
                            div [ _id "add-coaches"; _class M.Col.sm ] [
                                div [ _id addAnotherCoachId ] [ addCoach regionID ]
                                button [ _icGetFrom (Url.Coach.add regionId); _icTarget ("#"+addAnotherCoachId); _icSwapStyle Append ] [ rawText "Add Another Coach" ]
                                    ] ]
                        div [ _class M.container ] [
                            div [ _class M.row ] [
                                label [ _class (M.button + M.Color.secondary); _for "add-team-modal" ] [ rawText "Cancel" ]
                                button [ _type "reset"; _class M.Color.secondary; ] [ rawText "Reset" ]
                                button [ _type "submit"; _class (M.Color.primary + Class.moveRight) ] [ rawText "Add Team" ] ]
                            ] ] ] ] ]

    let edit (x : RegionEdit) =
        form [ _method "post"; _icPostTo (Url.show x.Id); _icTarget ((+) "#" <| regionAttrId x.Id); ] [
            UI.inputText InputSettings.Init x.Region <@ fun x -> x.Name @>
            button [ _type "submit" ] [ rawText "Save" ] ]

    let regionRow regionAttrId (x : RegionEdit) =
        h2 [] [ str x.Region.Name; UI.editButton (Url.edit x.Id) (Some regionAttrId) ]

    let show (RegionID.ID regionId) (regions : RegionEdit list) (teams : (TeamEdit * (bool * CoachEdit) list) list) =
        let region = regions |> List.find (fun x -> x.Id = regionId)
        let regionAttrId = regionAttrId region.Id
        [
        div [ _id regionAttrId ] [ regionRow regionAttrId region ]
        article [ _class M.container ] [
                yield div [ _class M.row ] [
                    h3 [ _class M.Col.sm ] [ rawText "Teams" ]
                    h3 [ _class M.Col.sm ] [ rawText "Coaches" ] ]
                yield!  teams
                        |> List.map (fun (team, coaches) ->
                            div [ _class M.row ] [
                                Teams.View.show team
                                div [ _class (M.Col.md_ 6) ] (coaches |> List.map (fun x -> x ||> Coaches.View.show))
                            ]
                        )
                yield div [ _class M.row; _id "add-team-button" ] [
                    button [ _class (M.Color.primary + M.Col.sm); _icGetFrom (Url.Team.add regionId); _icTarget "#add-team-button" ] [ rawText "Add Team" ] ] ]
        ]

    let index =
        div []
            [
            h2 [] [ rawText "Regions" ]
            div [] [
                p [] [ rawText "Please enter the name of your new region." ]
                form [ _method "post"; _icPostTo Url.index; _id "new-region" ] [
                    UI.inputText { InputSettings.Init with Attrs = [ _autofocus ]; Label = sprintf "New %s" >> Some } Region.Init <@ fun x -> x.Name @>
                    button [ _type "submit"; _icSuccessAction "[0]reset"; _icActionTarget "#new-region"  ] [ rawText "Submit" ] ]
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
        return View.regionRow (View.regionAttrId result.RegionId) (RegionEdit.Create (result.RegionId, result.Name))
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

    //let createTeam regionId : UserContext =
    //    fun user ctx ->
    //    taskResult {
    //    let! regionId = Task.lift <| user.getRegionId regionId
    //    let! regionTeam = AspNet.getForm<RegionTeam> ctx
    //    let! added = Repository.createRegionTeam regionId regionTeam
    //    User.reset user
    //    return added
    //    } |> AspNet.createdId ctx Url.show (fun ctx -> Header.resetForm ctx "#new-region-team")

    let createRegion : UserContext =
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
        return View.regionRow (View.regionAttrId region.RegionId) (RegionEdit.Create (region.RegionId, region.Name))
        }
        |> AspNet.partial ctx

    let addTeam regionId : UserContext =
        fun user ctx ->
        taskResult {
        let! regionId = Task.lift <| user.getRegionId regionId
        return View.addTeam regionId
        } |> AspNet.partial ctx

    let addCoach regionId : UserContext =
        fun user ctx ->
        taskResult {
        let! regionId = Task.lift <| user.getRegionId regionId
        return View.addCoach regionId
        } |> AspNet.partial ctx

    let customEndpoints = router {
        get "/latest" (userContextFunc latestRegion)
    }

    let teamsEndpoints regionId = controller {
        add (userContext (addTeam regionId))
        //create (userContext (createTeam regionId))
    }

    let coachesEndpoints regionId = controller {
        add (userContext (addCoach regionId))
    }

    let endpoints = controller {
        subController "/teams" teamsEndpoints
        subController "/coaches" coachesEndpoints
        index (userContext indexAction)
        show (fun ctx id -> userContext (showRegion id) ctx)
        update (fun ctx id -> userContext (updateRegion id) ctx)
        edit (fun ctx id -> userContext (editRegion id) ctx)
        create (userContext createRegion)
    }

