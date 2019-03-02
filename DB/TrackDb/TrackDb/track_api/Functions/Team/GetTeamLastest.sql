CREATE FUNCTION [track_api].[GetTeamLatest]
( @UserId int
) RETURNS TABLE AS RETURN (
SELECT TOP 1
      t.TeamId
    , t.[Name]
FROM track.UserTeam ut
JOIN track.Team t ON t.TeamId = ut.TeamId
WHERE ut.UserId = @UserId
ORDER BY t.TeamId DESC
)
