CREATE TABLE [track].[UserRole]
( [UserId] INT NOT NULL
, RoleId INT NOT NULL
, CONSTRAINT FK_UserRole_UserId FOREIGN KEY (UserId) REFERENCES [track].[User] (UserId)
, CONSTRAINT FK_UserRole_RoleId FOREIGN KEY (RoleId) REFERENCES [track].[Role] (RoleId)
, CONSTRAINT PK_UserRole PRIMARY KEY (UserId, RoleId)
)
