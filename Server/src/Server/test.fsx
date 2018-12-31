module Print
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns
open System
open Microsoft.FSharp.Reflection

let rec eval = function
    //| Value(v,t) -> v
    //| Coerce(e,t) -> eval e
    //| NewObject(ci,args) -> ci.Invoke(evalAll args)
    //| NewArray(t,args) -> 
    //    let array = Array.CreateInstance(t, args.Length) 
    //    args |> List.iteri (fun i arg -> array.SetValue(eval arg, i))
    //    box array
    //| NewUnionCase(case,args) -> FSharpValue.MakeUnion(case, evalAll args)
    //| NewRecord(t,args) -> FSharpValue.MakeRecord(t, evalAll args)
    //| NewTuple(args) ->
    //    let t = FSharpType.MakeTupleType [|for arg in args -> arg.Type|]
    //    FSharpValue.MakeTuple(evalAll args, t)
    //| FieldGet(Some(Value(v,_)),fi) -> fi.GetValue(v)
    | PropertyGet(None, pi, args) -> pi.GetValue(null, evalAll args)
    | PropertyGet(Some(x),pi,args) -> pi.GetValue(eval x, evalAll args)
    //| Call(None,mi,args) -> mi.Invoke(null, evalAll args)
    //| Call(Some(x),mi,args) -> mi.Invoke(eval x, evalAll args)
    | arg -> raise <| NotSupportedException(arg.ToString())
and evalAll args = [|for arg in args -> eval arg|]

let getValue (expr: Quotations.Expr<'t>) =
    match eval expr with
    | null -> None
    | x -> Some (x :?> 't)

type Test =
    { FirstName : string
      Age : int }

let test = { FirstName = "Jon"; Age = 22 }

let field (field : Expr<'a> ) =
    let propertyName =
        match field with
        | PropertyGet(_, propInfo, _) ->
            Some (propInfo.Name)
        | _ -> None

    let propertyValue =
        getValue field

    propertyName, propertyValue

printfn "%A" (field <@ test.Age @>)
