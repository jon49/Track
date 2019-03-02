namespace Track

type Errors =
    | DBError of string
    | ValidationError of string
    | AuthorizationError
    | NotAuthenticated
    | NotFound
    | InternalServerError
