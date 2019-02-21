open System.ComponentModel.DataAnnotations
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

type Person = {
    //[<Display(Name = "First Name"); Required; MinLength(1); MaxLength(128)>]
    Name : string
    }

let rec getPropertyInfo quotation =
    match quotation with
    | PropertyGet (_,propertyInfo,_) -> Some propertyInfo
    | Lambda (_,expr) -> getPropertyInfo expr
    | _ -> None

let pi = getPropertyInfo <@ fun x -> x.Name @> |> Option.get |> fun x -> x.Name

printfn "%s" (pi)

