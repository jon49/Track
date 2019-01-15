namespace TrackTests.ReflectionTests

module GetPropertyValueTests =

    open Expecto
    open TrackTests.ViewModel
    open Utils.Reflection

    [<Tests>]
    let tests =

        testList "Get Property Value" [
            testCase "Should be able to get first name property's value." <| fun _ ->
                // Act
                let value = getPropertyValue <@ person.FirstName @>

                // Assert
                Expect.equal value (Some person.FirstName) ""

            testCase "Should be able to get Age property's name" <| fun _ ->
                // Act
                let value = getPropertyValue <@ person.Age @>

                // Assert
                Expect.equal value (Some person.Age) ""
        ]
