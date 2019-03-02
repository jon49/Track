CREATE PROCEDURE [track_api].[AddTeam]
  @RegionId INT
, @UserId INT
, @TeamName nvarchar(256)
AS

SET NOCOUNT ON;

DECLARE @Ids util.Ids;

BEGIN TRANSACTION

INSERT INTO track.Team ([Name])
OUTPUT inserted.TeamId INTO @Ids
VALUES (@TeamName);

INSERT INTO track.RegionTeam (RegionId, TeamId)
VALUES (@RegionId, (SELECT Id FROM @Ids));

INSERT INTO track.UserTeam (TeamId, UserId)
VALUES ((SELECT Id FROM @Ids), @UserId);

COMMIT TRANSACTION

SELECT t.Id TeamId
FROM @Ids t

RETURN 0
