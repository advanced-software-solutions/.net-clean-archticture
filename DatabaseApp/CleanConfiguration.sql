﻿CREATE TABLE [Core].[CleanConfigurations]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(100) NOT NULL, 
    [Rowversion] TIMESTAMP NULL
)
