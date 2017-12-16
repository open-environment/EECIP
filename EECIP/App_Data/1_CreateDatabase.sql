﻿/***************************************************************** */
/*************DROP EXISTING DATABASE (only use if refreshing DB*** */
/***************************************************************** */
/*
	  EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'EECIP'
	  GO
	  USE [master]
	  GO
	  ALTER DATABASE [EECIP] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
	  GO
	  USE [master]
	  GO
	  DROP DATABASE [EECIP]
	  GO
*/

/************************************************************ */
/*************CREATE DATABASE******************************** */
/************************************************************ */
CREATE DATABASE [EECIP]
GO


/************************************************************************* */
/*************CREATE USER AND GRANT RIGHTS******************************** */
/************************************************************************* */
IF EXISTS (SELECT * FROM sys.server_principals WHERE name = N'eecip_login')
DROP LOGIN [eecip_login]


use [EECIP]
Create login eecip_login with password='FEG$GHWjpN!18g';
EXEC sp_defaultdb @loginame='eecip_login', @defdb='EECIP' 
Create user [eecip_user] for login [eecip_login]; 
exec sp_addrolemember 'db_owner', 'eecip_user'; 



/************************************************************ */
/*************CREATE TABLES  ******************************** */
/************************************************************ */
Use [EECIP] 

CREATE TABLE [dbo].[T_OE_APP_SETTINGS](
	[SETTING_IDX] [int] IDENTITY(1,1) NOT NULL,
	[SETTING_NAME] [varchar](100) NOT NULL,
	[SETTING_DESC] [varchar](500) NULL,
	[SETTING_VALUE] [varchar](200) NULL,
	[ENCRYPT_IND] [bit] NULL,
	[MODIFY_USERIDX] [int] NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_APP_SETTINGS] PRIMARY KEY CLUSTERED ([SETTING_IDX] ASC)
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[T_OE_REF_STATE](
	[STATE_CD] [varchar](5) NOT NULL,
	[STATE_NAME] [varchar](100) NOT NULL,
 CONSTRAINT [PK_T_OE_REF_STATE] PRIMARY KEY CLUSTERED  ([STATE_CD] ASC)
) ON [PRIMARY]

GO



CREATE TABLE [dbo].[T_OE_REF_REGION](
	[EPA_REGION] [int] NOT NULL,
	[EPA_REGION_NAME] [varchar](100) NOT NULL,
 CONSTRAINT [PK_T_OE_REF_REGION] PRIMARY KEY CLUSTERED  ([EPA_REGION] ASC)
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[T_OE_REF_SYNONYMS](
	[SYNONYM_IDX] [int] IDENTITY(1,1) NOT NULL,
	[SYNONYM_TEXT] [varchar](100) NOT NULL,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
	[MODIFY_USERIDX] [int] NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_REF_SYNONYMS] PRIMARY KEY CLUSTERED  ([SYNONYM_IDX] ASC)
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[T_OE_REF_TAG_CATEGORIES](
	[TAG_CAT_NAME] [varchar](50) NOT NULL,
	[TAG_CAT_DESCRIPTION] [varchar](200) NULL,
	[TAG_CAT_COLOR] [varchar](6) NULL,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_REF_TG_CATEGORIES] PRIMARY KEY CLUSTERED  ([TAG_CAT_NAME] ASC)
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[T_OE_REF_TAGS](
	[TAG_IDX] [int] IDENTITY(1,1) NOT NULL,
	[TAG_NAME] [varchar](100) NOT NULL,
	[TAG_CAT_NAME] [varchar](50) NOT NULL,
	[USER_CREATE_IND] [bit] NOT NULL DEFAULT 0,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_REF_TAGS] PRIMARY KEY CLUSTERED  ([TAG_IDX] ASC),
 FOREIGN KEY ([TAG_CAT_NAME]) references [T_OE_REF_TAG_CATEGORIES] ([TAG_CAT_NAME])
) ON [PRIMARY]

GO
 
 CREATE TABLE [dbo].[T_OE_REF_ENTERPRISE_PLATFORM](
	[ENT_PLATFORM_IDX] [int] IDENTITY(1,1) NOT NULL,
	[ENT_PLATFORM_NAME] [varchar](80) NOT NULL,
	[ENT_PLATFORM_DESC] [varchar](1000) NULL,
	[ENT_PLATFORM_EXAMPLE] [varchar](100) NULL,
	[SEQ_NO] [int] NULL,
	[ACT_IND] [bit] NOT NULL,
	[MODIFY_USERIDX] [int] NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_REF_ENTERPRISE_PLATFORM] PRIMARY KEY CLUSTERED  ([ENT_PLATFORM_IDX] ASC)
) ON [PRIMARY]


