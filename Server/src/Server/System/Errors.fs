namespace Track

type Errors =
    | DBError of string list
    | ValidationError of string list

