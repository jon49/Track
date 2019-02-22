CREATE PROCEDURE [track].[EditRegion]
  @RegionId int
, @Name nvarchar(256)
AS

UPDATE t
SET t.[Name] = @Name
FROM track.Region t
WHERE t.RegionId = @RegionId

RETURN 0
