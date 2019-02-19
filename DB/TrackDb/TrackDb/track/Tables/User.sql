CREATE TABLE [track].[User]
( [UserId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY
, Email nvarchar(256) NULL
, [Auth0Id] varchar(256) NULL
, FirstName nvarchar(32) NOT NULL
, LastName nvarchar(32) NOT NULL
)
