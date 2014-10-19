CREATE TABLE [Config].[Suite] (
    [SuiteId]           BIGINT        IDENTITY (1, 1) NOT NULL,
    [SuiteName]         VARCHAR (60)  NOT NULL,
    [IsActive]          BIT           CONSTRAINT [DF_Suites_IsActive] DEFAULT ((1)) NOT NULL,
    [UsesSysEncryption] BIT           CONSTRAINT [DF_Suite_UseSystemEncryption] DEFAULT ((1)) NOT NULL,
    [PublicKey]         NVARCHAR (1000) NULL,
    [PrivateKey]        NVARCHAR (1000) NULL,
    [CreatedDate]       DATETIME      CONSTRAINT [DF_Suite_CreatedDate] DEFAULT (getdate()) NULL,
    [Timestamp]         ROWVERSION    NULL,
    [SuiteType]         INT           CONSTRAINT [DF_Suite_SuiteType] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Suites] PRIMARY KEY CLUSTERED ([SuiteId] ASC)
);



