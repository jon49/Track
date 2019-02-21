namespace Track.Regions

module Model =
    open System.ComponentModel.DataAnnotations

    [<Struct>]
    type Region = {
        [<MaxLength(256)>]
        Name : string
    } with
        static member Init = { Name = "" }

    [<Struct>]
    type RegionEdit = {
        Region : Region
        Id : int
    }

module Url =

    let index = "/regions"
    let show = sprintf "/regions/%i"

module Views =
    open Giraffe.GiraffeViewEngine
    open Utils.ViewEngine
    open Utils
    open Model

    let index (regions : RegionEdit list) =
        let regionForms =
            match List.isEmpty regions with
            | true -> []
            | false ->
                regions
                |> List.map (fun x ->
                    form [ _method "post"; _icPostTo (Url.show x.Id) ] [
                        yield! UI.inputText x <@ fun x -> x.Region.Name @> []
                    ]
                )
        [
        h2 [] [ rawText "Regions" ]
        //p [] [ rawText "Please enter the name of your new region." ]
        //form [ _method "post"; _icPostTo Url.index ]
        //    [
        //    UI.inputText <@  @>
        //    ]
        ]
