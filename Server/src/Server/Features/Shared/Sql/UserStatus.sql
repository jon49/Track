--DECLARE @Auth0Id varchar(128);

SELECT
	  CAST(CASE WHEN ur.UserId IS NULL THEN 0 ELSE 1 END AS BIT) UserIsRegionCoordinator
	, CAST(CASE WHEN ut.UserId IS NULL THEN 0 ELSE 1 END AS BIT) UserIsCoach
FROM track.[User] u
LEFT JOIN track.UserRegion ur
	ON ur.UserId = u.UserId
	AND ur.Active = 1
LEFT JOIN track.UserTeam ut
	ON ut.UserId = u.UserId
	AND ut.Active = 1
WHERE u.Auth0Id = @Auth0Id
