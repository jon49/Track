namespace Track.Regions

module Repository =

    open Track.Repository
    open Track.User

    let getAll (UserID.ID userId) =
        tryList (fun () ->
            use cmd = new DB.track_api.GetRegions(conn)
            cmd.AsyncExecute(userId) )

    let update (RegionID.ID regionId) name =
        tryId (fun () ->
        use cmd = new DB.track_api.EditRegion(conn)
        cmd.AsyncExecute(regionId, name) )

    let add (UserID.ID userId) regionName =
        tryId (fun () ->
        use cmd = new DB.track_api.AddRegion(conn)
        cmd.AsyncExecuteSingle(userId, regionName) )

    let getLastest (UserID.ID userId) =
        tryId (fun () ->
        use cmd = new DB.track_api.GetRegionLatest(conn)
        cmd.AsyncExecuteSingle(userId) )

module Model =
    open System.ComponentModel.DataAnnotations

    [<Struct; CLIMutable>]
    type Region = {
        [<MaxLength(256)>]
        Name : string
    } with
        static member Init = { Name = "" }

    [<Struct>]
    type RegionEdit = {
        Region : Region
        Id : int
    }

open Model

module Url =

    let index = "/regions"
    let show = sprintf "/regions/%i"
    let latest = "/regions/latest"

module View =
    open Giraffe.GiraffeViewEngine
    open Utils.ViewEngine
    open Utils
    open Model
    open Utils.UI

    let index (regions : RegionEdit list) =
        let regionForms =
            match List.isEmpty regions with
            | true -> []
            | false ->
                let settings = { InputSettings.Init with AddLabel = false }
                regions
                |> List.map (fun x ->
                    form [ _method "post"; _icPostTo (Url.show x.Id) ] [
                        yield! UI.inputText settings x.Region <@ fun x -> x.Name @>
                        yield button [ _type "submit" ] [ rawText "Save" ] ]
                )
        [
        h2 [] [ rawText "Regions" ]
        div [ _icAppendFrom Url.latest ] regionForms
        p [] [ rawText "Please enter the name of your new region." ]
        form [ _method "post"; _icPostTo Url.index ] [
            yield! UI.inputText { InputSettings.Init with Attrs = [ _autofocus ] } Region.Init <@ fun x -> x.Name @>
            yield button [ _type "submit"; _icAction "parent.reset" ] [ rawText "Submit" ] ]
        ]

module Map =
    open Model
    open Track.Repository

    let regionEdit (region : DB.track_api.GetRegions.Record) =
        {
            Region = { Name = region.Name }
            Id = region.RegionId
        }

module Controller =
    open Saturn
    open Microsoft.AspNetCore.Http
    open Track
    open Track.AspNet
    open Utils
    open Utils.TaskResult.Symbols

    let indexAction : HtmlFunc =
        fun ctx user next ->
        Repository.getAll user.UserId
        |> AspNet.toHttpResult ctx (fun xs -> next (View.index (xs |> List.map Map.regionEdit)) )

    let editRegion regionId : HtmlFunc =
        fun ctx user next ->
        let regionId = Task.lift <| user.getRegionId regionId
        let name = (fun x -> x.Name) <!> (AspNet.getForm<Region> ctx)
        let result = Repository.update <!> regionId <*> name
        result
        |> AspNet.toHttpResult ctx (fun xs -> next (View.index xs))

    let regionsController = controller {
        index (htmlPipeline indexAction)
        edit (fun ctx id -> htmlPipeline (editRegion id) ctx)
    }

