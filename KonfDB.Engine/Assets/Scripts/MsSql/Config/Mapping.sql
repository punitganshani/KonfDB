CREATE TABLE [Config].[Mapping] (
    [MappingId]     BIGINT IDENTITY (1, 1) NOT NULL,
    [SuiteId]       BIGINT NOT NULL,
    [EnvironmentId] BIGINT NOT NULL,
    [ApplicationId] BIGINT NOT NULL,
    [RegionId]      BIGINT NOT NULL,
    [ServerId]      BIGINT NOT NULL,
    [CategoryId]    BIGINT NULL,
    [ParameterId]   BIGINT NOT NULL,
    CONSTRAINT [PK_Mapping] PRIMARY KEY CLUSTERED ([MappingId] ASC),
    CONSTRAINT [FK_Mapping_Application] FOREIGN KEY ([ApplicationId]) REFERENCES [Config].[Application] ([ApplicationId]),    
    CONSTRAINT [FK_Mapping_Environment] FOREIGN KEY ([EnvironmentId]) REFERENCES [Config].[Environment] ([EnvironmentId]),
    CONSTRAINT [FK_Mapping_Mapping] FOREIGN KEY ([ParameterId]) REFERENCES [Config].[Parameter] ([ParameterId]),
    CONSTRAINT [FK_Mapping_Region] FOREIGN KEY ([RegionId]) REFERENCES [Config].[Region] ([RegionId]),
    CONSTRAINT [FK_Mapping_Server] FOREIGN KEY ([ServerId]) REFERENCES [Config].[Server] ([ServerId]),
    CONSTRAINT [FK_Mapping_Suite] FOREIGN KEY ([SuiteId]) REFERENCES [Config].[Suite] ([SuiteId])
);

