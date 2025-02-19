CREATE TABLE [Core].[CleanConfigurationItems]
(
	[Id] [uniqueidentifier] NOT NULL DEFAULT(NEWID()), 
    [Key] NVARCHAR(50) NOT NULL, 
    [Value] NVARCHAR(MAX) NOT NULL, 
    [CleanConfigurationId] [uniqueidentifier] NOT NULL, 
    [Rowversion] TIMESTAMP NULL, 
    CONSTRAINT [FK_CleanConfigurationItem_CleanConfiguration] FOREIGN KEY ([CleanConfigurationId]) REFERENCES [Core].[CleanConfigurations]([Id])
)
