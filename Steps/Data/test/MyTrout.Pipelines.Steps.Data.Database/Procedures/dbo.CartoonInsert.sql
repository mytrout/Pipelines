CREATE PROCEDURE [dbo].[CartoonInsert]
	@CartoonId int,
	@Name varchar(50),
	@Description text = null
AS
	INSERT INTO dbo.Cartoon (
		CartoonId,
		[Name],
		[Description],
		IsActive,
		ModifiedBy,
		ModifiedDate)
	VALUES (	
		@CartoonId,
		@Name,
		@Description,
		1,
		SUSER_NAME(),
		GETDATE())

	SELECT SCOPE_IDENTITY();
