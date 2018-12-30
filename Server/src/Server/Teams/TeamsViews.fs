namespace Teams

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
        tr [ _icSrc (Url.Put.make x.TeamId x.UserId) ] (team x)

    let addTeamForm (maybeTeam : Team option) =
        let (postTo, firstName, lastName, email, teamName) =
            match maybeTeam with
            | Some x ->
                let d = x.Data
                (Url.Put.make x.TeamId x.UserId, d.FirstName, d.LastName, d.Email, d.TeamName)
            | None ->
                (Url.Post.``new``, "", "", "", "")
        form [ _method "post"; _icPostTo postTo ] [
            fieldset [] [
                fieldset [] [
                    label [] [ rawText "Team" ]
                    input [ _type "text"; _value teamName; _autofocus ] ]
                fieldset [] [
                    legend [] [ rawText "Coach" ]
                    label [] [ rawText "First Name" ]
                    input [ _type "text"; _value firstName ]
                    label [] [ rawText "Last Name" ]
                    input [ _type "text"; _value lastName ]
                    br []
                    label [] [ rawText "Email" ]
                    input [ _type "email"; _value email ] ]
                button [ _type "submit" ] [ rawText "Submit" ]
                button [ _icGetFrom "/teams/cancel"; _icTarget "#edit" ] [ rawText "Cancel" ]
            ]
        ]

    let addTeamButton =
        button [ _icGetFrom Url.Partial.addTeamButton; _icTarget "#edit" ] [ rawText "Add Team" ]

    let all (teams : Team list) = [
        table [] [
            thead [] [
                tr [] [
                    th [] [ rawText "Team Name" ]
                    th [] [ rawText "Coach Name" ]
                    th [] [ rawText "Coach Email" ] ] ]
            tbody [ _id "teams"; _icAppendFrom Url.Get.latest ] (teams |> List.map teamRow) ]
        div [ _id "edit" ] [ addTeamButton ] ]

