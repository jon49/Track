namespace Utils

module Reflection =

    open Microsoft.FSharp.Quotations
    open Microsoft.FSharp.Quotations.Patterns
    open System
    open System.Reflection
    open System.ComponentModel
    open Microsoft.FSharp.Reflection

    let isOption (a : obj) = 
        let aType = a.GetType()
        aType.IsGenericType && aType.GetGenericTypeDefinition() = typedefof<Option<_>>

    let SomeObj (a : obj) =
        let optionType = typedefof<option<_>>
        let aType = a.GetType()
        let v = aType.GetProperty("Value")
        if aType.IsGenericType && aType.GetGenericTypeDefinition() = optionType
            then
                if a = null
                    then true, None
                else true, Some(v.GetValue(a, [| |]))
        else false, if a = null then None else Some a

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
    and private evalAll args = [|for arg in args -> eval arg|]

    let getPropertyInfo (expr : Expr<'a>) =
        match expr with
        | PropertyGet(_, propInfo, _) -> Some propInfo
        | _ -> None

    let getPropertyValue (expr: Expr<'a>) =
        match eval expr with
        | null -> None
        | x -> Some (x :?> 't)

    let getValue (propertyName : string) (obj : 'a) =
        obj.GetType().GetProperty(propertyName).GetValue(obj)

    let getPropertyName expr =
        getPropertyInfo expr
        |> Option.map (fun x -> x.Name)

    let getDisplayName (expr: Expr<'a>) =
        let names =
            getPropertyInfo expr
            |> Option.map (fun x ->
                let displayName =
                    if x.IsDefined(typeof<DisplayNameAttribute>)
                        then Some <| x.GetCustomAttribute<DisplayNameAttribute>().DisplayName
                    else None
                displayName, x.Name )
        match names with
        | Some (Some null, x)
        | Some (Some "", x)
        | Some (None, x) -> x
        | Some (Some x, _) -> x
        | None -> ""

    // http://www.contactandcoil.com/software/dotnet/getting-a-property-name-as-a-string-in-f/
    let rec propertyName quotation =
        match quotation with
        | PropertyGet (_,propertyInfo,_) -> propertyInfo.Name
        | Lambda (_,expr) -> propertyName expr
        | _ -> failwith "Unexpected quotation format."
