CREATE TABLE [track].[UserTeam]
( [UserId] INT NOT NULL
, TeamId INT NOT NULL
, [Active] BIT NOT NULL DEFAULT 1
, CONSTRAINT FK_UserTeam_UserId FOREIGN KEY (UserId) REFERENCES [track].[User] (UserId)
, CONSTRAINT FK_UserTeam_TeamId FOREIGN KEY (UserId) REFERENCES [track].[Team] (TeamId)
, CONSTRAINT PK_UserTeam PRIMARY KEY (UserId, TeamId)
)

