CREATE PROCEDURE [track_api].[CreateRegionTeam]
  @RegionId int
, @TeamName nvarchar(256)
, @FirstName AS util.[IdNames] READONLY
, @LastName AS util.[IdNames] READONLY
, @Email AS util.[IdNames] READONLY
AS

SET NOCOUNT ON;

DECLARE @TeamId util.Ids;
DECLARE @Users AS TABLE (
  FirstName nvarchar(128) NOT NULL
, LastName nvarchar(128) NOT NULL
, Email nvarchar(256) NOT NULL
)

INSERT INTO @Users (Email, FirstName, LastName)
SELECT email.[Name], [first].[Name], [last].[Name]
FROM @Email email
JOIN @FirstName [first] ON [first].Id = email.Id
JOIN @LastName [last] ON [last].Id = email.Id;

BEGIN TRANSACTION

INSERT INTO track.Team ([Name])
OUTPUT inserted.TeamId INTO @TeamId
VALUES (@TeamName);

INSERT INTO track.RegionTeam (RegionId, TeamId)
VALUES (@RegionId, (SELECT Id FROM @TeamId));

UPDATE t
SET t.PreferredTeamId = (SELECT Id FROM @TeamId)
FROM track.[User] t
JOIN @Users u ON u.Email = t.Email;

INSERT INTO track.UserTeam (TeamId, UserId)
SELECT (SELECT Id FROM @TeamId), ut.UserId
FROM track.[User] ut
JOIN @Users u ON u.Email = ut.Email

INSERT INTO track.CoachInvitation (Email, ExpirationDateTime, FirstName, LastName, TeamId)
SELECT u.Email, DATEADD(day, 7, SYSUTCDATETIME()), u.FirstName, u.LastName, (SELECT Id FROM @TeamId)
FROM @Users u
LEFT JOIN track.[User] tu
    ON tu.Email = u.Email
WHERE tu.UserId IS NULL;

COMMIT TRANSACTION

SELECT Id
FROM @TeamId;

RETURN 0
