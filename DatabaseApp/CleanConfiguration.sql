CREATE TABLE [Core].[CleanConfigurations]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT(NEWID()), 
    [Name] NVARCHAR(100) NOT NULL, 
    [Rowversion] TIMESTAMP NULL
)
