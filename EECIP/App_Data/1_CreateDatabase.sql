/***************************************************************** */
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
	[POP_DENSITY] [varchar](10) NULL,
 CONSTRAINT [PK_T_OE_REF_STATE] PRIMARY KEY CLUSTERED  ([STATE_CD] ASC)
) ON [PRIMARY]
GO
--ALTER TABLE [T_OE_REF_STATE]  add [POP_DENSITY] [varchar](10) NULL;



CREATE TABLE [dbo].[T_OE_REF_REGION](
	[EPA_REGION] [int] NOT NULL,
	[EPA_REGION_NAME] [varchar](100) NOT NULL,
 CONSTRAINT [PK_T_OE_REF_REGION] PRIMARY KEY CLUSTERED  ([EPA_REGION] ASC)
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[T_OE_REF_ORG_TYPE](
	[ORG_TYPE] [varchar](20) NOT NULL,
	[ORG_TYPE_DESC] [varchar](200) NULL,
	[SORT_SEQ] [int] NULL,
 CONSTRAINT [PK_T_OE_REF_ORG_TYPE] PRIMARY KEY CLUSTERED  ([ORG_TYPE] ASC)
) ON [PRIMARY]

GO

--ALTER TABLE [T_OE_REF_ORG_TYPE]  add [ORG_TYPE_DESC] [varchar](200) NULL;
--ALTER TABLE [T_OE_REF_ORG_TYPE]  add [SORT_SEQ] [int] NULL;


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
	[ACT_IND] bit NOT NULL DEFAULT 1,
 CONSTRAINT [PK_T_OE_REF_TG_CATEGORIES] PRIMARY KEY CLUSTERED  ([TAG_CAT_NAME] ASC)
) ON [PRIMARY]

GO
--ALTER TABLE [T_OE_REF_TAG_CATEGORIES]  add [ACT_IND] bit NOT NULL DEFAULT 1;

CREATE TABLE [dbo].[T_OE_REF_TAGS](
	[TAG_IDX] [int] IDENTITY(1,1) NOT NULL,
	[TAG_NAME] [varchar](100) NOT NULL,
	[TAG_DESC] [varchar](500) NULL,
	[TAG_CAT_NAME] [varchar](50) NOT NULL,
	[USER_CREATE_IND] [bit] NOT NULL DEFAULT 0,
	[PROMOTE_IND] [bit] NOT NULL DEFAULT 0,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_REF_TAGS] PRIMARY KEY CLUSTERED  ([TAG_IDX] ASC),
 FOREIGN KEY ([TAG_CAT_NAME]) references [T_OE_REF_TAG_CATEGORIES] ([TAG_CAT_NAME])
) ON [PRIMARY]

GO
--ALTER TABLE [T_OE_REF_TAGS]  add [PROMOTE_IND] [bit] NOT NULL DEFAULT 0;
--ALTER TABLE [T_OE_REF_TAGS]  add [TAG_DESC] [varchar](500) NULL;
--GO


CREATE TABLE [dbo].[T_OE_REF_EMAIL_TEMPLATE](
	[EMAIL_TEMP_ID] [varchar](50) NOT NULL,
	[EMAIL_TEMP_NAME] [varchar](100) NULL,
	[EMAIL_TEMP_DESC] [varchar](250) NULL,
	[EMAIL_TEMP_SUBJECT] [varchar](200) NULL,
	[EMAIL_TEMP_BODY_HTML] [varchar](max) NULL,
	[EMAIL_TEMP_BODY_TXT] [varchar](max) NULL,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
	[MODIFY_USERIDX] [int] NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_REF_EMAIL_TEMP] PRIMARY KEY CLUSTERED  ([EMAIL_TEMP_ID] ASC)
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
	[ORG_TYPE] [varchar](20) NULL,
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

