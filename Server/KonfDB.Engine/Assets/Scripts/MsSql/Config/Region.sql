CREATE TABLE [Config].[Region] (
    [RegionId]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [SuiteId]     BIGINT         NOT NULL,
    [RegionName]  VARCHAR (60)   NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    [IsActive]    BIT            CONSTRAINT [DF_Region_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED ([RegionId] ASC),
    CONSTRAINT [FK_Region_Suite] FOREIGN KEY ([SuiteId]) REFERENCES [Config].[Suite] ([SuiteId])
);





