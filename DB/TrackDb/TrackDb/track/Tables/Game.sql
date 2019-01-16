CREATE TABLE [track].[Game]
( [GameId] INT NOT NULL PRIMARY KEY
, DistrictId int NOT NULL
, [DateTime] datetimeoffset NOT NULL
, [Location] nvarchar(128) NOT NULL
, CONSTRAINT [FK_Game_DistrictId] FOREIGN KEY ([DistrictId]) REFERENCES [track].[District]([DistrictId])
)
