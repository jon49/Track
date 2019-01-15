namespace Teams

module Database =

    open Model
    open FSharp.Control.Tasks
    open System.Threading.Tasks

    let mutable private teams = [
        {
            Data = {
                Email = "millie@dns.com"
                FirstName = "Millie"
                LastName = "Nyman"
                TeamName = "Fastest Team Ever"
            }
            TeamId = 1
            UserId = 1
        }
        {
            Data = {
                Email = "jorge@321.com"
                FirstName = "Jorge"
                LastName = "George"
                TeamName = "Really Fast Team"
            }
            TeamId = 2
            UserId = 2
        }
    ]

    let all () =
        Task.FromResult <| Ok teams

    let add (team : Data) =
        let newId = teams.Length + 1
        teams <- { Data = team; TeamId = newId; UserId = newId }::teams
        Ok ()
        |> Task.FromResult

    let find (teamId : int) (userId : int) =
        teams
        |> List.tryFind (fun x -> x.TeamId = teamId && x.UserId = userId)
        |> Ok
        |> Task.FromResult

    let getLastest () =
        match teams with
        | team::xs -> Ok team
        | _ -> Error "Could not get latest team."
        |> Task.FromResult

    let update (team : Team) =
        teams <- teams
        |> List.map (fun x -> 
            if x.TeamId = team.TeamId && x.UserId = team.UserId
                then team
            else x )
        Ok ()
        |> Task.FromResult
        

