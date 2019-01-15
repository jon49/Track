namespace TrackTests.ReflectionTests

module GetPropertyNameTests =

    open Expecto
    open TrackTests.ViewModel
    open Utils.Reflection

    [<Tests>]
    let tests =

        testList "Get Property Names" [
            testCase "Should be able to get first name property's name." <| fun _ ->
                // Act
                let propertyName = getPropertyName <@ person.FirstName @>

                // Assert
                Expect.equal propertyName (Some "FirstName") ""

            testCase "Should be able to get Age property's name" <| fun _ ->
                // Act
                let propertyName = getPropertyName <@ person.Age @>

                // Assert
                Expect.equal propertyName (Some "Age") ""
        ]
