CREATE TABLE [Core].[TodoItems]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT(NEWID()), 
    [Title] NVARCHAR(MAX) NOT NULL, 
    [Completed] BIT NOT NULL DEFAULT 0, 
    [TodoListId] [uniqueidentifier] NOT NULL, 
    [Rowversion] TIMESTAMP NULL
)
