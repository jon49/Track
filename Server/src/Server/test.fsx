open System.ComponentModel.DataAnnotations

type A =
| B = 0
| C = 1

let b = A.B

type D = {
    A : A
}

let c = fun x -> x.A

type Person = {
    //[<Display(Name = "First Name"); Required; MinLength(1); MaxLength(128)>]
    Name : string
    }
