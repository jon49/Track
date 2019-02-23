CREATE PROCEDURE [track_api].[AddLogEntry]
  @Message NVARCHAR(max)
, @Exception NVARCHAR(max)
AS

INSERT INTO track.[Log] ([Message], Exception)
VALUES (@Message, @Exception)

RETURN 0
