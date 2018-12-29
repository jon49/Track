CREATE TABLE [dbo].[UserRole]
( [UserId] INT NOT NULL
, RoleId INT NOT NULL
, CONSTRAINT FK_UserRole_UserId FOREIGN KEY (UserId) REFERENCES [dbo].[User] (UserId)
, CONSTRAINT FK_UserRole_RoleId FOREIGN KEY (RoleId) REFERENCES [dbo].[Role] (RoleId)
, CONSTRAINT PK_UserRole PRIMARY KEY (UserId, RoleId)
)
