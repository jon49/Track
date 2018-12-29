CREATE TABLE [dbo].[PlayerEvent]
( PlayerId int NOT NULL
, EventId int NOT NULL
, Heat int NULL
, RelayPlacement int NULL
, [Time] time NULL
, Distance decimal NULL
, CONSTRAINT FK_PlayerEvent_PlayerId FOREIGN KEY (PlayerId) REFERENCES [dbo].[Player] (PlayerId)
, CONSTRAINT FK_PlayerEvent_EventId FOREIGN KEY (EventId) REFERENCES [dbo].[Event] (EventId)
)
