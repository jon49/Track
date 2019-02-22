namespace Track.Regions

module Repository =

    open Track.Repository

    let conn = Track.Settings.Setting.Database.Connection

    let getAll userId =
        async {
        use cmd = new StoredProcedures.Track.track.GetRegions(conn)
        let! regions = cmd.AsyncExecute(userId)
        return regions |> Seq.toList
        }
        |> Async.StartAsTask

    let update regionId name =
        async {
        use cmd = new StoredProcedures.Track.track.EditRegion(conn)
        return! cmd.AsyncExecute(regionId, name)
        }
        |> Async.StartAsTask

    let add userId regionName =
        async {
        use cmd = new StoredProcedures.Track.track.AddRegion(conn)
        return! cmd.AsyncExecuteSingle(userId, regionName)
        }
        |> Async.StartAsTask

    let getLastest userId =
        async {
        use cmd = new StoredProcedures.Track.track.GetRegionLatest(conn)
        return! cmd.AsyncExecuteSingle(userId)
        }
        |> Async.StartAsTask

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

    let regionEdit (region : Track.Repository.StoredProcedures.Track.track.GetRegions.Record) =
        {
            Region = { Name = region.Name }
            Id = region.RegionId
        }

module Controller =
    open Saturn
    open FSharp.Control.Tasks
    open Microsoft.AspNetCore.Http
    open Track

    let indexAction (ctx : HttpContext) (user : Track.User.T) =
        task {
            let! regions = Repository.getAll user.UserId
            return View.index (regions |> List.map Map.regionEdit)
        }

    let editRegion id (ctx : HttpContext) (user : Track.User.T) =
        task {
            let! region =
                Controller.getForm<Region>(ctx)
            let! updated = Repository.update id
            return View.index []
        }

    let regionsController = controller {
        index (htmlPipeline indexAction)
        edit (fun id -> htmlPipeline <| editRegion id)
    }

