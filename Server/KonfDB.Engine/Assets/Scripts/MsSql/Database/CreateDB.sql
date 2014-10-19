CREATE DATABASE $InstanceName$ ON PRIMARY (NAME = $InstanceName$, FILENAME = '$Location$\$InstanceName$.mdf', SIZE = 5MB, MAXSIZE = 10MB, FILEGROWTH = 10%) 
		LOG ON (NAME = $InstanceName$_Log, FILENAME = '$Location$\$InstanceName$.ldf', SIZE = 1MB, MAXSIZE = 5MB, FILEGROWTH = 10%)
--go

--CREATE SCHEMA [Config] AUTHORIZATION [dbo];
--go





