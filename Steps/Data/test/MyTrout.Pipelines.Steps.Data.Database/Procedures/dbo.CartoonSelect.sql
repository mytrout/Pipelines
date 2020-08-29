CREATE PROCEDURE [dbo].[CartoonSelect]
	@CartoonId int
AS
	SELECT
		CartoonId,
		[Name],
		[Description],
		IsActive,
		ModifiedBy,
		ModifiedDate
	FROM
		dbo.Cartoon
	WHERE
		CartoonId = @CartoonId
