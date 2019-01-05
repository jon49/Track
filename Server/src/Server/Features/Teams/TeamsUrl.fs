namespace Teams

module Url =

    module Put =
        let team =
            sprintf "/teams/%i/users/%i"

    module Get =
        let team = sprintf "/teams/%i/users/%i"
        let all = "/teams"
        let latest = "/teams/latest"

    module Partial =

        let userForm teamId userId =
            sprintf "/teams/%i/users/%i/edit" teamId userId

        let addTeamButton = "/teams/add-button"

        let addTeamForm = "/teams/add"

    module Post =
        let ``new`` = "/teams";