--ALTER TABLE [T_OE_ORGANIZATION]  add [ORG_TYPE] [varchar](20) NULL;


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
	[PROJECT_CONTACT]  [varchar](100) NULL,
	[ACTIVE_INTEREST_IND] [bit] NULL,
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
--  alter table T_OE_ORGANIZATION_ENT_SVCS add PROJECT_CONTACT [varchar](100) NULL;
--  alter table T_OE_ORGANIZATION_ENT_SVCS add ACTIVE_INTEREST_IND [bit] NULL;



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
    [ALLOW_GOVERNANCE] [bit] NOT NULL DEFAULT 0,
	[ORG_IDX] [uniqueidentifier] NULL,
	[SYNC_IND] [bit] NOT NULL DEFAULT 0,
	[ACT_IND] [bit] NOT NULL,
	[EXCLUDE_POINTS_IND] [bit] NOT NULL DEFAULT 0,
	[NOTIFY_DISCUSSION_IND] [BIT] NOT NULL DEFAULT 1,
	[NOTIFY_BADGE_IND] [BIT] NOT NULL DEFAULT 1,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
	[MODIFY_USERIDX] [int] NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_USERS] PRIMARY KEY CLUSTERED ( [USER_IDX] ASC),
 FOREIGN KEY ([ORG_IDX]) references T_OE_ORGANIZATION ([ORG_IDX]), 
) ON [PRIMARY]

--  alter table [T_OE_USERS] add SYNC_IND [bit] NOT NULL DEFAULT 0;
--  alter table [T_OE_USERS] add LINKEDIN [varchar](100) NULL;
--ALTER TABLE [T_OE_USERS]  add [ALLOW_GOVERNANCE] [bit] NOT NULL DEFAULT 0;
--ALTER TABLE [T_OE_USERS] ADD [EXCLUDE_POINTS_IND] [BIT] NOT NULL DEFAULT 0;
--ALTER TABLE [T_OE_USERS] ADD [NOTIFY_DISCUSSION_IND] [BIT] NOT NULL DEFAULT 1;
--ALTER TABLE [T_OE_USERS] ADD [NOTIFY_BADGE_IND] [BIT] NOT NULL DEFAULT 1;


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
	[NOTIFY_DESC] [varchar](2000) NULL,
	[READ_IND] [bit] NULL,
	[FROM_USER_IDX] [int] NULL,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
	[MODIFY_USERIDX] [int] NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_EM_USER_NOTIFICATION] PRIMARY KEY CLUSTERED ([NOTIFICATION_IDX] ASC )
) ON [PRIMARY]
GO
--  alter table T_OE_USER_NOTIFICATION alter column [NOTIFY_DESC] varchar(2000) NULL;


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
	[PROJECT_CONTACT_IDX] [int] NULL,
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
--  alter table T_OE_PROJECTS add [PROJECT_CONTACT_IDX] [int] NULL;


CREATE TABLE [dbo].[T_OE_PROJECT_TAGS](
	[PROJECT_IDX] [uniqueidentifier] NOT NULL DEFAULT newid(),
	[PROJECT_ATTRIBUTE] [varchar](50) NOT NULL,
	[PROJECT_TAG_NAME] [varchar](100) NOT NULL,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_PROJECT_TAGS] PRIMARY KEY CLUSTERED ([PROJECT_IDX], [PROJECT_ATTRIBUTE], [PROJECT_TAG_NAME] ASC),
 FOREIGN KEY ([PROJECT_IDX]) references [T_OE_PROJECTS] ([PROJECT_IDX]) ON UPDATE CASCADE ON DELETE CASCADE,
) ON [PRIMARY]

GO	 

--ALTER TABLE [dbo].[T_OE_PROJECT_TAGS] ADD CONSTRAINT FK_ProjectTag_Project FOREIGN KEY ([PROJECT_IDX]) references [dbo].[T_OE_PROJECTS] ([PROJECT_IDX]) ON UPDATE CASCADE ON DELETE CASCADE;

