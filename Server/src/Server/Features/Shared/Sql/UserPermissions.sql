
--DECLARE @AuthId INT = 1;

SELECT
	  u.Auth0Id
	, u.Email
	, u.FirstName
	, u.LastName
	, u.UserId
	, ur.RegionId
	, ut.TeamId
FROM track.[User] u
LEFT JOIN track.UserRegion ur ON ur.UserId = u.UserId
LEFT JOIN track.UserTeam ut ON ut.UserId = u.UserId
WHERE u.Auth0Id = @AuthId;

