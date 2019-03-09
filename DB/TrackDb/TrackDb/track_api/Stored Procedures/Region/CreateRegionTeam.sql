CREATE PROCEDURE [track_api].[CreateRegionTeam]
  @RegionId int
, @TeamName nvarchar(256)
, @Coaches AS track.BasicUser READONLY
AS

SET NOCOUNT ON;

DECLARE @TeamId INT;

BEGIN TRANSACTION

EXEC track.CreateTeam @TeamName = @TeamName, @RegionId = @RegionId, @TeamId = @TeamId OUT;

EXEC track.CreateTeamUser @TeamId = @TeamId, @Users = @Coaches, @UserId = NULL;

INSERT INTO track.CoachInvitation (Email, ExpirationDateTime, FirstName, LastName, TeamId)
SELECT u.Email, DATEADD(day, 7, SYSUTCDATETIME()), u.FirstName, u.LastName, @TeamId
FROM @Coaches u
LEFT JOIN track.[User] tu
    ON tu.Email = u.Email
WHERE tu.UserId IS NULL;

COMMIT TRANSACTION

SELECT @TeamId TeamId;

RETURN 0
