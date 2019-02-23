CREATE PROCEDURE [track_api].[AddUser]
  @Auth0Id varchar(256)
, @FirstName nvarchar(32)
, @LastName nvarchar(32)
, @Email nvarchar(256) = NULL
AS

DECLARE @Ids AS util.Ids;

IF NOT EXISTS (SELECT * FROM track.[User] t WHERE t.Auth0Id = @Auth0Id)
BEGIN
INSERT INTO track.[User] (Auth0Id, Email, FirstName, LastName)
OUTPUT inserted.UserId INTO @Ids
VALUES (@Auth0Id, @Email, @FirstName, @LastName);
END

SELECT TOP 1 Id FROM @Ids;

RETURN 0
