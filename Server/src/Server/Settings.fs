namespace Track

module Settings =

    open FSharp.Data

    type Settings = JsonProvider<"""./Settings.json""">

    let Setting = Settings.GetSample ()

