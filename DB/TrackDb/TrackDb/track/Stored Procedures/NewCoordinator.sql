CREATE PROCEDURE [track].[NewCoordinator]
  @TempGuidId UNIQUEIDENTIFIER
, @UserId INT
AS

DECLARE @Return TABLE
( [Message] varchar(64) NOT NULL
, HttpCode INT NOT NULL
, RegionId INT NOT NULL DEFAULT 0
);

WITH Result AS (
    SELECT
          t.RegionId
        , CASE WHEN t.ExpirationDateTime < GETUTCDATE() THEN 1 ELSE 0 END Expired
        , t.Registered
    FROM track.CoordinatorInvitation t
    WHERE t.TempId = @TempGuidId
)
INSERT @Return (RegionId, HttpCode, [Message])
SELECT
      t.RegionId
    , CASE
        WHEN t.Expired = 1 THEN 400
        WHEN t.Registered = 1 THEN 400
        ELSE 200 END
    , CASE
        WHEN t.Expired = 1 THEN 'This invitation has expired.'
        WHEN t.Registered = 1 THEN 'This registration code has already been used.'
        ELSE '' END
FROM Result t;

IF   (SELECT t.RegionId FROM @Return t) > 0
 AND (SELECT t.HttpCode FROM @Return t) = 200
BEGIN
    BEGIN TRANSACTION

    INSERT INTO track.UserRegion (Active, RegionId, UserId)
    VALUES (1, (SELECT t.RegionId FROM @Return t), @UserId);

    UPDATE t
    SET t.Registered = 1
    FROM track.CoordinatorInvitation t
    WHERE t.TempId = @TempGuidId

    COMMIT TRANSACTION
END;

SELECT *
FROM @Return

RETURN 0
