CREATE TABLE [Config].[EnvironmentType] (
    [EnvironmentTypeId] INT          NOT NULL,
    [TypeName]          VARCHAR (30) NOT NULL,
    CONSTRAINT [PK_EnvironmentType] PRIMARY KEY CLUSTERED ([EnvironmentTypeId] ASC)
);

