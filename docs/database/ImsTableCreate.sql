CREATE TABLE ApplicationUser(
	Id bigint IDENTITY(1,1) NOT NULL,
	AspNetUsersId bigint NOT NULL,
	FullName nvarchar(100) NOT NULL,
	CreateBy bigint NOT NULL,
	CreationDate datetime NOT NULL,
	ModifyBy bigint NOT NULL,
	ModificationDate datetime,
	Status int NOT NULL,
	Rank int NOT NULL,
	VersionNumber int NOT NULL,
	BusinessId nvarchar(500)
);