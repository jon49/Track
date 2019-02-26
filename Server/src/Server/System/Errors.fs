namespace Track

type Errors =
    | DBError of string
    | ValidationError of string
    | AuthorizationError
