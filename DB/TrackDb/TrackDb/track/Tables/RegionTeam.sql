CREATE TABLE [track].[RegionTeam]
( [TeamId] INT NOT NULL
, [RegionId] INT NOT NULL
, Active BIT NOT NULL DEFAULT 1
, CONSTRAINT FK_RegionTeam_TeamId FOREIGN KEY (TeamId) REFERENCES [track].[Team] (TeamId)
, CONSTRAINT FK_RegionTeam_RegionId FOREIGN KEY ([RegionId]) REFERENCES [track].[Region] ([RegionId])
, CONSTRAINT PK_RegionTeam PRIMARY KEY (TeamId, [RegionId])
)
