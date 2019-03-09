CREATE FUNCTION [track_api].[GetRegionTeams]
( @RegionId int
) RETURNS TABLE AS RETURN (
SELECT TOP 1000 *
FROM track_api._GetRegionTeams(@RegionId) t
ORDER BY t.[Name]
)
