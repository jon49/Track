
let (>=>) switch1 switch2 x = 
    match switch1 x with
    | Ok s -> switch2 s
    | Error f -> Error f 

let map f xResult =
    match xResult with
    | Ok x ->
        Ok (f x)
    | Error errs ->
        Error errs

let lift x =
    Ok x

let apply fResult xResult =
    match fResult,xResult with
    | Ok f, Ok x ->
        Ok (f x)
    | Error errs, Ok x ->
        Error errs
    | Ok f, Error errs ->
        Error errs
    | Error errs1, Error errs2 ->
        Error (List.concat [errs1; errs2])

let bind f xResult =
    match xResult with
    | Ok x ->
        f x
    | Error errs ->
        Error errs

type CustomerId = CustomerId of int
type EmailAddress = EmailAddress of string

let createCustomerId id =
    if id > 0 then
        Ok (CustomerId id)
    else
        Error ["CustomerId must be positive"]
// int -> Result<CustomerId>

let createEmailAddress str =
    if System.String.IsNullOrEmpty(str) then
        Error ["Email must not be empty"]
    elif str.Contains("@") then
        Ok (EmailAddress str)
    else
        Error ["Email must contain @-sign"]

type CustomerInfo = {
    id: CustomerId
    //name: string  // New!
    email: EmailAddress
    }

let createCustomer customerId email =
    { id=customerId;  email=email }

let (<!>) = map
let (<*>) = apply

let createCustomerResultA id email =
    let idResult = createCustomerId id
    let emailResult = createEmailAddress email
    createCustomer <!> idResult <*> emailResult

