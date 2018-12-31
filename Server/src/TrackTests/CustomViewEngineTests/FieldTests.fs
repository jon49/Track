namespace TrackTests.CustomViewEngineTests

module FieldTests =

    open Expecto
    open TrackTests.ViewModel
    open Track.ViewEngine
    open Giraffe.GiraffeViewEngine

    [<Tests>]
    let tests =

        testList "Field Tests" [
            testCase "Should return correct rendering" <| fun _ ->
                // Act
                let value =
                    field "text" [ _autofocus ] <@ person.FirstName @>
                    |> renderHtmlNodes

                // Assert
                let expected = """<label for="FirstName">First Name</label><input type="text" value="Jon" name="FirstName" id="FirstName" autofocus>"""
                Expect.equal value expected ""

        ]

