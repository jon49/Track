open Microsoft.FSharp.Quotations.Patterns
open System
open Microsoft.FSharp.Reflection
open Microsoft.FSharp.Quotations
module P = Microsoft.FSharp.Quotations.Patterns

// http://www.fssnip.net/h1
let rec eval = function
    | Value(v,t) -> v
    | Coerce(e,t) -> eval e
    | NewObject(ci,args) -> ci.Invoke(evalAll args)
    | NewArray(t,args) -> 
        let array = Array.CreateInstance(t, args.Length) 
        args |> List.iteri (fun i arg -> array.SetValue(eval arg, i))
        box array
    | NewUnionCase(case,args) -> FSharpValue.MakeUnion(case, evalAll args)
    | NewRecord(t,args) -> FSharpValue.MakeRecord(t, evalAll args)
    | NewTuple(args) ->
        let t = FSharpType.MakeTupleType [|for arg in args -> arg.Type|]
        FSharpValue.MakeTuple(evalAll args, t)
    | FieldGet(Some(Value(v,_)),fi) -> fi.GetValue(v)
    | PropertyGet(None, pi, args) -> pi.GetValue(null, evalAll args)
    | PropertyGet(Some(x),pi,args) -> pi.GetValue(eval x, evalAll args)
    | Call(None,mi,args) -> mi.Invoke(null, evalAll args)
    | Call(Some(x),mi,args) -> mi.Invoke(eval x, evalAll args)
    | arg -> raise <| NotSupportedException(arg.ToString())
and evalAll args = [|for arg in args -> eval arg|]

