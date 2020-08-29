CREATE TABLE [dbo].[Cartoon]
(
	[CartoonId] INT NOT NULL, 
    [Name] VARCHAR(50) NOT NULL, 
    [Description] TEXT NULL, 
    [IsActive] BIT NOT NULL DEFAULT 1, 
    [ModifiedBy] VARCHAR(255) NOT NULL DEFAULT SUSER_NAME(), 
    [ModifiedDate] DATETIME NOT NULL DEFAULT getdate(), 
    PRIMARY KEY ([CartoonId])
)
