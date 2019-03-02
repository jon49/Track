CREATE FUNCTION [track_api].[GetRegion]
( @RegionId INT
) RETURNS TABLE AS RETURN
(
SELECT r.[Name], r.RegionId
FROM track.Region r
WHERE r.[RegionId] = @RegionId
)
