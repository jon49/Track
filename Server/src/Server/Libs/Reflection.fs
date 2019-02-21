namespace Utils

module Reflection =

    open Microsoft.FSharp.Quotations
    open Microsoft.FSharp.Quotations.Patterns
    open System
    open System.Reflection
    open System.ComponentModel
    open Microsoft.FSharp.Reflection
    open System.ComponentModel.DataAnnotations
    open System.Linq

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

    // http://www.contactandcoil.com/software/dotnet/getting-a-property-name-as-a-string-in-f/
    let rec getPropertyInfo quotation =
        match quotation with
        | PropertyGet (_,propertyInfo,_) -> propertyInfo
        | Lambda (_,expr) -> getPropertyInfo expr
        | _ -> failwith "Expected some property information for quotation."

    let getValue (propertyInfo : PropertyInfo) (obj : 'a) =
        obj.GetType().GetProperty(propertyInfo.Name).GetValue(obj)

    let getDisplayName (propertyInfo: PropertyInfo) =
        let names =
            let displayName =
                if propertyInfo.IsDefined(typeof<DisplayAttribute>)
                    then propertyInfo.GetCustomAttribute<DisplayAttribute>().Name |> Option.ofObj
                else None
            displayName, propertyInfo.Name
        match names with
        | Some "", x -> x
        | Some x, _ -> x
        | None, x -> x

    let getCustomAttributes<'a> (pi : PropertyInfo) =
        pi.GetCustomAttributes(typeof<'a>, false).OfType<'a>()
        |> Seq.toArray

    let getCustomAttribute<'a when 'a :> Attribute> (expr : Expr<obj>) =
        (getPropertyInfo expr).GetCustomAttribute<'a>()
