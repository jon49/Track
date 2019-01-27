﻿namespace Teams

module View =

    open Giraffe.GiraffeViewEngine
    open Model
    open Utils.ViewEngine
    open Utils.UI

    let team (team : Team) =
        let teamId = sprintf "team-id-%i" team.TeamId
        let edit id = button [ _icTarget ("#" + id) ] [ rawText "Edit" ]
        [
        div [ _id teamId ] [
            a [ _href "#" ] [ str team.Data.TeamName ]
            edit teamId
        ]
        div [] [
            a [ _id (sprintf "user-%i" team.UserId); _href "#" ] [
                span [] [ str (sprintf "%s %s" team.Data.FirstName team.Data.LastName) ]
                span [] [ str team.Data.Email ]
            ]
        ]
        button [ _icTarget "#edit"; _icGetFrom (Url.View.edit team.TeamId team.UserId); _title "Edit" ] [
                rawText "Edit" ] ]

    let teamRow (x : Team) =
        div [ _icSrc (Url.show x.TeamId x.UserId); _icDeps "ignore" ] (team x)

    let addEditTeamForm (maybeTeam : Team option) =
        let team, target =
            match maybeTeam with
            | Some x -> x.Data, Url.show x.TeamId x.UserId
            | None -> { FirstName = ""
                        LastName = ""
                        Email = ""
                        TeamName = "" }, ""
        let postTo =
            match maybeTeam with
            | Some x -> Url.show x.TeamId x.UserId
            | None -> Url.index
        form [ _method "post"; _icPostTo postTo ] [
            fieldset [ _class "show" ] [
                fieldset [] [ inputText Data.TeamNameInfo team [ _autofocus ] ]
                fieldset [ _class "show" ] [
                    legend [] [ rawText "Coach" ]
                    inputText Data.FirstNameInfo team []
                    inputText Data.LastNameInfo team []
                    inputEmail Data.EmailInfo team [] ]
                button [ _type "submit"; _icActionTarget target ] [ rawText "Submit" ]
                button [ _icGetFrom Url.View.addTeamButton; _icTarget "#edit" ] [ rawText "Cancel" ]
            ]
        ]

    let addTeamButton =
        button [ _icGetFrom Url.View.addTeamForm; _icTarget "#edit"; _autofocus ] [ rawText "Add Team" ]

    let all (teams : Team list) = [
        header [] [
            strong [] [ rawText "Team Name" ]
            strong [] [ rawText "Coach Name" ]
            strong [] [ rawText "Coach Email" ]
        ]
        div [ _id "teams"; _icAppendFrom Url.latest ] (teams |> List.map teamRow)
        div [ _id "edit" ] [ addTeamButton ] ]

