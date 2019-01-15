namespace Teams

module Url =

    let show = sprintf "/teams/%i/users/%i"
    let index = "/teams"
    let latest = "/teams/latest"

    module View =

        let edit = sprintf "/teams/%i/users/%i/edit"
        let addTeamButton = "/teams/add-button"
        let addTeamForm = "/teams/add"
