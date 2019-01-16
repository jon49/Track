CREATE TABLE [track].[User]
( [UserId] INT NOT NULL PRIMARY KEY
, Email nvarchar(256) NOT NULL
, AuthId varchar(256) NOT NULL
, LastLoginDateTime DateTimeOffset NOT NULL
, FirstName nvarchar(128) NOT NULL
, LastName nvarchar(128) NOT NULL
)
