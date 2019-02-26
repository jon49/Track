namespace Utils

// https://fsharpforfunandprofit.com/posts/elevated-world-5/
open System.Threading.Tasks

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

    //let flatten (result : TaskResult<TaskResult<'a, 'c>, 'c>) : TaskResult<'a, 'c> =

    module Symbols =

        let (>>=) x f = bind f x
        let (<*>) = apply
        let (<!>) = map
