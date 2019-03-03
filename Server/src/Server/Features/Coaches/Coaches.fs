namespace Track.Coaches

module Model =
    open System.ComponentModel.DataAnnotations

    [<CLIMutable>]
    type Coach = {
        [<Display(Name = "First Name"); StringLength(128, MinimumLength = 1)>]
        FirstName : string
        [<Display(Name = "Last Name"); StringLength(128, MinimumLength = 1)>]
        LastName : string
        [<Display(Name = "Email"); EmailAddress; StringLength(256, MinimumLength = 1)>]
        Email : string
    }

    [<CLIMutable>]
    type CoachEdit = {
        Coach : Coach
        [<Range(1, System.Int32.MaxValue)>]
        Id : int
    } with
        static member Create(id, firstName, lastName, email) =
            {
            Id = id
            Coach = {
                FirstName = firstName
                LastName = lastName
                Email = Option.defaultValue "" email }
            }

open Model

module Url =

    let index = "/coaches"
    let show = sprintf "/coaches/%i"
    let latest = "/coaches/latest"
    let edit = sprintf "/coaches/%i/edit"

module View =
    open Giraffe.GiraffeViewEngine
    open Utils

    let show hasBeenVerified (coach : CoachEdit) =
        let coachId = coach.Id
        let coach = coach.Coach
        let verified =
            if hasBeenVerified then
                let message = "The coach has been verified."
                img [ _alt message; _src "/images/check-circle.svg"; _title message ]
            else
                let message =  "The coach has NOT been verified."
                img [ _alt message; _src "/images/slash.svg"; _title message ]
        article [ _class Class.P.card ] [
            p [] [ str <| sprintf "%s %s" coach.FirstName coach.LastName ]
            p [] [ str coach.Email; verified ]
            UI.editButton (Url.edit coachId) None ]
