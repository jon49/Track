CREATE TABLE [track].[Team]
( [TeamId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY
, RegionId INT NOT NULL
, [Name] nvarchar(128) NOT NULL
, CONSTRAINT [FK_Team_RegionId] FOREIGN KEY ([RegionId]) REFERENCES [track].[Region]([RegionId])
)
