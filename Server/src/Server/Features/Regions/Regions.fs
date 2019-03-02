namespace Track.Regions

module Repository =

    open Track.Repository
    open Track.User
    open Track.Settings

    let conn = Setting.Database.Connection

    let getAll (UserID.ID userId) =
        async {
        use cmd = new DB.track_api.GetRegions(conn)
        let! result = cmd.AsyncExecute(userId)
        return Seq.toList result
        }
        |> list

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

    let edit (x : RegionEdit) =
        form [ _method "post"; _icPostTo (Url.show x.Id) ] [
            yield! UI.inputText InputSettings.Init x.Region <@ fun x -> x.Name @>
            yield button [ _type "submit" ] [ rawText "Save" ] ]

    let regionRow =
        let getRegionShowId = sprintf "region-%i"
        fun x ->
        li [ _id (getRegionShowId x.Id) ] [
            a [ _icGetFrom (Url.edit x.Id) ] [ str x.Region.Name ]
            button [ _icGetFrom (Url.edit x.Id); _icTarget ("#" + (getRegionShowId x.Id)) ] [ rawText "Edit" ]
            ]

    let index =
        fun regions ->
        let regionForms =
            regions
            |> List.map regionRow
        [
        h2 [] [ rawText "Regions" ]
        ul [ _icAppendFrom Url.latest ] regionForms
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

    let indexAction : UserContext =
        fun user ctx ->
        taskResult {
        let! regions = Repository.getAll user.UserId
        return App.layout user.Type (View.index (regions |> List.map (fun x -> RegionEdit.Create(x.RegionId, x.Name))))
        }
        |> AspNet.html ctx

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
        update (fun ctx id -> userContext (updateRegion id) ctx)
        edit (fun ctx id -> userContext (editRegion id) ctx)
        create (userContext addRegion)
    }