let getValue (expr: Quotations.Expr<'t>) =
    match eval expr with
    | null -> None
    | x -> Some (x :?> 't)

type Validate<'a> =
    | Object of
        value : Expr<'a> *
        required : bool *
        proof : ('a -> (string list option) list)
    | Array of
        value : Expr<'a[]> *
        required : bool *
        proof : ('a[] -> string Option) list *
        proveItems : ('a -> Validate<'a>)
    | Primitive of
        value : Expr<'a> *
        required : bool *
        proof : ('a -> string Option) list
    | Raw of
        value : 'a *
        message : string *
        proof : ('a -> string Option) list


let getParameterName a =
    match a with
    | PropertyGet(e, info, li) ->
        let d = info.DeclaringType.ToString()
        Some (sprintf "%s.%s" (d.Substring(d.IndexOf('+')+1)) info.Name)
    | _ -> Some a.Type.Name

let getOrElse def =
    function
    | Some x -> x
    | None -> def

let printParameterWith s expression =
    Option.fold (fun _ v -> v + s) "" (getParameterName expression)

let private prettyIndex i xs =
    xs
    |> List.head
    |> sprintf "  [%i]: %s" i

let rec prove validation =
    match validation with
    | Primitive (v, required, fs) ->
        match required, getValue v with
        | true, None -> Some [sprintf "The value `%s` is required but was found to be `null`." v.Type.Name]
        | false, None -> None
        | _, Some value ->
            fs
            |> List.fold (fun acc f ->
                match acc with
                | None -> f value
                | Some _ -> acc
                ) None
            |> Option.map (fun x -> [printParameterWith " - " v + x] )
    | Object (v, required, f) ->
        match required, getValue v with
        | true, None -> Some [sprintf "The object `%s` is required but was found to be `null`." (getOrElse "Unknown Parameter" <| getParameterName v)]
        | false, None -> None
        | _, Some x ->
            f x
            |> List.fold (
                fun acc option ->
                match option, acc with
                | Some newError, Some error -> Some (List.append error newError)
                | Some newError, None -> Some newError
                | None, Some error -> Some error
                | None, None -> None
                ) None
    | Array (vs, required, proof, proveItems) -> 
        match required, getValue vs with
        | true, None -> Some [sprintf "%s: %s" (getOrElse "Unknown Parameter" <| getParameterName vs) "This array is required."]
        | false, None -> None
        | _, Some xs ->
            let validSelf =
                proof
                |> List.fold (fun acc f ->
                    match acc with
                    | None -> f xs
                    | Some _ -> acc
                ) None
            match validSelf, obj.Equals(vs, null) with
            | Some error, _ -> 
                Some [(printParameterWith ": " vs) + error]
            | _, true ->
                None
            | None, false ->
                xs
                |> Array.fold (fun (i, acc) x ->
                    let i = i + 1
                    match (prove (proveItems x) ), acc with
                    | Some newError, Some error -> 
                        (i, Some <| List.append error [prettyIndex i newError])
                    | Some newError, None -> (i, Some [prettyIndex i newError])
                    | None, Some error -> (i, Some error)
                    | None, None -> (i, None)
                ) (-1, None)
                |> snd
                |> Option.map (fun xs -> (printParameterWith ":" vs)::xs)
    | Raw (a, msg, fs) ->
        fs
        |> List.fold (fun acc f ->
            match acc with
            | None -> f a
            | Some _ -> acc
            ) None
        |> Option.map (fun x -> [msg+" - "+x] )

let stringMax value (x : string) =
    if x.Length <= value
    then None
    else Some <| sprintf "String must be less than %i characters, but was %i" value x.Length

let maxDate (max : DateTime) x =
    if x <= max then
        None
    else
        Some (sprintf "Date must be less than or equal to %s, but given %s" (max.ToShortDateString()) (x.ToShortDateString()))

let arrayMinLength (min : int) xs =
    let len = Array.length xs
    if min <= len then
        None
    else
        Some (sprintf "Array must have at least %i items, but only has %i items." min len)

//type IValidation =
//    abstract member Validate: unit -> string list option

type Name = {
    First : string
    Last : string
    } with
    static member Proof a =
        [
        prove <| Primitive (<@ a.First @>, true, [stringMax 50])
        prove <| Primitive (<@ a.Last @>, true, [stringMax 5])
        ]

type Person = {
    Name : Name
    BirthDate : DateTime
    Favorites : string[]
    } with
    static member Proof a =
        [
        prove <| Primitive (<@ a.BirthDate @>, true, [])
        prove <| Object (<@ a.Name @>, true, Name.Proof)
        prove <|
            Array (
                <@ a.Favorites @>,
                true,
                [arrayMinLength 1],
                (fun favorite -> Primitive (<@ favorite @>, true, [stringMax 5]) ))
        ]
    static member Validate a =
        prove <| Object (<@ a @>, true, Person.Proof)

let jon = {
    Name = { First = "Jon"; Last = "Nyman1" }
    BirthDate = new DateTime(1947, 9, 9)
    Favorites = [| "Reading"; "Red";  "Writing" |]
    }

let result =
    prove <| Object (<@ jon.Name @>, true, Name.Proof)
let result2 =
    prove <| Object (<@ jon @>, true, Person.Proof)
let result3 =
    prove <| Object (<@ Microsoft.FSharp.Core.Operators.Unchecked.defaultof<Person> @>, true, Person.Proof)

let result4 =
    prove <| Primitive (<@ jon.Favorites @>, true, [fun a -> if a.Length > 0 then None else Some <| "Array must contain at least one item."])

type DosArrays = {
    Array1 : string[]
    Array2 : int[]
}

let dos = { Array1 = [| "yep" |]; Array2 = [| 1; 2 |] }

let result5 =
    prove <| Primitive (<@ (dos.Array1, dos.Array2) @>, true, [fun (a, b) -> if a.Length = b.Length then None else Some <| "Arrays must be same length!" ])

let result6 =
    prove <| Raw ((dos.Array1, dos.Array2), "DosArrays - Array1, Array2",
                  [
                  fun (a, b) ->
                    match a, b with
                    | null, null | null, _ | _, null -> Some "Items must not be null."
                    | _ -> None
                  fun (a, b) ->
                     if a.Length = b.Length then None else Some "Arrays must be same length!" 
                  ])

// Can't do it this way since `a` could be null.
//let validate (a : IValidation) =
//    match a.Validate () with
//    | Some xs -> Choice1Of2 <| String.concat "\n" xs
//    | None -> Choice2Of2 a

let validate f a =
    match f a with
    | Some xs -> Choice1Of2 <| String.concat "\n" xs
    | None -> Choice2Of2 a

jon
|> validate Person.Validate
// |> ....

// OR
jon
|> validate (fun a -> prove <| Object (<@ a @>, true, Person.Proof))