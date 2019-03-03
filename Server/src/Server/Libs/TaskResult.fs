namespace Utils

// https://fsharpforfunandprofit.com/posts/elevated-world-5/
open System.Threading.Tasks
open FSharp.Control.Tasks

type TaskResult<'a, 'b> = Task<Result<'a, 'b>>

module Result =

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

module Task = 
    open FSharp.Control.Tasks
    open System.Threading.Tasks
    open System.IO
    open System.Threading
    open System.Threading.Tasks

    type T () =
        static member Parallel(a : unit -> Task<'a>, b : unit -> Task<'b>) =
            task {
            let a' = Task.Run<'a>(a)
            let b' = Task.Run<'b>(b)
            let! a'' = a'
            let! b'' = b'
            return a'', b''
            }

    let apply (fTask : Task<'a -> 'b>) (xTask : Task<'a>) =
        task {
        let fChild = fTask
        let xChild = xTask
        let! f = fChild
        let! x = xChild
        return f x 
        }

    let map (f : 'a -> 'b) (xTask : Task<'a>) =
        task {
        let! x = xTask 
        return f x
        }

    let lift x =
        task { return x }

    let bind (f : 'a -> Task<'b>) (xTask : Task<'a>) : Task<'b> =
        task {
        let! x = xTask 
        return! f x
        }

module TaskResult =
    open FSharp.Control.Tasks
    open Giraffe

    type T () =
        static member Parallel(a, b) =
            task {
            let! a', b' = Task.T.Parallel(a, b)
            match a', b' with
            | Error errorA, Error errorB -> return Error <| errorA @ errorB
            | Error error, _ | _, Error error -> return Error error
            | Ok a, Ok b -> return Ok (a, b)
            }

    let map (f : 'a -> 'b) (result : TaskResult<'a, 'c>) : TaskResult<'b, 'c> = 
        task {
            let! result1 = result
            return (Result.map f result1)
        }

    let lift x : TaskResult<_, _> = 
        x |> Result.lift |> Task.lift

    // TaskResult<('a -> 'b)> -> TaskResult<'a, 'c> -> TaskResult<'b, 'c>
    let apply (fTaskResult : TaskResult<'a -> 'b, 'c list>) (xTaskResult : TaskResult<'a, 'c list>) : TaskResult<'b, 'c list> = 
        fTaskResult
        |> Task.bind (fun fResult ->
            xTaskResult
            |> Task.map (fun xResult -> 
                Result.apply fResult xResult ) )

    let bind (f : 'a -> TaskResult<'b, 'c>) (xTaskResult : TaskResult<'a, 'c>) : TaskResult<'b, 'c> =
        task {
        let! xResult = xTaskResult 
        match xResult with
        | Ok x -> return! f x
        | Error err -> return Error err
        }

    [<Sealed>]
    type TaskResultBuilder () =

        member __.Bind (m : TaskResult<_, _>, f : _ -> TaskResult<_, _>) = bind f m
        member __.Return (x) = lift x
        member __.ReturnFrom (x) = x
        member __.Zero () : TaskResult<unit, 'Error> =
            __.Return ()
        member __.Delay (generator : unit -> TaskResult<'T, 'Error>) =
            generator
        member __.Run (f) = f()
        member __.Combine (r1 : TaskResult<_,_>, r2 : TaskResult<_,_>) : TaskResult<'T, 'Error> =
            task {
            let! r1' = r1
            match r1' with
            | Error error ->
                return Error error
            | Ok () ->
            return! r2
            }

    let taskResult = new TaskResultBuilder()

    module Symbols =

        let (>>=) x f = bind f x
        let (<*>) = apply
        let (<!>) = map

// https://github.com/jack-pappas/ExtCore/blob/master/ExtCore/Control.fs#L1478

    // 'T -> M<'T>
//    member (*inline*) __.Return value : TaskResult<'T, 'Error> =
//        task { return Ok value }

//    // M<'T> -> M<'T>
//    member (*inline*) __.ReturnFrom (taskResult : TaskResult<'T, 'Error>) =
//        taskResult

//    // unit -> M<'T>
//    member inline this.Zero () : TaskResult<unit, 'Error> =
//        this.Return ()

//    // (unit -> M<'T>) -> M<'T>
//    //member inline this.Delay (generator : unit -> TaskResult<'T, 'Error>) : TaskResult<'T, 'Error> =
//    //    Task.
//    //    Task.Delay generator

//    // M<'T> -> M<'T> -> M<'T>
//    // or
//    // M<unit> -> M<'T> -> M<'T>
//    member (*inline*) __.Combine (r1 : TaskResult<'a, 'Error>, r2 : TaskResult<'b, 'Error>) : TaskResult<'b, 'Error> =
//        task {
//        let! r1' = r1
//        match r1' with
//        | Error error ->
//            return Error error
//        | Ok _ ->
//            return! r2
//        }

//    // M<'T> * ('T -> M<'U>) -> M<'U>
//    member (*inline*) __.Bind (value : TaskResult<'a, 'Error>, binder : 'a -> TaskResult<'b, 'Error>) : TaskResult<'b, 'Error> =
//        task {
//        let! value' = value
//        match value' with
//        | Error error ->
//            return Error error
//        | Ok x ->
//            return! binder x
//        }

//    // M<'T> * (exn -> M<'T>) -> M<'T>
//    member inline __.TryWith (computation : TaskResult<'T, 'Error>, catchHandler : exn -> TaskResult<'T, 'Error>) : TaskResult<'T, 'Error> =
//        task {
//        try
//            return! computation
//        with
//        | ex ->
//            return! (catchHandler ex)
//        }

//    // M<'T> * (unit -> unit) -> M<'T>
//    member inline __.TryFinally (computation : TaskResult<'T, 'Error>, compensation : unit -> unit) : TaskResult<'T, 'Error> =
//        task {
//        try
//            return! computation
//        finally
//            compensation ()
//        }

//    // 'T * ('T -> M<'U>) -> M<'U> when 'T :> IDisposable
//    member inline __.Using (resource : ('T :> System.IDisposable), binder : _ -> TaskResult<'U, 'Error>) : TaskResult<'U, 'Error> =
//        task {
//        use r = resource
//        }
//        //task.Using (resource, binder)

//    // (unit -> bool) * M<'T> -> M<'T>
//    member this.While (guard, body : TaskResult<unit, 'Error>) : TaskResult<_,_> =
//        if guard () then
//            // OPTIMIZE : This could be simplified so we don't need to make calls to Bind and While.
//            this.Bind (body, (fun () -> this.While (guard, body)))
//        else
//            this.Zero ()

//    // seq<'T> * ('T -> M<'U>) -> M<'U>
//    // or
//    // seq<'T> * ('T -> M<'U>) -> seq<M<'U>
//    member this.For (sequence : seq<_>, body : 'T -> TaskResult<unit, 'Error>) =
//        // OPTIMIZE : This could be simplified so we don't need to make calls to Using, While, Delay.
//        this.Using (sequence.GetEnumerator (), fun enum ->
//            this.While (
//                enum.MoveNext,
//                this.Delay (fun () -> body enum.Current)))

//[<Sealed>]
//type AsyncChoiceBuilder () =
//    // 'T -> M<'T>
//    member (*inline*) __.Return value : Async<Choice<'T, 'Error>> =
//        Choice1Of2 value
//        |> async.Return

//    // M<'T> -> M<'T>
//    member (*inline*) __.ReturnFrom (asyncChoice : Async<Choice<'T, 'Error>>) =
//        asyncChoice

//    // unit -> M<'T>
//    member inline this.Zero () : Async<Choice<unit, 'Error>> =
//        this.Return ()

//    // (unit -> M<'T>) -> M<'T>
//    member inline this.Delay (generator : unit -> Async<Choice<'T, 'Error>>) : Async<Choice<'T, 'Error>> =
//        async.Delay generator

//    // M<'T> -> M<'T> -> M<'T>
//    // or
//    // M<unit> -> M<'T> -> M<'T>
//    member (*inline*) __.Combine (r1, r2) : Async<Choice<'T, 'Error>> =
//        async {
//        let! r1' = r1
//        match r1' with
//        | Choice2Of2 error ->
//            return Choice2Of2 error
//        | Choice1Of2 () ->
//            return! r2
//        }

//    // M<'T> * ('T -> M<'U>) -> M<'U>
//    member (*inline*) __.Bind (value : Async<Choice<'T, 'Error>>, binder : 'T -> Async<Choice<'U, 'Error>>)
//        : Async<Choice<'U, 'Error>> =
//        async {
//        let! value' = value
//        match value' with
//        | Choice2Of2 error ->
//            return Choice2Of2 error
//        | Choice1Of2 x ->
//            return! binder x
//        }

//    // M<'T> * (exn -> M<'T>) -> M<'T>
//    member inline __.TryWith (computation : Async<Choice<'T, 'Error>>, catchHandler : exn -> Async<Choice<'T, 'Error>>)
//        : Async<Choice<'T, 'Error>> =
//        async.TryWith(computation, catchHandler)

//    // M<'T> * (unit -> unit) -> M<'T>
//    member inline __.TryFinally (computation : Async<Choice<'T, 'Error>>, compensation : unit -> unit)
//        : Async<Choice<'T, 'Error>> =
//        async.TryFinally (computation, compensation)

//    // 'T * ('T -> M<'U>) -> M<'U> when 'T :> IDisposable
//    member inline __.Using (resource : ('T :> System.IDisposable), binder : _ -> Async<Choice<'U, 'Error>>)
//        : Async<Choice<'U, 'Error>> =
//        async.Using (resource, binder)

//    // (unit -> bool) * M<'T> -> M<'T>
//    member this.While (guard, body : Async<Choice<unit, 'Error>>) : Async<Choice<_,_>> =
//        if guard () then
//            // OPTIMIZE : This could be simplified so we don't need to make calls to Bind and While.
//            this.Bind (body, (fun () -> this.While (guard, body)))
//        else
//            this.Zero ()

//    // seq<'T> * ('T -> M<'U>) -> M<'U>
//    // or
//    // seq<'T> * ('T -> M<'U>) -> seq<M<'U>>
//    member this.For (sequence : seq<_>, body : 'T -> Async<Choice<unit, 'Error>>) =
//        // OPTIMIZE : This could be simplified so we don't need to make calls to Using, While, Delay.
//        this.Using (sequence.GetEnumerator (), fun enum ->
//            this.While (
//                enum.MoveNext,
//                this.Delay (fun () -> body enum.Current)))

