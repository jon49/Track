CREATE TABLE track.UserRegion
( UserId INT NOT NULL
, [RegionId] INT NOT NULL
, Active BIT NOT NULL DEFAULT 1
, CONSTRAINT FK_UserDistrict_UserId FOREIGN KEY (UserId) REFERENCES [track].[User] (UserId)
, CONSTRAINT FK_UserDistrict_RegionId FOREIGN KEY ([RegionId]) REFERENCES [track].[Region] ([RegionId])
, CONSTRAINT PK_UserDistrict PRIMARY KEY (UserId, [RegionId])
)
