CREATE TABLE [track].[Game]
( [GameId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY
, RegionId int NOT NULL
, [DateTime] datetimeoffset NOT NULL
, [Location] nvarchar(128) NOT NULL
, CONSTRAINT [FK_Game_RegionId] FOREIGN KEY ([RegionId]) REFERENCES [track].[Region]([RegionId])
)
