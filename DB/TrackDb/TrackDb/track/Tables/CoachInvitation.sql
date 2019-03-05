CREATE TABLE [track].[CoachInvitation]
( TempId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID()
, ExpirationDateTime DATETIME NOT NULL
, TeamId INT NOT NULL
, UserId INT NULL
, Email nvarchar(256) NULL
, FirstName nvarchar(32) NOT NULL
, LastName nvarchar(32) NOT NULL
, Registered BIT NOT NULL DEFAULT 0
, CONSTRAINT FK_CoachInvitation_TeamId FOREIGN KEY (TeamId) REFERENCES track.Team (TeamId)
, CONSTRAINT FK_CoachInvitation_UserId FOREIGN KEY (UserId) REFERENCES track.[User] (UserId)
)
