namespace Track.FirstTime

module Model =

    open Track.Repository
    open Track.Settings

    let getNewCoordinatorRegionId guid userId =
        async {
        use cmd = new StoredProcedures.Track.track.NewCoordinator(Setting.Database.Connection)
        return! cmd.AsyncExecuteSingle(guid, userId)
        }
        |> Async.StartAsTask

    let getNewCoacTeamId guid userId =
        async {
        use cmd = new StoredProcedures.Track.track.NewCoach(Setting.Database.Connection)
        return! cmd.AsyncExecuteSingle(guid, userId)
        }
        |> Async.StartAsTask
