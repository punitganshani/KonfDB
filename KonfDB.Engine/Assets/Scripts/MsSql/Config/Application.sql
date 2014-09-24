CREATE TABLE [Config].[Application] (
    [ApplicationId]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [SuiteId]         BIGINT         NOT NULL,
    [ApplicationName] NVARCHAR (60)  NOT NULL,
    [Description]     NVARCHAR (MAX) NULL,
    [IsActive]        BIT            CONSTRAINT [DF_Applications_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Applications] PRIMARY KEY CLUSTERED ([ApplicationId] ASC),
    CONSTRAINT [FK_Application_Suite] FOREIGN KEY ([SuiteId]) REFERENCES [Config].[Suite] ([SuiteId])
);



