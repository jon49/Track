CREATE TABLE [dbo].[RolePermission]
( [RoleId] INT NOT NULL
, PermissionId INT NOT NULL
, CONSTRAINT FK_RolePermission_RoleId FOREIGN KEY (RoleId) REFERENCES [dbo].[Role] (RoleId)
, CONSTRAINT FK_RolePermission_PermissionId FOREIGN KEY (RoleId) REFERENCES [dbo].[Permission] (PermissionId)
, CONSTRAINT PK_RolePermission PRIMARY KEY (RoleId, PermissionId)
)
