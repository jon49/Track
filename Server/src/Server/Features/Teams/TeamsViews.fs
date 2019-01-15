namespace Teams

module View =

    open Giraffe.GiraffeViewEngine
    open Model
    open Utils.ViewEngine
    open Track.UI

    let team (team : Team) = [
        td [ _id (sprintf "team-name-%i" team.TeamId) ] [ str team.Data.TeamName ]
        td [ _id (sprintf "user-name-%i" team.UserId) ] [ str (sprintf "%s %s" team.Data.FirstName team.Data.LastName) ]
        td [ _id (sprintf "user-email-%i" team.UserId) ] [ str team.Data.Email ]
        td [] [
            a [ _icTarget "#edit"; _icGetFrom (Url.View.edit team.TeamId team.UserId); _title "Edit" ] [
                rawText "&laquo;" ] ] ]

    let teamRow (x : Team) =
        tr [ _icSrc (Url.show x.TeamId x.UserId); _icDeps "ignore" ] (team x)

    let addEditTeamForm (maybeTeam : Team option) =
        let team =
            match maybeTeam with
            | Some x -> x.Data
            | None -> { FirstName = ""
                        LastName = ""
                        Email = ""
                        TeamName = "" }
        let postTo =
            match maybeTeam with
            | Some x -> Url.show x.TeamId x.UserId
            | None -> Url.index
        form [ _method "post"; _icPostTo postTo ] [
            fieldset [] [
                fieldset [] (field "text" [ _autofocus ] Model.teamUI <@ team.TeamName @>)
                fieldset [] [
                    yield  legend [] [ rawText "Coach" ]
                    yield! field "text" [] Model.teamUI <@ team.FirstName @>
                    yield  br []
                    yield! field "text" [] Model.teamUI <@ team.LastName @>
                    yield  br []
                    yield! field "email" [] Model.teamUI <@ team.Email @> ]
                button [ _type "submit" ] [ rawText "Submit" ]
                button [ _icGetFrom Url.View.addTeamButton; _icTarget "#edit" ] [ rawText "Cancel" ]
            ]
        ]

    let addTeamButton =
        button [ _icGetFrom Url.View.addTeamForm; _icTarget "#edit"; _autofocus ] [ rawText "Add Team" ]

    let all (teams : Team list) = [
        table [] [
            thead [] [
                tr [] [
                    th [] [ rawText "Team Name" ]
                    th [] [ rawText "Coach Name" ]
                    th [] [ rawText "Coach Email" ] ] ]
            tbody [ _id "teams"; _icAppendFrom Url.latest ] (teams |> List.map teamRow) ]
        div [ _id "edit" ] [ addTeamButton ] ]

