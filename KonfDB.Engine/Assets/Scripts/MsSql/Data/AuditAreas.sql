IF NOT EXISTS(SELECT * FROM [Config].[AuditArea] WHERE AuditAreaId=100) INSERT INTO [Config].[AuditArea] VALUES (100, 'Access') ELSE UPDATE  [Config].[AuditArea] SET AuditAreaName='Access' WHERE AuditAreaId=100
IF NOT EXISTS(SELECT * FROM [Config].[AuditArea] WHERE AuditAreaId=200) INSERT INTO [Config].[AuditArea] VALUES (200, 'Application') ELSE UPDATE   [Config].[AuditArea] SET AuditAreaName='Application' WHERE AuditAreaId=200
IF NOT EXISTS(SELECT * FROM [Config].[AuditArea] WHERE AuditAreaId=300) INSERT INTO [Config].[AuditArea] VALUES (300, 'Environment') ELSE UPDATE  [Config].[AuditArea] SET AuditAreaName='Environment' WHERE AuditAreaId=300
IF NOT EXISTS(SELECT * FROM [Config].[AuditArea] WHERE AuditAreaId=400) INSERT INTO [Config].[AuditArea] VALUES (400, 'Mappint') ELSE UPDATE  [Config].[AuditArea] SET AuditAreaName='Mapping' WHERE AuditAreaId=400
IF NOT EXISTS(SELECT * FROM [Config].[AuditArea] WHERE AuditAreaId=500) INSERT INTO [Config].[AuditArea] VALUES (500, 'Parameter') ELSE UPDATE [Config].[AuditArea] SET AuditAreaName='Parameter' WHERE AuditAreaId=500
IF NOT EXISTS(SELECT * FROM [Config].[AuditArea] WHERE AuditAreaId=600) INSERT INTO [Config].[AuditArea] VALUES (600, 'Region') ELSE UPDATE [Config].[AuditArea] SET AuditAreaName='Region' WHERE AuditAreaId=600
IF NOT EXISTS(SELECT * FROM [Config].[AuditArea] WHERE AuditAreaId=700) INSERT INTO [Config].[AuditArea] VALUES (700, 'Server') ELSE UPDATE [Config].[AuditArea] SET AuditAreaName='Server' WHERE AuditAreaId=700
IF NOT EXISTS(SELECT * FROM [Config].[AuditArea] WHERE AuditAreaId=800) INSERT INTO [Config].[AuditArea] VALUES (800, 'Suite') ELSE UPDATE [Config].[AuditArea] SET AuditAreaName='Suite' WHERE AuditAreaId=800
IF NOT EXISTS(SELECT * FROM [Config].[AuditArea] WHERE AuditAreaId=1000) INSERT INTO [Config].[AuditArea] VALUES (1000, 'Login') ELSE UPDATE [Config].[AuditArea] SET AuditAreaName='Login' WHERE AuditAreaId=1000

