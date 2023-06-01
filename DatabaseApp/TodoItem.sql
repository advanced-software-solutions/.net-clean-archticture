﻿CREATE TABLE [Core].[TodoItems]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Title] NVARCHAR(MAX) NOT NULL, 
    [Completed] BIT NOT NULL DEFAULT 0, 
    [TodoListId] INT NOT NULL, 
    [Rowversion] TIMESTAMP NULL
)
