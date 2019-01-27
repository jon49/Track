CREATE TABLE [dbo].[UserTeam]
( [UserId] INT NOT NULL
, TeamId INT NOT NULL
, CONSTRAINT FK_UserTeam_UserId FOREIGN KEY (UserId) REFERENCES [track].[User] (UserId)
, CONSTRAINT FK_UserTeam_TeamId FOREIGN KEY (UserId) REFERENCES [track].[Team] (TeamId)
, CONSTRAINT PK_UserTeam PRIMARY KEY (UserId, TeamId)
)