CREATE TABLE [dbo].[T_OE_PROJECT_URLS](
	[PROJECT_URL_IDX] [uniqueidentifier] NOT NULL DEFAULT newid(),
	[PROJECT_IDX] [uniqueidentifier] NULL,	
	[PROJECT_URL] [varchar](200) NULL,
	[PROJ_URL_DESC] [varchar](max) NULL,		
 CONSTRAINT [PK_T_OE_PROJECTS_URL] PRIMARY KEY CLUSTERED  ([PROJECT_URL_IDX] ASC),
 FOREIGN KEY ([PROJECT_IDX]) references [T_OE_PROJECTS] ([PROJECT_IDX]) ON UPDATE CASCADE ON DELETE CASCADE,
) ON [PRIMARY]

GO	 


CREATE TABLE [dbo].[T_OE_PROJECT_VOTES](
	[PROJECT_VOTE_IDX] [uniqueidentifier] NOT NULL,
	[VOTE_AMOUNT] [int] NOT NULL,
	[DATE_VOTED] [datetime] NULL,
	[PROJECT_IDX] [uniqueidentifier] NULL,
	[ORG_ENT_SVCS_IDX] [int] NULL,
	[VOTED_BY_USER_IDX] [int] NULL,
 CONSTRAINT [PK_T_OE_PROJECT_VOTES] PRIMARY KEY CLUSTERED  ([PROJECT_VOTE_IDX] ASC ),
 FOREIGN KEY ([VOTED_BY_USER_IDX]) references [dbo].[T_OE_USERS] ([USER_IDX]),
 FOREIGN KEY ([PROJECT_IDX]) references [dbo].[T_OE_PROJECTS] ([PROJECT_IDX]) ON DELETE CASCADE,
 FOREIGN KEY ([ORG_ENT_SVCS_IDX]) references [dbo].[T_OE_ORGANIZATION_ENT_SVCS] ([ORG_ENT_SVCS_IDX])
) ON [PRIMARY]
GO

--ALTER TABLE [dbo].[T_OE_PROJECT_VOTES] ADD CONSTRAINT FK_ProjectVote_Project FOREIGN KEY ([PROJECT_IDX]) references [dbo].[T_OE_PROJECTS] ([PROJECT_IDX]) ON UPDATE CASCADE ON DELETE CASCADE;


CREATE TABLE [dbo].[T_OE_DOCUMENTS](
	[DOC_IDX] [uniqueidentifier] NOT NULL,
	[DOC_CONTENT] [varbinary](max) NULL,
	[DOC_NAME] [varchar](100) NULL,
	[DOC_TYPE] [varchar](50) NULL,
	[DOC_FILE_TYPE] [varchar](75) NULL,
	[DOC_SIZE] [int] NULL,
	[DOC_COMMENT] [varchar](200) NULL,
	[DOC_AUTHOR] [varchar](100) NULL,
	[PROJECT_IDX] [uniqueidentifier] NULL,
	[ORG_ENT_SVCS_IDX] [int] NULL,
	[CREATE_USERIDX] [int] NULL,
	[CREATE_DT] [datetime2](0) NULL,
	[MODIFY_USERIDX] [int] NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_OE_DOCUMENTS] PRIMARY KEY CLUSTERED  ([DOC_IDX] ASC),
 FOREIGN KEY ([PROJECT_IDX]) references [T_OE_PROJECTS] ([PROJECT_IDX]) ON DELETE CASCADE,
 FOREIGN KEY ([ORG_ENT_SVCS_IDX]) references [T_OE_ORGANIZATION_ENT_SVCS]([ORG_ENT_SVCS_IDX]) ON DELETE CASCADE
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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

CREATE TABLE [dbo].[T_OE_APP_SETTINGS_CUSTOM](
	[SETTING_CUSTOM_IDX] [int] IDENTITY(1,1) NOT NULL,
	[TERMS_AND_CONDITIONS] [varchar](max) NULL,
	[ANNOUNCEMENTS] [varchar](max) NULL,
 CONSTRAINT [PK_T_OE_APP_SETTINGS_CUSTOM] PRIMARY KEY CLUSTERED ([SETTING_CUSTOM_IDX] ASC)
) ON [PRIMARY]
GO
--  alter table [T_OE_APP_SETTINGS_CUSTOM] add [ANNOUNCEMENTS] [varchar](max) NULL;

