CREATE FUNCTION [track_api].[GetLatestTeamWithCoaches]
( @RegionId int
) RETURNS TABLE AS RETURN (
WITH Latest AS (
SELECT TOP 1 rt.TeamId
FROM track.RegionTeam rt
WHERE rt.RegionId = @RegionId
  AND rt.Active = 1
ORDER BY rt.TeamId DESC
)
SELECT t.*
FROM track_api._GetRegionTeams(@RegionId) t
JOIN Latest l ON l.TeamId = t.TeamId
)
