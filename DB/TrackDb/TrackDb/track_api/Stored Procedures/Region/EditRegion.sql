CREATE PROCEDURE [track_api].[EditRegion]
  @RegionId int
, @Name nvarchar(256)
AS

SET NOCOUNT ON;

UPDATE t
SET t.[Name] = @Name
FROM track.Region t
WHERE t.RegionId = @RegionId

SELECT t.[Name], t.RegionId
FROM track.Region t
WHERE t.RegionId = @RegionId

RETURN 0
