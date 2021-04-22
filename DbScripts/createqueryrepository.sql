﻿CREATE SCHEMA GQuery
GO

CREATE TABLE GQuery.QueryRepository(
	Id int IDENTITY(1,1) NOT NULL,
	Name [NVARCHAR](256) NOT NULL,
	Label [NVARCHAR](256) NOT NULL,
	Description [NVARCHAR](2048) NULL,
	Sql [NVARCHAR](2048) NOT NULL,
	CONSTRAINT [PK_SYSADM_TSY_QUERYREPOSITORY] PRIMARY KEY CLUSTERED (Id ASC),
	CONSTRAINT [UC_SYSADM_TSY_QUERYREPOSITORY_NAME] UNIQUE(Name)
)
GO

