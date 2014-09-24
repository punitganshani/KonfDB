CREATE TABLE [Config].[Parameter] (
    [ParameterId]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [CategoryId]     BIGINT         NOT NULL,
	[SuiteId]     BIGINT         NOT NULL,
    [ParameterName]  NVARCHAR (100) NOT NULL,
    [ParameterValue] NVARCHAR (MAX) NOT NULL,
    [IsEncrypted]    BIT            CONSTRAINT [DF_Parameter_IsEncrypted] DEFAULT ((1)) NOT NULL,
    [IsActive]       BIT            CONSTRAINT [DF_Parameter_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Parameter] PRIMARY KEY CLUSTERED ([ParameterId] ASC),
	CONSTRAINT [FK_Parameter_Suite] FOREIGN KEY ([SuiteId]) REFERENCES [Config].[Suite] ([SuiteId])
);

