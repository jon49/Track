﻿CREATE TABLE [track].[Player]
( [PlayerId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY
, TeamId int NOT NULL
, [Name] nvarchar(128) NOT NULL
, Gender char(1) NOT NULL
, CONSTRAINT [FK_Player_TeamId] FOREIGN KEY ([TeamId]) REFERENCES [track].[Team]([TeamId])
, CONSTRAINT CK_Player_Gender CHECK([Gender] IN ('M', 'F'))
)
