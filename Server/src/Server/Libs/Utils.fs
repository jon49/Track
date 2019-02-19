namespace Utils

module String =

    let trim (s : string) = s.Trim()

module Fun =

    let toFunc f =
        System.Func<_,_>(f)
