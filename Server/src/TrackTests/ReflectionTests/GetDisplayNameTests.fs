namespace TrackTests.ReflectionTests

module GetDisplayNameTests =

    open Expecto
    open TrackTests
    open ViewModel
    open Utils.Reflection

    [<Tests>]
    let tests =

        testList "Get Property Display Name" [
            testCase "Should be able to get first name property's display name." <| fun _ ->
                // Act
                let displayName = getDisplayName <@ person.FirstName @>

                // Assert
                Expect.equal displayName FIRST_NAME ""

            testCase "Should be able to get Age property's display name fallback to the name of the property itself" <| fun _ ->
                // Act
                let displayName = getDisplayName <@ person.Age @>

                // Assert
                Expect.equal displayName "Age" ""

            testCase "Should use property name if display name is empty string" <| fun _ ->
                // Act
                let displayName = getDisplayName <@ person.LastName @>

                // Assert
                Expect.equal displayName "LastName" ""

            testCase "Should use property name if display name is null" <| fun _ ->
                // Act
                let displayName = getDisplayName <@ person.MiddleName @>

                // Assert
                Expect.equal displayName "MiddleName" ""
        ]