--ALTER TABLE [T_OE_REF_ENTERPRISE_PLATFORM]  alter column [ENT_PLATFORM_DESC] [varchar](1000) NULL;
GO

CREATE TABLE [dbo].[T_OE_ORGANIZATION](
	[ORG_IDX] [uniqueidentifier] NOT NULL DEFAULT newid(),
	[ORG_ABBR] [varchar](100) NOT NULL,
	[ORG_NAME] [varchar](200) NULL,
	[STATE_CD] [varchar](5) NULL,
	[EPA_REGION] [int] NULL,
	[ALLOW_JOIN_IND] [bit] NULL,
	[CLOUD] [varchar](250) NULL,
	[API] [varchar](250) NULL,
	[ACT_IND] [bit] NOT NULL DEFAULT 1,
	[SYNC_IND] [bit] NOT NULL DEFAULT 0,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
	[MODIFY_USERIDX] [int] NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_ORGANIZATION] PRIMARY KEY CLUSTERED  ([ORG_IDX] ASC),
 FOREIGN KEY ([STATE_CD]) references T_OE_REF_STATE ([STATE_CD]),
 FOREIGN KEY ([EPA_REGION]) references T_OE_REF_REGION ([EPA_REGION])
) ON [PRIMARY]

GO	 


CREATE TABLE [dbo].[T_OE_ORGANIZATION_TAGS](
	[ORG_IDX] [uniqueidentifier] NOT NULL,
	[ORG_ATTRIBUTE] [varchar](50) NOT NULL,
	[ORG_TAG] [varchar](100) NOT NULL,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_ORGANIZATION_TAGS] PRIMARY KEY CLUSTERED ([ORG_IDX], [ORG_ATTRIBUTE], [ORG_TAG] ASC),
 FOREIGN KEY ([ORG_IDX]) references [T_OE_ORGANIZATION] ([ORG_IDX]) ON UPDATE CASCADE 	ON DELETE CASCADE
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[T_OE_ORGANIZATION_ENT_SVCS](
	[ORG_ENT_SVCS_IDX] [int] IDENTITY(1,1) NOT NULL,
	[ORG_IDX] [uniqueidentifier] NOT NULL,
	[ENT_PLATFORM_IDX] [int] NOT NULL,
	[PROJECT_NAME] [varchar](100) NULL,
	[VENDOR] [varchar](100) NULL,
	[IMPLEMENT_STATUS] [varchar](250) NULL,
	[COMMENTS] [varchar](max) NULL,
	[RECORD_SOURCE] [varchar](50) NULL,
	[SYNC_IND] [bit] NOT NULL DEFAULT 0,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
	[MODIFY_USERIDX] [int] NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_ORGANIZATION_ENT_SVCS] PRIMARY KEY CLUSTERED ([ORG_ENT_SVCS_IDX] ASC),
 FOREIGN KEY ([ORG_IDX]) references [T_OE_ORGANIZATION] ([ORG_IDX]) ON UPDATE CASCADE 	ON DELETE CASCADE,
 FOREIGN KEY ([ENT_PLATFORM_IDX]) references T_OE_REF_ENTERPRISE_PLATFORM ([ENT_PLATFORM_IDX]) ON UPDATE CASCADE ON DELETE CASCADE 
) ON [PRIMARY]

GO

--  alter table T_OE_ORGANIZATION_ENT_SVCS add SYNC_IND [bit] NOT NULL DEFAULT 0;
--  alter table T_OE_ORGANIZATION_ENT_SVCS add RECORD_SOURCE varchar(50) NULL;

 CREATE TABLE [dbo].[T_OE_ORGANIZATION_EMAIL_RULE](
	[ORG_IDX] [uniqueidentifier] NOT NULL,
	[EMAIL_STRING] [varchar](100) NOT NULL,
	[MODIFY_USERIDX] [int] NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_ORGANIZATION_EMAIL_RULE] PRIMARY KEY CLUSTERED  ([ORG_IDX] ASC, [EMAIL_STRING] ASC),
 FOREIGN KEY ([ORG_IDX]) references T_OE_ORGANIZATION ([ORG_IDX]) ON UPDATE CASCADE 	ON DELETE CASCADE
) ON [PRIMARY]

