CREATE FUNCTION [track_api].[GetTeam]
( @TeamId INT
) RETURNS TABLE AS RETURN
(
SELECT t.[Name], t.TeamId
FROM track.Team t
WHERE t.[TeamId] = @TeamId
)
