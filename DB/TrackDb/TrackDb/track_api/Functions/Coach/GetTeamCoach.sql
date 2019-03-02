CREATE FUNCTION [track_api].[GetTeamCoach]
( @UserId INT
, @TeamId INT
) RETURNS TABLE AS RETURN
(
SELECT
      u.UserId
    , u.Email
    , u.FirstName
    , u.LastName
    , ut.TeamId
FROM track.[User] u
JOIN track.UserTeam ut
    ON ut.UserId = u.UserId
WHERE u.UserId = @UserId
  AND ut.TeamId = @TeamId
  AND ut.Active = 1
)
