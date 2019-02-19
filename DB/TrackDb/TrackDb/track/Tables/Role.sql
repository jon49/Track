CREATE TABLE [track].[Role]
( [RoleId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY
, [Name] varchar(128) NOT NULL
, CONSTRAINT CK_Role_Name CHECK([Name] IN ('Coordinator', 'Coach'))
)
