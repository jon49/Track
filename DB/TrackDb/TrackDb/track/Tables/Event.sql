CREATE TABLE [track].[Event]
( [EventId] INT NOT NULL PRIMARY KEY
, DistrictId int NOT NULL
, [EventDateTime] DATETIMEOFFSET NOT NULL
, [Location] nvarchar(128) NULL
, [Name] nvarchar(128) NOT NULL
, [Time] datetimeoffset NOT NULL
, NumberOfHeats int NULL
, MaxPerHeat int NULL
, RelayCount int NOT NULL DEFAULT 0
, GenderEvent char(1) NOT NULL
, CONSTRAINT [FK_Event_DistrictId] FOREIGN KEY ([DistrictId]) REFERENCES [track].[District]([DistrictId])
, CONSTRAINT CK_Event_GenderEvent CHECK(GenderEvent IN ('M', 'F', 'B'))
)
