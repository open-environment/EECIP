use [EECIP]

IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = 'forum' )
BEGIN
    EXEC sp_executesql N'CREATE SCHEMA forum;';
END

GO

/****** Object:  Table [dbo].[Category]    Script Date: 12/14/2017 10:47:30 AM ******/
CREATE TABLE [Forum].[Category](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[IsLocked] [bit] NOT NULL,
	[ModerateTopics] [bit] NOT NULL,
	[ModeratePosts] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[Slug] [nvarchar](450) NOT NULL,
	[PageTitle] [nvarchar](80) NULL,
	[Path] [nvarchar](2500) NULL,
	[MetaDescription] [nvarchar](200) NULL,
	[Colour] [nvarchar](50) NULL,
	[Image] [nvarchar](200) NULL,
	[Category_Id] [uniqueidentifier] NULL,
 CONSTRAINT [Forum.PK_Category] PRIMARY KEY CLUSTERED  ([Id] ASC)  ON [PRIMARY],
 FOREIGN KEY ([Category_Id]) references [Forum].[Category] ([Id])

) ON [PRIMARY] 
GO


CREATE TABLE [forum].[Poll](
	[Id] [uniqueidentifier] NOT NULL,
	[IsClosed] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[ClosePollAfterDays] [int] NULL,
	[MembershipUser_Id] [int] NOT NULL,
 CONSTRAINT [Forum.PK_Poll] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY],
 FOREIGN KEY ([MembershipUser_Id]) references [dbo].[T_OE_USERS] ([USER_IDX])

) ON [PRIMARY]
GO


