namespace Teams

//module View =

//    open Giraffe.GiraffeViewEngine
//    open Model
//    open Utils.ViewEngine
//    open Utils.UI

//    let team (team : TeamEdit) (coaches : CoachEditNew list) =
//        let teamLocator = sprintf "team-id-%i" team.Id
//        let coachLocator id = sprintf "coach-id-%i" id
//        let edit ``type`` id locator =
//            button [ _icTarget ("#" + locator); _icGetFrom (sprintf "/%s/%i" ``type`` id) ] [ rawText "Edit" ]
//        [
//        div [ _id teamLocator ] [
//            a [ _href "#" ] [ str team.Team.Name ]
//            edit "teams" team.Id teamLocator
//        ]
//        div [] (
//            coaches
//            |> List.map (fun x ->
//                div [ _id (coachLocator x.Id) ] [
//                    a [ _href "#" ] [
//                        span [] [ str (sprintf "%s %s" x.Coach.FirstName x.Coach.LastName) ]
//                        span [] [ str x.Coach.Email ] ]
//                    edit "coaches" x.Id (coachLocator x.Id)
//                ]
//            )
//        )
//        div [ _id "add-coach" ] []
//        button [ _icTarget "#add-coach"; _icGetFrom (Url.View.addCoachForm team.Id); _title "Add" ] [
//                rawText "Add Coach" ]
//        ]

//    let teamRow (x : Team) =
//        div [ _icSrc (Url.show x.TeamId x.UserId); _icDeps "ignore" ] (team x)

//    let addEditTeamForm (maybeTeam : Team option) =
//        let team, target =
//            match maybeTeam with
//            | Some x -> x.Data, Url.show x.TeamId x.UserId
//            | None -> { FirstName = ""
//                        LastName = ""
//                        Email = ""
//                        TeamName = "" }, ""
//        let postTo =
//            match maybeTeam with
//            | Some x -> Url.show x.TeamId x.UserId
//            | None -> Url.index
//        form [ _method "post"; _icPostTo postTo ] [
//            fieldset [ _class "show" ] [
//                fieldset [] [ inputText Data.TeamNameInfo team [ _autofocus ] ]
//                fieldset [ _class "show" ] [
//                    legend [] [ rawText "Coach" ]
//                    inputText Data.FirstNameInfo team []
//                    inputText Data.LastNameInfo team []
//                    inputEmail Data.EmailInfo team [] ]
//                button [ _type "submit"; _icActionTarget target ] [ rawText "Submit" ]
//                button [ _icGetFrom Url.View.addTeamButton; _icTarget "#edit" ] [ rawText "Cancel" ]
//            ]
//        ]

//    let addTeamButton =
//        button [ _icGetFrom Url.View.addTeamForm; _icTarget "#edit"; _autofocus ] [ rawText "Add Team" ]

//    let all (teams : Team list) = [
//        header [] [
//            strong [] [ rawText "Team Name" ]
//            strong [] [ rawText "Coach Name" ]
//            strong [] [ rawText "Coach Email" ]
//        ]
//        div [ _id "teams"; _icAppendFrom Url.latest ] (teams |> List.map teamRow)
//        div [ _id "edit" ] [ addTeamButton ] ]

