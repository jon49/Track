namespace Track

module Register =
    open Giraffe
    open Saturn
    open FSharp.Control.Tasks
    open Track.AspNet
    open Track.Repository
    open Utils.TaskResult

    let user : HttpHandler =
        fun next ctx ->
        task {
            match AspNet.isAuthenticated ctx with
            | false -> ()
            | true ->
                let authId = AspNet.getAuth0Id ctx
                match! User.getUserPermissions authId with
                | Ok _ -> ()
                | Error _ ->
                    match! User.addUser ctx with
                    | Ok _ ->
                        Cache.remove <| User.cacheKey authId
                        let! _ = User.getUserPermissions authId
                        ()
                    | Error _ -> ()
            return! next ctx
        }
