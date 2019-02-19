CREATE TABLE [track].[CoordinatorInvitation]
( TempId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID()
, ExpirationDateTime DATETIME NOT NULL
, RegionId INT NOT NULL
, Registered BIT NOT NULL DEFAULT 0
, CONSTRAINT FK_CoordinatorInvitation_RegionId FOREIGN KEY (RegionId) REFERENCES track.Region (RegionId)
)
