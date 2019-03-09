CREATE PROCEDURE [track].[CreateTeamUser]
  @TeamId INT
, @Users AS track.BasicUser NULL READONLY
, @UserId INT NULL
AS

SET NOCOUNT ON;

BEGIN TRANSACTION

IF EXISTS (SELECT * FROM @Users)
BEGIN

UPDATE t
SET t.PreferredTeamId = @TeamId
FROM track.[User] t
JOIN @Users u ON u.Email = t.Email;

INSERT INTO track.UserTeam (TeamId, UserId)
SELECT @TeamId, ut.UserId
FROM track.[User] ut
JOIN @Users u ON u.Email = ut.Email

END ELSE IF (@UserId IS NOT NULL)
BEGIN

UPDATE t
SET t.PreferredTeamId = @TeamId
FROM track.[User] t
WHERE t.UserId = @UserId;

INSERT INTO track.UserTeam (TeamId, UserId)
VALUES (@TeamId, @UserId);

END;

COMMIT TRANSACTION

RETURN 0
