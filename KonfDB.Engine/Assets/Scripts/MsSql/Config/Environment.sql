CREATE TABLE [Config].[Environment] (
    [EnvironmentId]     BIGINT        IDENTITY(1,1) NOT NULL,
    [EnvironmentName]   NVARCHAR (50) NOT NULL,
    [EnvironmentTypeId] INT           NOT NULL,
    [SuiteId]           BIGINT        NOT NULL,
    [IsActive]          BIT           CONSTRAINT [DF_Environment_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Environment] PRIMARY KEY CLUSTERED ([EnvironmentId] ASC),
    CONSTRAINT [FK_Environment_EnvironmentType] FOREIGN KEY ([EnvironmentTypeId]) REFERENCES [Config].[EnvironmentType] ([EnvironmentTypeId]),
    CONSTRAINT [FK_Environment_Suite] FOREIGN KEY ([SuiteId]) REFERENCES [Config].[Suite] ([SuiteId])
);

