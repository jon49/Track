CREATE TABLE [dbo].[Player]
( [PlayerId] INT NOT NULL PRIMARY KEY
, TeamId int NOT NULL
, [Name] nvarchar(128) NOT NULL
, Gender char(1) NOT NULL
, CONSTRAINT [FK_Player_TeamId] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Team]([TeamId])
, CONSTRAINT CK_Player_Gender CHECK([Gender] IN ('M', 'F'))
)
