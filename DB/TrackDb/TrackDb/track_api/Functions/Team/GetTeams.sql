CREATE FUNCTION [track_api].[GetTeams]
( @UserId int
) RETURNS TABLE AS RETURN (
SELECT
      t.TeamId
    , t.[Name]
FROM track.UserTeam ut
JOIN track.Team t ON t.TeamId = ut.TeamId
WHERE ut.UserId = @UserId
)
