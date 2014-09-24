-- Add Roles

IF NOT EXISTS(SELECT * FROM [Account].[Roles] WHERE RoleId=100) 
	BEGIN
		SET  IDENTITY_INSERT [Account].[Roles] ON
		INSERT INTO [Account].[Roles] (RoleId, RoleName) VALUES (100, 'Administrator') 
		SET  IDENTITY_INSERT [Account].[Roles] OFF
	END 
ELSE 
	UPDATE [Account].[Roles] SET RoleName='Admin' WHERE RoleId=100


IF NOT EXISTS(SELECT * FROM [Account].[Roles] WHERE RoleId=200) 
	BEGIN
		SET  IDENTITY_INSERT [Account].[Roles] ON
		INSERT INTO [Account].[Roles] (RoleId, RoleName) VALUES (200, 'ReadOnly') 
		SET  IDENTITY_INSERT [Account].[Roles] OFF
	END 
ELSE 
	UPDATE [Account].[Roles] SET RoleName='ReadOnly' WHERE RoleId=200