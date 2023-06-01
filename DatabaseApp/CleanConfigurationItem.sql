CREATE TABLE [Core].[CleanConfigurationItems]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Key] NVARCHAR(50) NOT NULL, 
    [Value] NVARCHAR(MAX) NOT NULL, 
    [CleanConfigurationId] INT NOT NULL, 
    [Rowversion] TIMESTAMP NULL, 
    CONSTRAINT [FK_CleanConfigurationItem_CleanConfiguration] FOREIGN KEY ([CleanConfigurationId]) REFERENCES [Core].[CleanConfigurations]([Id])
)
