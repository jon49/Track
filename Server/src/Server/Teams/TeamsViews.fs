﻿namespace Teams

module View =

    open Giraffe.GiraffeViewEngine
    open Model
    open Track.ViewEngine

    let team (team : Team) = [
        td [ _id (sprintf "team-name-%i" team.TeamId) ] [ str team.Data.TeamName ]
        td [ _id (sprintf "user-name-%i" team.UserId) ] [ str (sprintf "%s %s" team.Data.FirstName team.Data.LastName) ]
        td [ _id (sprintf "user-email-%i" team.UserId) ] [ str team.Data.Email ]
        td [] [
            a [ _icTarget "#edit"; _icGetFrom (Url.Partial.userForm team.TeamId team.UserId); _title "Edit" ] [
                rawText "&laquo;" ] ] ]

    let teamRow (x : Team) =
        tr [ _icSrc (Url.Put.team x.TeamId x.UserId); _icDeps "ignore" ] (team x)

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
            | Some x -> Url.Put.team x.TeamId x.UserId
            | None -> Url.Post.``new``
        form [ _method "post"; _icPostTo postTo ] [
            fieldset [] [
                fieldset [] (field "text" [ _autofocus ] <@ team.TeamName @>)
                fieldset [] (
                    [ legend [] [ rawText "Coach" ] ]
                    @ (field "text" [] <@ team.FirstName @>)
                    @ (field "text" [] <@ team.LastName @>)
                    @ [ br [] ]
                    @ (field "email" [] <@ team.Email @>))
                button [ _type "submit" ] [ rawText "Submit" ]
                button [ _icGetFrom Url.Partial.addTeamButton; _icTarget "#edit" ] [ rawText "Cancel" ]
            ]
        ]

    let addTeamButton =
        button [ _icGetFrom Url.Partial.addTeamForm; _icTarget "#edit"; _autofocus ] [ rawText "Add Team" ]

    let all (teams : Team list) = [
        table [] [
            thead [] [
                tr [] [
                    th [] [ rawText "Team Name" ]
                    th [] [ rawText "Coach Name" ]
                    th [] [ rawText "Coach Email" ] ] ]
            tbody [ _id "teams"; _icAppendFrom Url.Get.latest ] (teams |> List.map teamRow) ]
        div [ _id "edit" ] [ addTeamButton ] ]

