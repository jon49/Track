CREATE PROCEDURE [track].[GetUserInfo]
  @Auth0Id varchar(256)
AS

SELECT
	  u.Auth0Id
	, u.Email
	, u.FirstName
	, u.LastName
	, u.UserId
    , u.PreferredRegionId
    , u.PreferredTeamId
	, ur.RegionId
	, ut.TeamId
    , r.[Name] RoleName
FROM track.[User] u
LEFT JOIN track.UserRegion ur ON ur.UserId = u.UserId
LEFT JOIN track.UserTeam ut ON ut.UserId = u.UserId
LEFT JOIN (
    track.UserRole
    JOIN track.[Role] r ON r.RoleId = UserRole.RoleId
    ) ON ur.UserId = u.UserId
WHERE u.Auth0Id = @Auth0Id;

RETURN 0
