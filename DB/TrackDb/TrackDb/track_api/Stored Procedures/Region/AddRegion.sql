CREATE PROCEDURE [track_api].[AddRegion]
  @UserId int
, @RegionName nvarchar(256)
AS

SET NOCOUNT ON;

DECLARE @Ids util.Ids;

BEGIN TRANSACTION

INSERT INTO track.Region ([Name])
OUTPUT inserted.RegionId INTO @Ids
VALUES (@RegionName);

INSERT INTO track.UserRegion (Active, RegionId, UserId)
VALUES (1, (SELECT Id FROM @Ids), @UserId);

COMMIT TRANSACTION

SELECT t.Id
FROM @Ids t

RETURN 0
