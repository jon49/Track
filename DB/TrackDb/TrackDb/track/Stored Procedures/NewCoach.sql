CREATE PROCEDURE [track].[NewCoach]
  @TempGuidId UNIQUEIDENTIFIER
, @UserId INT
AS

DECLARE @Return TABLE
( [Message] varchar(64) NOT NULL
, HttpCode INT NOT NULL
, TeamId INT NOT NULL DEFAULT 0
);

WITH Result AS (
    SELECT
          t.TeamId
        , CASE WHEN t.ExpirationDateTime < GETUTCDATE() THEN 1 ELSE 0 END Expired
        , t.Registered
    FROM track.CoachInvitation t
    WHERE t.TempId = @TempGuidId
)
INSERT @Return (TeamId, HttpCode, [Message])
SELECT
      t.TeamId
    , CASE
        WHEN t.Expired = 1 THEN 400
        WHEN t.Registered = 1 THEN 400
        ELSE 200 END
    , CASE
        WHEN t.Expired = 1 THEN 'This invitation has expired.'
        WHEN t.Registered = 1 THEN 'This registration code has already been used.'
        ELSE '' END
FROM Result t;

IF   (SELECT t.TeamId FROM @Return t) > 0
 AND (SELECT t.HttpCode FROM @Return t) = 200
BEGIN
    BEGIN TRANSACTION

    INSERT INTO track.UserTeam (Active, TeamId, UserId)
    VALUES (1, (SELECT t.TeamId FROM @Return t), @UserId);

    UPDATE t
    SET t.Registered = 1
    FROM track.CoachInvitation t
    WHERE t.TempId = @TempGuidId

    UPDATE t
    SET t.PreferredTeamId = (SELECT t.TeamId FROM @Return t)
    FROM track.[User] t
    WHERE t.UserId = @UserId

    COMMIT TRANSACTION
END;

SELECT *
FROM @Return

RETURN 0
