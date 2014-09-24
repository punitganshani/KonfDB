CREATE TABLE [Config].[Server] (
    [ServerId]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [SuiteId]     BIGINT         NOT NULL,
    [ServerName]  VARCHAR (60)   NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    [IsActive]    BIT            CONSTRAINT [DF_Server_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Server] PRIMARY KEY CLUSTERED ([ServerId] ASC),
    CONSTRAINT [FK_Server_Suite] FOREIGN KEY ([SuiteId]) REFERENCES [Config].[Suite] ([SuiteId])
);





