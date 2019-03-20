namespace Track.Regions

module Repository =

    open Track.Repository
    open Track.User
    open Track.Settings
    open FSharp.Data
    open Utils

    let conn = Setting.Database.Connection

    type RegionTeams = DB.track_api._GetRegionTeams.Record

    let getLatestTeam (RegionID.ID regionId) =
        async {
        use cmd = new SqlCommandProvider<"SELECT * FROM [track_api].[GetLatestTeamWithCoaches](@RegionId)", CONNECTION_STRING, ResultType = ResultType.Tuples>(conn)
        let! result = cmd.AsyncExecute(regionId)
        return result |> Seq.map (fun x -> new RegionTeams(x)) |> Seq.toList
        } |> list

    let getAllTeams (RegionID.ID regionId) =
        async {
        use cmd = new SqlCommandProvider<"SELECT * FROM [track_api].GetRegionTeams(@RegionId)", CONNECTION_STRING, ResultType = ResultType.Tuples>(conn)
        let! result = cmd.AsyncExecute(regionId)
        return result |> Seq.map (fun x -> new RegionTeams(x)) |> Seq.toList
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

    let createRegionTeam (RegionID.ID regionId) users teamName =
        async {
        use cmd = new DB.track_api.CreateRegionTeam(conn)
        return! cmd.AsyncExecuteSingle(regionId, teamName, users)
        } |> single

    let createTeamWithCoordinator (RegionID.ID regionId) (UserID.ID userId) teamName =
        async {
        use cmd = new DB.track_api.CreateRegionTeamWithCoordinator(conn)
        return! cmd.AsyncExecuteSingle(regionId, userId, teamName)
        } |> single

    let create (UserID.ID userId) regionName =
        async {
        use cmd = new DB.track_api.CreateRegion(conn)
        return! cmd.AsyncExecuteSingle(userId, regionName)
        } |> single

    let getLastest (UserID.ID userId) =
        async {
        use cmd = new DB.track_api.GetRegionLatest(conn)
        return! cmd.AsyncExecuteSingle(userId)
        } |> single

module Model =
    open System.ComponentModel.DataAnnotations
    open Track.User

    [<CLIMutable>]
    type Region = {
        [<Display(Name = "Region Name"); MaxLength(256)>]
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
    type RegionTeamForm = {
        [<EmailAddress; MaxLength(256)>]
        Email : string option
        [<Display(Name = "First Name"); MaxLength(128)>]
        FirstName : string option
        IAmTheCoach : bool
        [<Display(Name = "Last Name"); MaxLength(128)>]
        LastName : string option
        [<Display(Name = "Team Name"); MaxLength(256)>]
        TeamName : string
    } with
        static member Init =
            { TeamName = ""
              FirstName = Some ""
              LastName = Some ""
              Email = Some ""
              IAmTheCoach = false }

open Model

module Mapping =
    open Track
    open Teams.Model
    open Coaches.Model
    open Repository

    type TeamCoachEdit = TeamEdit * ((bool * CoachEdit) list)

    let toGroupTeams (teams : RegionTeams list) : TeamCoachEdit list =
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

    let toBasicUser (user : RegionTeamForm) =
        match user.Email, user.FirstName, user.LastName with
        | Some email, Some firstName, Some lastName ->
            Ok <| new DB.track.``User-Defined Table Types``.BasicUser(email, firstName, lastName)
        | _ -> Error <| ValidationError "Coaches must have values."

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

    module TeamCoach  =
        let latest = sprintf "/regions/%i/team-coaches/latest"

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
    module P = Class.P

    let regionAttrId = sprintf "region-%i"

    let edit (x : RegionEdit) =
        form [ _method "post"; _icPostTo (Url.show x.Id); _icTarget ((+) "#" <| regionAttrId x.Id); ] [
            UI.inputText InputSettings.Init x.Region <@ fun x -> x.Name @>
            UI.saveButton ]

    let regionRow regionAttrId (x : RegionEdit) =
        h2 [] [ str x.Region.Name; UI.editButton (Url.edit x.Id) (Some regionAttrId) ]

    let show (RegionID.ID regionId) (regions : RegionEdit list) (teams : (TeamEdit * (bool * CoachEdit) list) list) =
        let region = regions |> List.find (fun x -> x.Id = regionId)
        let regionAttrId = regionAttrId region.Id
        [
        div [ _id regionAttrId ] [ regionRow regionAttrId region ]
        article [] [
                div [ _class P.flex ] [
                    h3 [] [ rawText "Teams" ]
                    h3 [] [ rawText "Coaches" ] ]
                div [ _id "teams"; _class (P.flex + P.Flex.two) ] (
                        teams
                        |> List.map (fun (team, coaches) ->
                            [ Teams.View.show team
                              div [] (coaches |> List.map (fun x -> x ||> Coaches.View.show)) ] )
                        |> List.concat )
                div [ _class P.flex; _id "add-team-button" ] [
                    button [ _icGetFrom (Url.Team.add regionId); _icTarget "#add-team-button" ] [ rawText "Add Team" ] ] ]
        ]

    let region (x : RegionEdit) =
        let id = sprintf "region-%i" x.Id
        div [ _id id; ] [
            a [ _href (Url.show x.Id) ] [ str x.Region.Name; UI.editButton (Url.edit x.Id) (Some id) ]
        ]

    let index (regions : RegionEdit list) =
        [
        h2 [] [ rawText "Regions" ]
        div [ _id "regions"; _icAppendFrom Url.latest ] (regions |> List.map region)
        div [] [
            p [] [ rawText "Please enter the name of your new region." ]
            form [ _method "post"; _icPostTo Url.index; _id "new-region" ] [
                UI.inputText { InputSettings.Init with Attrs = [ _autofocus ] } Region.Init <@ fun x -> x.Name @>
                button [ _type "submit"; _icSuccessAction "[0]reset"; _icActionTarget "#new-region"  ] [ rawText "Submit" ] ]
            ] ]

    module Coach =

        let add (RegionID.ID regionId) =
            let coach = Coach.Init
            div [ _id "coach-form" ] [
            UI.inputText InputSettings.Init coach <@ fun x -> x.FirstName @>
            UI.inputText InputSettings.Init coach <@ fun x -> x.LastName @>
            UI.inputEmail InputSettings.Init coach <@ fun x -> x.Email @>
            //hr []
            ]

    module Team =
        open Utils.Class.P.Flex

        let add regionID =
            let (RegionID.ID regionId) = regionID
            let team = RegionTeamForm.Init
            let addAnotherCoachId = "add-another-coach"
            div [] [
                input [ _type "checkbox"; _checked ; _id "add-team-modal"; _class Class.modal ]
                label [ _for "add-team-modal"; _class P.button ] [ rawText "Add Another Team" ]
                div [ _class Class.modalTarget ] [
                    form [ _id "new-region-team"; _method "post"; _icPostTo (Url.Team.index regionId); ] [
                        fieldset [ _id "add-team-title" ] []
                        legend [] [ h2 [] [ rawText "New Team" ] ]
                        div [ _class (P.flex + P.Flex.one + (P.Flex.two_ S800)) ] [
                            div [ _class Class.padding ] [
                                UI.inputText { InputSettings.Init with Attrs = [ _autofocus ] } team <@ fun x -> x.TeamName @>
                                label [ _for "i-am-the-coach" ] [
                                    input [ _tabindex "30"; _type "checkbox"; _id "i-am-the-coach"; _name "IAmTheCoach" ]
                                    span [ _class P.checkable ] [ rawText "I am the coach" ] ] ]
                            div [ _id "add-coaches"; _class Class.padding ] [
                                div [ _id addAnotherCoachId ] [ Coach.add regionID ]
                                //button [ _type "button"; _id "add-another-coach-button"; _icGetFrom (Url.Coach.add regionId); _icTarget ("#"+addAnotherCoachId); _icSwapStyle Append ] [ rawText "Add Another Coach" ]
                                    ] ]
                        div [] [
                            div [] [
                                label [ _class ""; _for "add-team-modal"; _tabindex "28" ] [ rawText "Cancel" ]
                                button [ _type "reset"; _class ""; _tabindex "29" ] [ rawText "Reset" ]
                                button [ _type "submit"; _class (Class.moveRight) ] [ rawText "Add Team" ] ] ] ] ]
                script [ _src "/scripts/region-teams.js" ] [] ]


    module TeamCoach =

        open Mapping

        let teamRow (team : TeamEdit) =
            let teamId = sprintf "team-%i" team.Id
            div [ _id teamId ] [
                a [ _href (Teams.Url.show team.Id) ] [ str team.Team.Name; UI.editButton (Teams.Url.edit team.Id) (Some teamId) ] ]

        let coachRow (registered, coach : CoachEdit) =
            let coachId = sprintf "coach-%i" coach.Id
            div [ _id coachId ] [
                p [] [
                    span [] [ str <| sprintf "%s %s" coach.Coach.FirstName coach.Coach.LastName ]
                    span [ _class (if registered then Class.Icon.checkCircle else Class.Icon.slash) ] []
                    br []
                    span [] [ str coach.Coach.Email ]
                    UI.editButton (Coaches.Url.edit coach.Id) (Some coachId) ] ]
 
        let row ((team, coaches) : TeamCoachEdit) =
            let coachId = sprintf "coach-%i"
            div [ _class Class.P.flex ] [
                div [ _id (sprintf "team-%i-coaches" team.Id) ] (coaches |> List.map coachRow)
            ]

        let show (RegionID.ID regionId) (teamCoaches : TeamCoachEdit list) =
            [
            h3 [] [ rawText "Teams" ]
            div [ _icAppendFrom (Url.Team.index regionId) ] (teamCoaches |> List.map row)
            ]

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
        | Some _, false -> AspNet.html ctx <| (Task.lift <| Error [ Errors.AuthorizationError ])
        | None, _ ->
            AspNet.html ctx <| (Task.lift <| Ok (App.layout (App.UserLayout.Init(user)) (View.index [])))

    let showRegion regionId : UserContext =
        fun user ctx ->
        taskResult {
        let! regionId = Task.lift <| user.getRegionId regionId
        let! regions, teams =
            TaskResult.T.Parallel(Repository.getAll user.UserId, Repository.getAllTeams regionId)
        let teams = Mapping.toGroupTeams teams
        let regions = regions |> List.map (fun x -> RegionEdit.Create(x.RegionId, x.Name))
        return App.layout (App.UserLayout.Init(user)) <| View.show regionId regions teams
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

    let createTeam regionId : UserContext =
        fun user ctx ->
        taskResult {
        let! regionId = Task.lift <| user.getRegionId regionId
        let! regionTeam = AspNet.getForm<RegionTeamForm> ctx
        let! created =
            match regionTeam.IAmTheCoach with
            | true ->
                User.reset user
                Repository.createTeamWithCoordinator regionId user.UserId regionTeam.TeamName
            | false ->
                // Further validation
                let coaches = [| regionTeam.Email; regionTeam.FirstName; regionTeam.LastName |]
                if (coaches |> Array.exists Option.isNone)
                    then Task.lift <| Error [ Errors.ValidationError "All coaches must have a an email, first and last names." ]
                else
                    let firstItemLength = coaches.[0].Value.Length
                    if (coaches |> Array.choose id |> Array.forall (fun xs -> not (xs.Length = firstItemLength)))
                        then Task.lift <| Error [ Errors.ValidationError "All coaches must have a an email, first and last names." ]
                    else
                        match Mapping.toBasicUser regionTeam with
                        | Ok coaches ->
                            Repository.createRegionTeam regionId [| coaches |] regionTeam.TeamName
                        | Error x ->
                            Task.lift <| Error [ x ]
        return created
        } |> AspNet.maybeCreatedId ctx Url.show (fun ctx -> Header.resetForm ctx "#new-region-team")

    let createRegion : UserContext =
        fun user ctx ->
        taskResult {
        let! region = AspNet.getForm<Region> ctx
        let! added = Repository.create user.UserId region.Name
        User.reset user
        return added
        }
        |> AspNet.createdId ctx Url.show (fun ctx -> Header.resetForm ctx "#new-region")

    let latestRegion (user: User.T) ctx =
        taskResult {
        let! region = Repository.getLastest user.UserId
        return View.region (RegionEdit.Create(region.RegionId, region.Name))
        }
        |> AspNet.partial ctx

    let latestTeam regionId : UserContext =
        fun user ctx ->
        taskResult {
        let! regionId = Task.lift <| user.getRegionId regionId
        let! team = Repository.getLatestTeam regionId
        return View.TeamCoach.show regionId (Mapping.toGroupTeams team)
        } |> AspNet.partials ctx

    let addTeam regionId : UserContext =
        fun user ctx ->
        taskResult {
        let! regionId = Task.lift <| user.getRegionId regionId
        return View.Team.add regionId
        } |> AspNet.partial ctx

    let addCoach regionId : UserContext =
        fun user ctx ->
        taskResult {
        let! regionId = Task.lift <| user.getRegionId regionId
        return View.Coach.add regionId
        } |> AspNet.partial ctx

    let customEndpoints = router {
        get "/latest" (userContextFunc latestRegion)
        getf "/%i/teams/latest" (fun regionId next ctx -> userContext (latestTeam regionId) ctx)
    }

    let teamsEndpoints regionId = controller {
        add (userContext (addTeam regionId))
        create (userContext (createTeam regionId))
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