GO	




CREATE TABLE [dbo].[T_OE_USERS](
	[USER_IDX] [int] IDENTITY(0,1) NOT NULL,
	[USER_ID] [varchar](150) NOT NULL,
	[PWD_HASH] [varchar](100) NOT NULL,
	[PWD_SALT] [varchar](100) NOT NULL,
	[FNAME] [varchar](40) NOT NULL,
	[LNAME] [varchar](40) NOT NULL,
	[EMAIL] [varchar](150) NULL,
	[INITAL_PWD_FLAG] [bit] NOT NULL,
	[EFFECTIVE_DT] [datetime2](0) NOT NULL,
	[LASTLOGIN_DT] [datetime2](0) NULL,
	[PHONE] [varchar](12) NULL,
	[PHONE_EXT] [varchar](4) NULL,
	[JOB_TITLE] [varchar](40) NULL,
	[NODE_ADMIN] [bit] NOT NULL DEFAULT 0,
	[USER_AVATAR] [varbinary](max) NULL,
	[LINKEDIN] [varchar](100) NULL,
	[LOG_ATMPT] [int] NULL,
	[LOCKOUT_END_DATE_UTC] [datetime2] NULL,
	[LOCKOUT_ENABLED] [bit] NOT NULL DEFAULT 0,
	[ORG_IDX] [uniqueidentifier] NULL,
	[SYNC_IND] [bit] NOT NULL DEFAULT 0,
	[ACT_IND] [bit] NOT NULL,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
	[MODIFY_USERIDX] [int] NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_USERS] PRIMARY KEY CLUSTERED ( [USER_IDX] ASC),
 FOREIGN KEY ([ORG_IDX]) references T_OE_ORGANIZATION ([ORG_IDX]), 
) ON [PRIMARY]

--  alter table [T_OE_USERS] add SYNC_IND [bit] NOT NULL DEFAULT 0;
--  alter table [T_OE_USERS] add LINKEDIN [varchar](100) NULL;



CREATE TABLE [dbo].[T_OE_ROLES](
	[ROLE_IDX] [int] IDENTITY(1,1) NOT NULL,
	[ROLE_NAME] [varchar](25) NOT NULL,
	[ROLE_DESC] [varchar](100) NOT NULL,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
	[MODIFY_USERIDX] [int] NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_ROLES] PRIMARY KEY CLUSTERED  ([ROLE_IDX] ASC)
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[T_OE_USER_ROLES](
	[USER_ROLE_IDX] [int] IDENTITY(1,1) NOT NULL,
	[USER_IDX] [int] NOT NULL,
	[ROLE_IDX] [int] NOT NULL,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_USER_ROLES] PRIMARY KEY CLUSTERED ([USER_ROLE_IDX] ASC),
 CONSTRAINT [UK_T_OE_USER_ROLES] UNIQUE NONCLUSTERED (	[USER_IDX] ASC,	[ROLE_IDX] ASC),
 FOREIGN KEY (ROLE_IDX) references T_OE_ROLES (ROLE_IDX) ON UPDATE CASCADE 	ON DELETE CASCADE, 
 FOREIGN KEY (USER_IDX) references T_OE_USERS (USER_IDX) ON UPDATE CASCADE 	ON DELETE CASCADE
) ON [PRIMARY]

GO




