namespace Teams

module Model =
    open Track.UI
    open Utils.Reflection
    open Utils.Validation
    open Newtonsoft.Json

    [<CLIMutable>]
    type Data = {
        TeamName : string
        FirstName : string
        LastName : string
        Email : string
    }

    type Team = {
        Data : Data
        TeamId : int
        UserId : int
    }

    let teamUI =
        Map.empty<string, Model>
        |> Map.add
            (propertyName <@ fun x -> x.TeamName @>)
            { DisplayName = "Team"; Validation = [| Validation.Required; MaxLength 256 |] }
        |> Map.add
            (propertyName <@ fun x -> x.FirstName @>)
            { DisplayName = "First Name"; Validation = [| Validation.Required; MaxLength 128 |] }
        |> Map.add
            (propertyName <@ fun x -> x.LastName @>)
            { DisplayName = "Last Name"; Validation = [| Validation.Required; MaxLength 128 |] }
        |> Map.add
            (propertyName <@ fun x -> x.Email @>)
            { DisplayName = "Email"; Validation = [| Validation.Required; MaxLength 256; Validation.Email |] }
    

