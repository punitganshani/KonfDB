CREATE TABLE [Config].[Audit] (
    [AuditId]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [AuditAreaId]   INT            NOT NULL,
    [AuditIdentity] NVARCHAR(100)  NOT NULL,
    [Message]       NVARCHAR (MAX) NOT NULL,
    [UserId]        INT            NOT NULL,
    [ActionAtUtc]   DATETIME       NOT NULL,
	[Reason]		NVARCHAR(20)   NOT NULL,
    [Metadata1]     NVARCHAR (50)  NULL,
    [Metadata2]     NVARCHAR (50)  NULL,
    CONSTRAINT [PK_Audit] PRIMARY KEY CLUSTERED ([AuditId] ASC),
    CONSTRAINT [FK_Audit_AuditArea] FOREIGN KEY ([AuditAreaId]) REFERENCES [Config].[AuditArea] ([AuditAreaId])
);

