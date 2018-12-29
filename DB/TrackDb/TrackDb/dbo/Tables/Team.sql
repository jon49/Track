﻿CREATE TABLE [dbo].[Team]
( [TeamId] INT NOT NULL PRIMARY KEY
, DistrictId INT NOT NULL
, [Name] nvarchar(256) NOT NULL
, CONSTRAINT [FK_Team_DistrictId] FOREIGN KEY ([DistrictId]) REFERENCES [dbo].[District]([DistrictId])
)

GO
