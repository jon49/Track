﻿namespace Track.Teams

module Repository =

    open Track.Repository
    open Track.User
    open Track.Settings

    let conn = Setting.Database.Connection

    let getAll (UserID.ID userId) =
        async {
        use cmd = new DB.track_api.GetTeams(conn)
        let! result = cmd.AsyncExecute(userId)
        return Seq.toList result
        }
        |> list

    let update (TeamID.ID teamId) name =
        async {
        use cmd = new DB.track_api.EditTeam(conn)
        return! cmd.AsyncExecuteSingle(teamId, name)
        }
        |> single

    let get (TeamID.ID teamId) =
        async {
        use cmd = new DB.track_api.GetTeam(conn)
        return! cmd.AsyncExecuteSingle(teamId)
        }
        |> single

    let add (UserID.ID userId, RegionID.ID regionId) teamName =
        async {
        use cmd = new DB.track_api.AddTeam(conn)
        return! cmd.AsyncExecuteSingle(regionId, userId, teamName)
        }
        |> single

    let getLastest (UserID.ID userId) =
        async {
        use cmd = new DB.track_api.GetTeamLatest(conn)
        return! cmd.AsyncExecuteSingle(userId)
        }
        |> single


module Model =
    open System.ComponentModel.DataAnnotations

    [<CLIMutable>]
    type Team = {
        [<MaxLength(256)>]
        Name : string
    } with
        static member Init = { Name = "" }

    [<Struct>]
    type TeamEdit = {
        Team : Team
        Id : int
    } with
        static member Create (id, name) =
            { Team = { Name = name }
              Id = id }

module Url =

    let index = "/teams"
    let show = sprintf "/teams/%i"
    let latest = "/teams/latest"
    let edit = sprintf "/teams/%i/edit"

module View =
