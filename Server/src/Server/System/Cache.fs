namespace Track

module Cache =

    open Microsoft.Extensions.Caching.Memory

    let private Memory =
        let options = new MemoryCacheOptions()
        new MemoryCache(options)

    let tryOrGetTask getter (key : string) =
        Memory.GetOrCreateAsync(key, Utils.Fun.toFunc getter)

    let tryOrGetAsync getter key =
        tryOrGetTask (getter >> Async.StartAsTask) key

    let remove (key : string) =
        Memory.Remove(key)
