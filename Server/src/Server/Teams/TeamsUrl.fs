namespace Teams

module Url =

    module Put =
        let make =
            sprintf "/teams/%i/users/%i"

    module Get =
        let all = "/teams"
        let latest = "/teams/latest"

    module Partial =
        let userForm teamId userId =
            sprintf "/teams/%i/users/%i/form" teamId userId

        let addTeamButton =
            "/teams/add"

    module Post =
        let ``new`` = "/teams";