CREATE TABLE [dbo].[T_OE_SYS_SEARCH_LOG](
	[SYS_SEARCH_LOG_ID] [int] IDENTITY(1,1) NOT NULL,
	[LOG_DT] [datetime2](0) NOT NULL,
	[LOG_USERIDX] [int] NULL,
	[LOG_TERM] [varchar](1000) NULL,
 CONSTRAINT [PK_T_OE_SYS_SEARCH_LOG] PRIMARY KEY CLUSTERED  ([SYS_SEARCH_LOG_ID] ASC)
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[T_OE_RPT_FRESHNESS](
	[YR] [int] NOT NULL,
	[MON] [int] NOT NULL,	
	[CAT] [int] NOT NULL,
	[COUNT] [int] NULL,		
 CONSTRAINT [PK_T_OE_RPT_FRESHNESS] PRIMARY KEY CLUSTERED  ([YR], [MON], [CAT])
) ON [PRIMARY]

GO

CREATE PROCEDURE SP_ENT_SVC_COUNT_DISPLAY
AS
BEGIN
	select 
	a.ENT_PLATFORM_IDX, max(a.ENT_PLATFORM_NAME) as ENT_PLATFORM_NAME, max(a.ENT_PLATFORM_DESC) as ENT_PLATFORM_DESC, max(a.ENT_PLATFORM_EXAMPLE) as ENT_PLATFORM_EXAMPLE, count(*) as CNT
	from [T_OE_REF_ENTERPRISE_PLATFORM] a
	left join T_OE_ORGANIZATION_ENT_SVCS b on a.ENT_PLATFORM_IDX = b.ENT_PLATFORM_IDX and (b.ACTIVE_INTEREST_IND=1 or b.IMPLEMENT_STATUS <> 'Not under consideration')
	group by a.ENT_PLATFORM_IDX
	order by CNT desc;
END
GO


CREATE PROCEDURE SP_PROJECT_CREATE_COUNT
AS
BEGIN

	DECLARE @StartDate SMALLDATETIME, @EndDate SMALLDATETIME;

	SELECT @StartDate = '20180101'
	select @EndDate = GetDate();

	;WITH d(d) AS 
	(
	--  SELECT DATEADD(MONTH, n, DATEADD(MONTH, DATEDIFF(MONTH, 0, @StartDate), 0))
	--  FROM ( SELECT TOP (DATEDIFF(WK, @StartDate, @EndDate) + 1) 
	  SELECT DATEADD(MONTH, n, DATEADD(MONTH, DATEDIFF(MONTH, 0, @StartDate), 0))
	  FROM ( SELECT TOP (DATEDIFF(MONTH, @StartDate, @EndDate) + 1) 
		n = ROW_NUMBER() OVER (ORDER BY [object_id]) - 1
		FROM sys.all_objects ORDER BY [object_id] ) AS n
	)
	SELECT 
	  [Month]    = MONTH(d.d), 
	--  [Week]    = DATENAME(WK, d.d), 
	  [Year]     = YEAR(d.d), 
	  OrderCount = COUNT(o.PROJECT_IDX) 
	FROM d LEFT OUTER JOIN dbo.T_OE_PROJECTS AS o
	  ON o.CREATE_DT >= d.d
	--  AND o.CREATE_DT < DATEADD(WK, 2, d.d)
	  AND o.CREATE_DT < DATEADD(MONTH, 1, d.d)
	GROUP BY d.d
	ORDER BY d.d;

END
GO




CREATE PROCEDURE SP_NEW_CONTENT_USER_AGE
AS
BEGIN
	select ZZZ.UserAge, SUM(CNT) as CNT
	from
	(select 
	case when U.CREATE_DT > getdate()-30 then 3
	when U.CREATE_DT > getdate()-182 and U.CREATE_DT <= getdate()-30 then 2
	else 1
	end as UserAge
	,count(*) as CNT
	from T_OE_PROJECTS P, T_OE_USERS U
	where P.CREATE_USERIDX = U.USER_IDX
	and P.CREATE_DT > getdate()-140
	group by 
	case when U.CREATE_DT > getdate()-30 then 3
	when U.CREATE_DT > getdate()-182 and U.CREATE_DT <= getdate()-30 then 2
	else 1 end
	UNION ALL
	select 
	case when U.CREATE_DT > getdate()-30 then 3
	when U.CREATE_DT > getdate()-182 and U.CREATE_DT <= getdate()-30 then 2
	else 1
	end as UserAge
	,count(*) as CNT
	from forum.Post P, T_OE_USERS U
	where p.MembershipUser_Id = U.USER_IDX
	and P.DateCreated > getdate()-140
	group by 
	case when U.CREATE_DT > getdate()-30 then 3
	when U.CREATE_DT > getdate()-182 and U.CREATE_DT <= getdate()-30 then 2
	else 1 end) ZZZ
	group by userAge;
END
GO


CREATE PROCEDURE SP_DISCUSSION_CREATE_COUNT
AS
BEGIN

	DECLARE @StartDate SMALLDATETIME, @EndDate SMALLDATETIME;

	SELECT @StartDate = '20180101'
	select @EndDate = GetDate();

	;WITH d(d) AS 
	(
	--  SELECT DATEADD(MONTH, n, DATEADD(MONTH, DATEDIFF(MONTH, 0, @StartDate), 0))
	--  FROM ( SELECT TOP (DATEDIFF(WK, @StartDate, @EndDate) + 1) 
	  SELECT DATEADD(MONTH, n, DATEADD(MONTH, DATEDIFF(MONTH, 0, @StartDate), 0))
	  FROM ( SELECT TOP (DATEDIFF(MONTH, @StartDate, @EndDate) + 1) 
		n = ROW_NUMBER() OVER (ORDER BY [object_id]) - 1
		FROM sys.all_objects ORDER BY [object_id] ) AS n
	)
	SELECT 
	  [Month]    = MONTH(d.d), 
	--  [Week]    = DATENAME(WK, d.d), 
	  [Year]     = YEAR(d.d), 
	  OrderCount = COUNT(o.Id) 
	FROM d LEFT OUTER JOIN forum.Topic AS o
	  ON o.CreateDate >= d.d
	--  AND o.CREATE_DT < DATEADD(WK, 2, d.d)
	  AND o.CreateDate < DATEADD(MONTH, 1, d.d)
	GROUP BY d.d
	ORDER BY d.d;

END
GO


CREATE PROCEDURE SP_RPT_FRESHNESS_RECORD
AS
BEGIN

	declare @iCount int;
	delete from [T_OE_RPT_FRESHNESS] where YR = year(getdate()) and MON = month(getdate());

	select @iCount = count(*) from T_OE_PROJECTS

	insert into [T_OE_RPT_FRESHNESS](YR,MON,CAT,COUNT)
	select year(getdate()), month(getdate()), 
	case when datediff(day,coalesce(MODIFY_DT, create_dt),GetDate())<=31 then 1
	when  datediff(day,coalesce(MODIFY_DT, create_dt),GetDate()) between 31 and 182 then 2
	when  datediff(day,coalesce(MODIFY_DT, create_dt),GetDate()) between 182 and 365 then 3
	else 4 end
	, (count(*) *100) / @iCount
	 from T_OE_PROJECTS
	 group by
	case when datediff(day,coalesce(MODIFY_DT, create_dt),GetDate())<=31 then 1
	when  datediff(day,coalesce(MODIFY_DT, create_dt),GetDate()) between 31 and 182 then 2
	when  datediff(day,coalesce(MODIFY_DT, create_dt),GetDate()) between 182 and 365 then 3
	else 4 end;
END
GO


