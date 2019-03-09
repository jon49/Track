CREATE PROCEDURE [track].[CreateTeam]
  @TeamName nvarchar(256)
, @RegionId INT
, @TeamId INT OUTPUT
AS

SET NOCOUNT ON;

DECLARE @TeamIds util.Ids;

BEGIN TRANSACTION

INSERT INTO track.Team ([Name])
OUTPUT inserted.TeamId INTO @TeamIds
VALUES (@TeamName);

SELECT @TeamId = Id
FROM @TeamIds

INSERT INTO track.RegionTeam (RegionId, TeamId)
VALUES (@RegionId, @TeamId);

COMMIT TRANSACTION

RETURN 0
