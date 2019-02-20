namespace Track.Pages

module Welcome =
    open Giraffe.GiraffeViewEngine
    open App
    open Track.User

    let View userType =
        let getStarted =
            match userType with
            | Authenticated ->
                [
                rawText "
Looks like you haven't been registered yet. If you are a coach please ask for an invitation from
your region coordinator. If you coordinate teams and there is already a coordinator in your region
then please ask for an invitation from the main coordinator. If you would like to start a new
region "
                a [ _href "/regions" ] [ rawText "please click here" ]
                rawText "."
                ]
            | Registered -> [ rawText "Looks like you ended up here by mistake!" ]
            | UnknownUser ->
                [
                rawText "Track is an app to organize your sporting events around track and field events. "
                a [ _href "/login" ] [ rawText "Click here to login and get started!" ]
                ]

        [
        h2 [] [
            rawText "Welcome to "
            a [ _href "/" ] [ rawText "track.JNyman.com" ]
            rawText "!"
            ]
        p [] getStarted
        ]
