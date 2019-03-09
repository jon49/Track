CREATE FUNCTION [track_api].[_GetRegionTeams]
( @RegionId int
) RETURNS TABLE AS RETURN (
SELECT
      t.[Name]
    , t.TeamId
    , rt.RegionId
    , u.Email
    , u.FirstName
    , u.LastName
    , u.UserId
    , u.PreferredTeamId
FROM track.Team t
JOIN track.RegionTeam rt
    ON rt.TeamId = t.TeamId
JOIN track.UserTeam ut
    ON ut.TeamId = t.TeamId
    AND ut.Active = 1
JOIN track.[User] u
    ON u.UserId = ut.UserId
WHERE rt.RegionId = @RegionId
  AND rt.Active = 1
)
