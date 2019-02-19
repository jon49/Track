namespace Track.Pages

module Welcome =
    open Giraffe.GiraffeViewEngine

    let View =

        [
        h2 [] [
            rawText "Welcome to "
            a [ _href "/" ] [ rawText "Track.JNyman.com" ]
            rawText "!"
            ]
        p [] [
            rawText "Track is an app to organize your sporting events around track and field events. "
            a [ _href "/login" ] [ rawText "Click here to login and get started!" ]
            ]
        ]
