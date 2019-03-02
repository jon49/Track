CREATE PROCEDURE [track_api].[EditTeam]
  @TeamId int
, @Name nvarchar(256)
AS

SET NOCOUNT ON;

UPDATE t
SET t.[Name] = @Name
FROM track.Team t
WHERE t.TeamId = @TeamId

SELECT t.[Name], t.TeamId
FROM track.Team t
WHERE t.TeamId = @TeamId

RETURN 0



