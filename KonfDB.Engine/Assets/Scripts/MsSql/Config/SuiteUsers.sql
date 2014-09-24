CREATE TABLE [Config].[SuiteUsers] (
    [AutoId]  INT    IDENTITY (1, 1) NOT NULL,
    [SuiteId] BIGINT NOT NULL,
    [UserId]  INT    NOT NULL,
	[RoleId] [int] NULL,
    CONSTRAINT [PK_SuiteUsers_1] PRIMARY KEY CLUSTERED ([AutoId] ASC),
    CONSTRAINT [FK_Config.SuiteUsers_Suite] FOREIGN KEY ([SuiteId]) REFERENCES [Config].[Suite] ([SuiteId]),
    CONSTRAINT [FK_Config.SuiteUsers_Users] FOREIGN KEY ([UserId]) REFERENCES [Account].[Users] ([UserId]),
	CONSTRAINT [FK_Config.SuiteUsers_Roles] FOREIGN KEY ([RoleId]) REFERENCES [Account].[Roles] ([RoleId])
);









