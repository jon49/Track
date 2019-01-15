namespace Track

module Settings =

    open FSharp.Data

    type Settings = JsonProvider<"""./Settings/Settings.json""">

    let Setting = Settings.GetSample ()
