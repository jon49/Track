namespace Track

module Register =
    open Giraffe
    open Saturn
    open FSharp.Control.Tasks
    open Track.AspNet
    open Track.Repository

    let user : HttpHandler =
        fun next ctx ->
        task {
            match AspNet.isAuthenticated ctx with
            | false -> ()
            | true ->
                let authId = AspNet.getAuth0Id ctx
                match! User.getUserPermissions authId with
                | Some _ -> ()
                | None ->
                    match! User.addUser ctx with
                    | Some _ ->
                        Cache.remove <| User.cacheKey authId
                        let! _ = User.getUserPermissions authId
                        ()
                    | None -> ()
            return! next ctx
        }
