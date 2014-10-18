CREATE TABLE [Settings].[Options] (   
    [OptionName]		NVARCHAR (60)  NOT NULL,
    [OptionValue]		NVARCHAR (MAX) NULL,
	[AutoLoad]			BIT CONSTRAINT [DF_Settings_Options_AutoLoad] DEFAULT ((1)) NOT NULL,
	[IsEncrypted]			BIT CONSTRAINT [DF_Settings_Options_IsEncrypted] DEFAULT ((0)) NOT NULL,
    [IsActive]			BIT            CONSTRAINT [DF_Settings_Options_IsActive] DEFAULT ((1)) NOT NULL,
	CONSTRAINT [PK_Options] PRIMARY KEY CLUSTERED ([OptionName])
 
);



