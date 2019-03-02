open System
let strToInt s =
    match Int32.TryParse(s) with
    | true, x -> Some x
    | false, _ -> None

type MaybeBuilder() =
    member this.Bind(m, f) =
        match m with
        | Some m -> f m
        | None -> None
        //Option.bind f m
    member this.Return(x) = Some x
    member this.ReturnFrom(x) = x

let maybe = MaybeBuilder()

let t =
    maybe {
    return 2
    }

let t2 =
    maybe {
    return! Some 3
    }

let stringAddWorkflow x y z =
    maybe {
    let! a = strToInt x
    let! b = strToInt y
    let! c = strToInt z
    return a + b + c
    }

let good = stringAddWorkflow "12" "3" "2"
let bad = stringAddWorkflow "12" "xyz" "2"

let strAdd str i =
    match strToInt str with
    | Some x -> Some <| i + x
    | None -> None

let (>>=) m f =
    match m with
    | Some x -> f x
    | None -> None

let good2 = strToInt "1" >>= strAdd "2" >>= strAdd "3"
let bad2 = strToInt "1" >>= strAdd "xyz" >>= strAdd "3"
