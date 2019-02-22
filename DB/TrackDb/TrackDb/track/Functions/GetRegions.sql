CREATE FUNCTION [track].[GetRegions]
( @UserId int
) RETURNS TABLE AS RETURN (
SELECT
      r.RegionId
    , r.[Name]
FROM track.UserRegion ur
JOIN track.Region r ON r.RegionId = ur.RegionId
WHERE ur.UserId = @UserId
)
