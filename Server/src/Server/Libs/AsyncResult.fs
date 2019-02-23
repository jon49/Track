namespace Utils

module Task = 
    open FSharp.Control.Tasks
    open System.Threading.Tasks

    //let map f (xTask : Task<'a>) =
    //    task {
    //        // get the contents of xTask
    //        let! x = xTask 
    //        // apply the function and lift the result
    //        return f x
    //    }

    //let retn x = task {
    //    // lift x to an Task
    //    return x
    //    }

    //let apply fTask xTask = task {

    //    let fChild = fTask ()
    //    let xChild = xTask ()
    //    let! f = fChild
    //    let! x = xChild

    //    // apply the function to the results
    //    return f x 
    //    }

    let bind f (xTask : Task<Result<'a, 'b>>) = task {
        let! x = xTask 
        match x with
        | Error x -> return Error x
        | Ok x -> return! f x
        }
