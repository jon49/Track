CREATE FUNCTION [track].[GetRegionLatest]
( @UserId int
) RETURNS TABLE AS RETURN (
SELECT TOP 1
      r.RegionId
    , r.[Name]
FROM track.UserRegion ur
JOIN track.Region r ON r.RegionId = ur.RegionId
WHERE ur.UserId = @UserId
ORDER BY r.RegionId DESC
)