CREATE TABLE [dbo].[T_OE_USER_EXPERTISE](
	[USER_IDX] [int] NOT NULL,
	[EXPERTISE_TAG] [varchar](100) NOT NULL,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_USER_EXPERTISE] PRIMARY KEY CLUSTERED ([USER_IDX], [EXPERTISE_TAG] ASC),
 FOREIGN KEY (USER_IDX) references T_OE_USERS (USER_IDX) ON UPDATE CASCADE 	ON DELETE CASCADE
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[T_OE_USER_NOTIFICATION](
	[NOTIFICATION_IDX] [uniqueidentifier] NOT NULL,
	[USER_IDX] [int] NOT NULL,
	[NOTIFY_DT] [datetime2](0) NOT NULL,
	[NOTIFY_TYPE] [varchar](10) NOT NULL,
	[NOTIFY_TITLE] [varchar](50) NULL,
	[NOTIFY_DESC] [varchar](200) NULL,
	[READ_IND] [bit] NULL,
	[FROM_USER_IDX] [int] NULL,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
	[MODIFY_USERIDX] [int] NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_EM_USER_NOTIFICATION] PRIMARY KEY CLUSTERED ([NOTIFICATION_IDX] ASC )
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[T_OE_PROJECTS](
	[PROJECT_IDX] [uniqueidentifier] NOT NULL DEFAULT newid(),
	[ORG_IDX] [uniqueidentifier] NULL,
	[PROJ_NAME] [varchar](150) NULL,
	[PROJ_DESC] [varchar](max) NULL,
	[MEDIA_TAG] [int] NULL,
	[START_YEAR] [int] NULL,
	[PROJ_STATUS] [varchar](100) NULL,
	[DATE_LAST_UPDATE] [int] NULL,
	[RECORD_SOURCE] [varchar](50) NULL,
	[PROJECT_URL] [varchar](200) NULL,
	[MOBILE_IND] [int] NULL,
	[MOBILE_DESC] [varchar](100) NULL,
	[ADV_MON_IND] [int] NULL,
	[ADV_MON_DESC] [varchar](100) NULL,
	[BP_MODERN_IND] [int] NULL,
	[BP_MODERN_DESC] [varchar](100) NULL,
	[COTS] [varchar](100) NULL,
	[VENDOR] [varchar](100) NULL,
	[PROJECT_CONTACT] [varchar](100) NULL,
	[IMPORT_ID] [varchar](20) NULL,
	[ACT_IND] [bit] NOT NULL DEFAULT 1,
	[SYNC_IND] [bit] NOT NULL DEFAULT 0,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
	[MODIFY_USERIDX] [int] NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_PROJECTS] PRIMARY KEY CLUSTERED  ([PROJECT_IDX] ASC),
 FOREIGN KEY ([ORG_IDX]) references [T_OE_ORGANIZATION] ([ORG_IDX]),
 FOREIGN KEY ([MEDIA_TAG]) references [T_OE_REF_TAGS] ([TAG_IDX]),
 FOREIGN KEY ([MOBILE_IND]) references [T_OE_REF_TAGS] ([TAG_IDX]),
 FOREIGN KEY ([ADV_MON_IND]) references [T_OE_REF_TAGS] ([TAG_IDX]),
 FOREIGN KEY ([BP_MODERN_IND]) references [T_OE_REF_TAGS] ([TAG_IDX])
) ON [PRIMARY]

GO	 

--  alter table T_OE_PROJECTS add IMPORT_ID [varchar](20) NULL;
--  alter table T_OE_PROJECTS add PROJECT_CONTACT [varchar](100) NULL;


CREATE TABLE [dbo].[T_OE_PROJECT_TAGS](
	[PROJECT_IDX] [uniqueidentifier] NOT NULL DEFAULT newid(),
	[PROJECT_ATTRIBUTE] [varchar](50) NOT NULL,
	[PROJECT_TAG_NAME] [varchar](100) NOT NULL,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_PROJECT_TAGS] PRIMARY KEY CLUSTERED ([PROJECT_IDX], [PROJECT_ATTRIBUTE], [PROJECT_TAG_NAME] ASC),
 FOREIGN KEY ([PROJECT_IDX]) references [T_OE_PROJECTS] ([PROJECT_IDX]) ON UPDATE CASCADE,
) ON [PRIMARY]

GO	 




CREATE TABLE [dbo].[T_OE_SYS_LOG](
	[SYS_LOG_ID] [int] IDENTITY(1,1) NOT NULL,
	[LOG_DT] [datetime2](0) NOT NULL,
	[LOG_USERIDX] [int] NULL,
	[LOG_TYPE] [varchar](15) NULL,
	[LOG_MSG] [varchar](2000) NULL,
 CONSTRAINT [PK_T_OE_SYS_LOG] PRIMARY KEY CLUSTERED  ([SYS_LOG_ID] ASC)
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[T_OE_SYS_EMAIL_LOG](
	[EMAIL_LOG_ID] [int] IDENTITY(1,1) NOT NULL,
	[LOG_DT] [datetime2](0) NULL,
	[LOG_FROM] [varchar](200) NULL,
	[LOG_TO] [varchar](200) NULL,
	[LOG_CC] [varchar](200) NULL,
	[LOG_SUBJ] [varchar](200) NULL,
	[LOG_MSG] [varchar](2000) NULL,
	[EMAIL_TYPE] [varchar](15) NULL,
 CONSTRAINT [PK_T_OE_SYS_EMAIL_LOG] PRIMARY KEY CLUSTERED  ([EMAIL_LOG_ID] ASC)
) ON [PRIMARY]

GO

