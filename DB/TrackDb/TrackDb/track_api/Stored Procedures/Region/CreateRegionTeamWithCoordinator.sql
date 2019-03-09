CREATE PROCEDURE [track_api].[CreateRegionTeamWithCoordinator]
  @RegionId INT
, @CoordinatorId INT
, @TeamName nvarchar(256)
AS

DECLARE @TeamId INT;

BEGIN TRANSACTION

EXEC track.CreateTeam @TeamName = @TeamName, @RegionId = @RegionId, @TeamId = @TeamId OUT;

EXEC track.CreateTeamUser @TeamId = @TeamId, @Users = NULL, @UserId = @CoordinatorId;

COMMIT TRANSACTION

SELECT @TeamId TeamId;

RETURN 0
