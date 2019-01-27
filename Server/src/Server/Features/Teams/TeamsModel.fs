namespace Teams

module Model =
    open Utils.Client
    open Utils.ClientDefinition

    [<CLIMutable>]
    type Team = {
        Name : string
    } with
        static member TeamNameInfo =
            { DisplayName = "Team"
              Get = fun x -> x.Name
              Name = <@ fun x -> x.Name @>
              Validations = [| maxLength 128 |] }
        interface IModelClean<Data> with
            member __.Clean () =
                { Name = __.Name.Trim() }
        interface Utils.Client.IModelValidation<Data> with    
            member __.Validate () =
                let prove = prove __
                proof [
                    prove Data.TeamNameInfo
                    prove Data.FirstNameInfo
                    prove Data.LastNameInfo
                    prove Data.EmailInfo
                ] |> function
                     | Some error -> Error error
                     | None -> Ok __

    [<CLIMutable>]
    type TeamEdit = {
        Team : Team
        Id : int
    }

    [<CLIMutable>]
    type Coach = {
        FirstName : string
        LastName : string
        Email : string
    } with
        static member FirstNameInfo =
            { DisplayName = "First Name"
              Get = fun x -> x.FirstName
              Name = <@ fun x -> x.FirstName @>
              Validations = [| maxLength 128 |] }
        static member LastNameInfo =
            { DisplayName = "Last Name"
              Get = fun x -> x.LastName
              Name = <@ fun x -> x.LastName @>
              Validations = [| maxLength 128 |] }
        static member EmailInfo =
            { DisplayName = "Email"
              Get = fun x -> x.Email
              Name = <@ fun x -> x.Email @>
              Validations = [| maxLength 256; email |] }

    [<CLIMutable>]
    type CoachNew = {
        Coach : Coach
        TeamId : int
    }

    [<CLIMutable>]
    type CoachEdit = {
        Coach : Coach
        Id : int
    }

    [<CLIMutable>]
    type Data = {
            TeamName : string
            FirstName : string
            LastName : string
            Email : string
        } with

    type Team = {
        Data : Data
        TeamId : int
        UserId : int
    }
