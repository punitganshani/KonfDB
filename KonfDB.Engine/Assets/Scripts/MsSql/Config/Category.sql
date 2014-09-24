CREATE TABLE [Config].[Category] (
    [CategoryId]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [ApplicationId] BIGINT         NOT NULL,
    [CategoryName]  NVARCHAR (60)  NOT NULL,
    [IsActive]      BIT            CONSTRAINT [DF_Category_IsActive] DEFAULT ((1)) NOT NULL,
    [Description]   NVARCHAR (100) NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([CategoryId] ASC),
    CONSTRAINT [FK_Category_Application] FOREIGN KEY ([ApplicationId]) REFERENCES [Config].[Application] ([ApplicationId])
);

