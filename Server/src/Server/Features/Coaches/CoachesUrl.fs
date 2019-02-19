namespace Coaches

module Url =

    let show = sprintf "/coaches/%i"
    let index = "/coaches"
    let latest = "/coaches/latest"

    module View =

        let edit = sprintf "/coaches/%i/edit"
        let addButton = "/coaches/add-button"
