CREATE TABLE [Core].[TodoLists]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT(NEWID()),  
    [Title] NVARCHAR(50) NOT NULL, 
    [DueDate] DATETIME NOT NULL, 
    [Rowversion] TIMESTAMP NULL

)
