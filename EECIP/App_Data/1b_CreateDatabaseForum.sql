﻿use [EECIP]

IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = 'forum' )
BEGIN
    EXEC sp_executesql N'CREATE SCHEMA forum;';
END

GO

/****** Object:  Table [dbo].[Category]    Script Date: 12/14/2017 10:47:30 AM ******/
CREATE TABLE [forum].[Category](
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
 CONSTRAINT [forum.PK_Category] PRIMARY KEY CLUSTERED  ([Id] ASC)  ON [PRIMARY],
 FOREIGN KEY ([Category_Id]) references [forum].[Category] ([Id])

) ON [PRIMARY] 
GO


CREATE TABLE [forum].[Poll](
	[Id] [uniqueidentifier] NOT NULL,
	[IsClosed] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[ClosePollAfterDays] [int] NULL,
	[MembershipUser_Id] [int] NOT NULL,
 CONSTRAINT [forum.PK_Poll] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY],
 FOREIGN KEY ([MembershipUser_Id]) references [dbo].[T_OE_USERS] ([USER_IDX])

) ON [PRIMARY]
GO


CREATE TABLE [forum].[PollAnswer](
	[Id] [uniqueidentifier] NOT NULL,
	[Answer] [nvarchar](600) NOT NULL,
	[Poll_Id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [forum.PK_PollAnswer] PRIMARY KEY CLUSTERED ([Id] ASC),
 FOREIGN KEY ([Poll_Id]) references [forum].[Poll] ([Id])
) ON [PRIMARY]
GO


CREATE TABLE [forum].[PollVote](
	[Id] [uniqueidentifier] NOT NULL,
	[PollAnswer_Id] [uniqueidentifier] NOT NULL,
	[MembershipUser_Id] [int] NOT NULL,
 CONSTRAINT [forum.PollVote] PRIMARY KEY CLUSTERED ([Id] ASC),
 FOREIGN KEY ([MembershipUser_Id]) references [dbo].[T_OE_USERS] ([USER_IDX]),
 FOREIGN KEY ([PollAnswer_Id]) references [forum].[PollAnswer] ([Id])
) ON [PRIMARY]
GO


CREATE TABLE [forum].[Post](
	[Id] [uniqueidentifier] NOT NULL,
	[PostContent] [nvarchar](max) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[VoteCount] [int] NOT NULL,
	[DateEdited] [datetime] NOT NULL,
	[IsSolution] [bit] NOT NULL,
	[IsTopicStarter] [bit] NULL,
	[FlaggedAsSpam] [bit] NULL,
	[IpAddress] [nvarchar](50) NULL,
	[Pending] [bit] NULL,
	[SearchField] [nvarchar](max) NULL,
	[InReplyTo] [uniqueidentifier] NULL,
	[Topic_Id] [uniqueidentifier] NOT NULL,
	[MembershipUser_Id] [int] NOT NULL,
	[SYNC_IND] [bit] NOT NULL DEFAULT 0,
	[LAST_POST_DT] [datetime],
 CONSTRAINT [forum.PK_Post] PRIMARY KEY CLUSTERED (	[Id] ASC) ON [PRIMARY],
 FOREIGN KEY ([MembershipUser_Id]) references [dbo].[T_OE_USERS] ([USER_IDX])
) ON [PRIMARY] 
GO
--alter table [forum].[Post] add SYNC_IND [bit] NOT NULL DEFAULT 0;


CREATE TABLE [forum].[Topic](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Solved] [bit] NOT NULL,
	[SolvedReminderSent] [bit] NULL,
	[Slug] [nvarchar](450) NOT NULL,
	[Views] [int] NULL,
	[IsSticky] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[Pending] [bit] NULL,
	[Category_Id] [uniqueidentifier] NOT NULL,
	[Post_Id] [uniqueidentifier] NULL,
	[Poll_Id] [uniqueidentifier] NULL,
	[MembershipUser_Id] [int] NOT NULL,
	[SYNC_IND] [bit] NOT NULL DEFAULT 0,
	LAST_POST_DT [datetime],
 CONSTRAINT [forum.PK_Topic] PRIMARY KEY CLUSTERED ([Id] ASC),
 FOREIGN KEY ([Category_Id]) references [forum].[Category] ([Id]),
 FOREIGN KEY ([MembershipUser_Id]) references [dbo].[T_OE_USERS] ([USER_IDX]),
 FOREIGN KEY ([Poll_Id]) references [forum].[Poll] ([Id]),
 FOREIGN KEY ([Post_Id]) references [forum].[Post] ([Id])
) ON [PRIMARY]
GO

--alter table [forum].[Topic] add SYNC_IND [bit] NOT NULL DEFAULT 0;
--alter table [forum].[Topic] add LAST_POST_DT [datetime];
--update [forum].[Topic]
--    set LAST_POST_DT = (select top 1 DateEdited
--                  from [forum].[Post] c
--                  where c.Topic_Id = [forum].[Topic].Id
--                  order by [DateEdited] desc);

ALTER TABLE [forum].[Post]  WITH CHECK ADD  CONSTRAINT [forum.Topic_Post_FK] FOREIGN KEY([Topic_Id]) REFERENCES [forum].[Topic] ([Id]) 
GO




CREATE TABLE [forum].[Topic_Tags](
	[Topic_Id] [uniqueidentifier] NOT NULL,
	[TopicTagAttribute] [varchar](50) NOT NULL,
	[TopicTag] [varchar](100) NOT NULL,
 CONSTRAINT [forum.PK_Topic_Tag] PRIMARY KEY CLUSTERED ([Topic_Id], [TopicTagAttribute] ASC, [TopicTag] ASC),
 FOREIGN KEY ([Topic_Id]) references [forum].[Topic] ([Id]) ON DELETE CASCADE,
) ON [PRIMARY]
GO


CREATE TABLE [forum].[MembershipUserPoints](
	[Id] [uniqueidentifier] NOT NULL,
	[Points] [int] NOT NULL,
	[DateAdded] [datetime] NOT NULL,
	[PointsFor] [int] NOT NULL,
	[PointsForId] [uniqueidentifier] NULL,
	[Notes] [nvarchar](400) NULL,
	[MembershipUser_Id] [int] NOT NULL,
 CONSTRAINT [forum.PK_MembershipUserPoints] PRIMARY KEY CLUSTERED ([Id] ASC),
 FOREIGN KEY ([MembershipUser_Id]) references [dbo].[T_OE_USERS] ([USER_IDX]) ON DELETE CASCADE,
) ON [PRIMARY]
GO

--ALTER TABLE [forum].[MembershipUserPoints] ADD CONSTRAINT FK_MembershipUserPoints FOREIGN KEY ([MembershipUser_Id]) references [dbo].[T_OE_USERS] ([USER_IDX]) ON DELETE CASCADE;

CREATE TABLE [forum].[TopicNotification](
	[Id] [uniqueidentifier] NOT NULL,
	[Topic_Id] [uniqueidentifier] NOT NULL,
	[MembershipUser_Id] [int] NOT NULL,
 CONSTRAINT [forum.PK_TopicNotification] PRIMARY KEY CLUSTERED ([Id] ASC),
 FOREIGN KEY ([MembershipUser_Id]) references [dbo].[T_OE_USERS] ([USER_IDX]) ON DELETE CASCADE,
 FOREIGN KEY ([Topic_Id]) references [forum].[Topic] ([Id]) ON DELETE CASCADE
) ON [PRIMARY]
GO

--ALTER TABLE [forum].[TopicNotification] ADD CONSTRAINT FK_TopicNotification FOREIGN KEY ([MembershipUser_Id]) references [dbo].[T_OE_USERS] ([USER_IDX]) ON DELETE CASCADE;


CREATE TABLE [forum].[Vote](
	[Id] [uniqueidentifier] NOT NULL,
	[Amount] [int] NOT NULL,
	[DateVoted] [datetime] NULL,
	[Post_Id] [uniqueidentifier] NOT NULL,
	[MembershipUser_Id] [int] NOT NULL,
	[VotedByMembershipUser_Id] [int] NULL,
 CONSTRAINT [forum.PK_Vote] PRIMARY KEY CLUSTERED  ([Id] ASC ),
 FOREIGN KEY ([MembershipUser_Id]) references [dbo].[T_OE_USERS] ([USER_IDX]) ,
 FOREIGN KEY ([VotedByMembershipUser_Id]) references [dbo].[T_OE_USERS] ([USER_IDX]) ON DELETE CASCADE ,
 FOREIGN KEY ([Post_Id]) references [forum].[Post] ([Id]) ON DELETE CASCADE 
) ON [PRIMARY]
GO

--ALTER TABLE [forum].[Vote] ADD CONSTRAINT FK_VoteMembership FOREIGN KEY ([MembershipUser_Id]) references [dbo].[T_OE_USERS] ([USER_IDX]);
--ALTER TABLE [forum].[Vote] ADD CONSTRAINT FK_VoteMembershipBy FOREIGN KEY ([VotedByMembershipUser_Id]) references [dbo].[T_OE_USERS] ([USER_IDX]) ON DELETE CASCADE;


CREATE TABLE [forum].[Favourite](
	[Id] [uniqueidentifier] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[MemberId] [int] NOT NULL,
	[PostId] [uniqueidentifier] NOT NULL,
	[TopicId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [forum.PK_Favourite] PRIMARY KEY CLUSTERED ([Id] ASC),
 FOREIGN KEY ([MemberId]) references [dbo].[T_OE_USERS] ([USER_IDX]),
 FOREIGN KEY ([PostId]) references [forum].[Post] ([Id]) ON DELETE CASCADE,
 FOREIGN KEY ([TopicId]) references [forum].[Topic] ([Id]) ON DELETE CASCADE
) ON [PRIMARY]
GO


CREATE TABLE [forum].[Badge](
	[Id] [uniqueidentifier] NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[DisplayName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Image] [nvarchar](50) NULL,
	[AwardsPoints] [int] NULL,
	[ACT_IND] [bit] NOT NULL DEFAULT 1,
	[SORT_SEQ] [int] NOT NULL DEFAULT 1
 CONSTRAINT [forum.PK_Badge] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY] 
GO

--  alter table [forum].[Badge] add ACT_IND [bit] NOT NULL DEFAULT 1;
--  alter table [forum].[Badge] add SORT_SEQ [int] NOT NULL DEFAULT 1;


CREATE TABLE [forum].[MembershipUser_Badge](
	[MembershipUser_Id] [int] NOT NULL,
	[Badge_Id] [uniqueidentifier] NOT NULL,
	[DateEarned] [DateTime] NOT NULL,
 CONSTRAINT [forum.PK_MembershipUser_Badge] PRIMARY KEY CLUSTERED ([MembershipUser_Id] ASC,[Badge_Id] ASC),
 FOREIGN KEY ([Badge_Id]) references [forum].[Badge] ([Id]),
 FOREIGN KEY ([MembershipUser_Id]) references [dbo].[T_OE_USERS] ([USER_IDX]),

) ON [PRIMARY]
GO



--INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints]) VALUES (NewID(), N'Post', N'OneThousandPoints', N'Thousand Pointer', N'This badge is awarded to users who have received 1000 points', N'OneThousandPoints.png', 10)
--INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints]) VALUES (NewID(), N'VoteUp', N'PosterVoteUp', N'First Vote Up Received', N'This badge is awarded to users after they receive their first vote up from another user', N'PosterVoteUpBadge.png', 2)
--INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints]) VALUES (NewID(), N'VoteUp', N'UserVoteUp', N'You''ve Given Your First Vote Up', N'This badge is awarded to users after they make their first vote up', N'UserVoteUpBadge.png', 2)
--INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints]) VALUES (NewID(), N'VoteDown', N'TheGrouch', N'The Grouch', N'This badge is awarded to users who have voted down other users posts 10 or more times', N'TheGrouch.png', 0)
--INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints]) VALUES (NewID(), N'MarkAsSolution', N'PosterMarkAsSolution', N'Post Selected As Answer', N'This badge is awarded to a user when their post is marked as the solution to a topic', N'PosterMarkAsSolutionBadge.png', 2)
--INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints]) VALUES (NewID(), N'MarkAsSolution', N'AuthorMarkAsSolution', N'Your Question Solved', N'This badge is awarded to a user when they mark another users post as the solution to their topic', N'UserMarkAsSolutionBadge.png', 2)
--INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints]) VALUES (NewID(), N'MarkAsSolution', N'Padawan', N'Padawan', N'Had 10 or more posts successfully marked as an answer', N'padawan.png', 10)
--INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints]) VALUES (NewID(), N'MarkAsSolution', N'JediMaster', N'Jedi Master', N'Had 50 or more posts successfully marked as an answer', N'jedi.png', 50)
--INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints]) VALUES (NewID(), N'Favourite', N'FavouriteFirstPost', N'First Favourite', N'You have Favourited your first post', N'you-favourited-your-first-post.png', 1)
--INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints]) VALUES (NewID(), N'Favourite', N'YourPostFavourited', N'Favourited Post', N'You have one or more posts that have been Favourited by another member', N'your-post-got-favourited-by-another-member.png', 2)
--INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints]) VALUES (NewID(), N'Favourite', N'YourPostFavouritedTenTimes', N'Recognised Post', N'One or more of your posts have been Favourited by 10 or more people', N'recognised-post.png', 10)


INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints], ACT_IND, SORT_SEQ) VALUES (NewID(), N'Profile', N'UserProfile', N'Fill Out User Profile', N'This badge is awarded to users after they fill out their user profile', N'AddUserProfile.png', 10, 1, 1);
INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints], ACT_IND, SORT_SEQ) VALUES (NewID(), N'Profile', N'Photogenic', N'Photogenic', N'Little things like uploading a profile picture make the community a better place. Thanks!', N'photogenic.png', 10, 1, 2);
INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints], ACT_IND, SORT_SEQ) VALUES (NewID(), N'Profile', N'AgencyProfile', N'Fill Out Agency Profile', N'This badge is awarded to users after they update their agency''s profile', N'AddAgency.png', 10, 1, 3);
INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints], ACT_IND, SORT_SEQ) VALUES (NewID(), N'Profile', N'ProjectProfile', N'Add A Project', N'This badge is awarded to users after they enter a project for their agency', N'AddProject.png', 10, 1, 4);
INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints], ACT_IND, SORT_SEQ) VALUES (NewID(), N'Profile', N'ProjectDocument', N'Upload a project document', N'This badge is awarded to users after they upload supporting documentation for a project', N'AddDocument.png', 10, 1, 5);

INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints], ACT_IND, SORT_SEQ) VALUES (NewID(), N'Post', N'OneTopic', N'Create a Discussion', N'This badge is awarded to users who have posted a new topic to the discussion forum', N'OneTopic.png', 10, 1, 6);
INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints], ACT_IND, SORT_SEQ) VALUES (NewID(), N'VoteUp', N'UserVoteUp', N'Vote Up Your First Discussion Topic', N'This badge is awarded to users after they make their first vote up', N'OneLike.png', 10, 1, 7);
INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints], ACT_IND, SORT_SEQ) VALUES (NewID(), N'VoteUp', N'UserProjectLike', N'Like A Project', N'This badge is awarded to users after they like a project', N'OneLikeProject.png', 10, 1, 8);

INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints], ACT_IND, SORT_SEQ) VALUES (NewID(), N'Profile', N'ProjectProfileFive', N'Add 5 Projects', N'This badge is awarded to users after they enter 5 projects for their agency', N'AddProjectFive.png', 20, 1, 9);
INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints], ACT_IND, SORT_SEQ) VALUES (NewID(), N'Post', N'FiveTopics', N'Create 5th Discussion', N'This badge is awarded to users who have posted their fifth topic to the discussion forum', N'FiveTopic.png', 20, 1, 10);
INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints], ACT_IND, SORT_SEQ) VALUES (NewID(), N'VoteUp', N'UserVoteUpFive', N'Vote Up 5 Discussion Topics', N'This badge is awarded to users after they make their 5th vote up on a discussion topic', N'FiveLike.png', 20, 1, 11);
INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints], ACT_IND, SORT_SEQ) VALUES (NewID(), N'VoteUp', N'UserProjectLikeFive', N'Like 5 Projects', N'This badge is awarded to users after they like 5 projects', N'FiveLikeProject.png', 20, 1, 12);

INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints], ACT_IND, SORT_SEQ) VALUES (NewID(), N'Profile', N'ProjectProfileTen', N'Add 10 Projects', N'This badge is awarded to users after they enter 10 projects for their agency', N'AddProjectTen.png', 40, 1, 13);
INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints], ACT_IND, SORT_SEQ) VALUES (NewID(), N'VoteUp', N'UserVoteUpTen', N'Vote Up 10 Discussion Topics', N'This badge is awarded to users after they make their 10th vote up on a discussion topic', N'TenLike.png', 40, 1, 14);
INSERT [forum].[Badge] (Id, [Type], [Name], [DisplayName], [Description], [Image], [AwardsPoints], ACT_IND, SORT_SEQ) VALUES (NewID(), N'VoteUp', N'UserProjectLikeTen', N'Like 10 Projects', N'This badge is awarded to users after they like 10 projects', N'TenLikeProject.png', 40, 1, 15);

GO

CREATE TABLE [forum].[PostFile](
	[Id] [uniqueidentifier] NOT NULL,
	[Filename] [nvarchar](200) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[Post_Id] [uniqueidentifier] NULL,
	[FileContent] [varbinary](max) NULL,
	[FileDecription] [nvarchar](200) NOT NULL,
	[FileContentType] [nvarchar](75) NULL,
	[MembershipUser_Id] [int] NOT NULL,
 CONSTRAINT [forum.PK_UploadedFile] PRIMARY KEY CLUSTERED  ([Id] ASC),
 FOREIGN KEY ([Post_Id]) references [forum].[Post] ([Id]),
 FOREIGN KEY ([MembershipUser_Id]) references [dbo].[T_OE_USERS] ([USER_IDX])
) ON [PRIMARY]
GO



ALTER PROCEDURE [dbo].[SP_RECENT_FORUM_BY_USER_TAG] 
	@useridx int, 
	@daysSince int,
	@tagFilter varchar(100)
AS
BEGIN

	--DECLARE @useridx int; 
	--DECLARE @daysSince int;
	--DECLARE @fallBackAny varchar(1);
	--DECLARE @tagFilter varchar(100);
	
	--set @useridx =1;
	--set @daysSince = 3000;
	--set @fallBackAny = 'Y';
	--set @tagFilter = null;


	if (@tagFilter='') 
		SET @tagFilter = null

	if (@tagFilter is null)
	BEGIN
		select T.Id, T.Name, T.Slug, P.PostContent, U.FNAME, U.LNAME, U.USER_IDX as TopicUserIDX,
		(select max(DateCreated) from forum.Post P1 where P1.Topic_Id = T.Id) as LatestPostDate,
		(select U2.FNAME + ' ' + U2.LNAME from forum.Post P2, T_OE_USERS U2 where P2.MembershipUser_Id=U2.USER_IDX and P2.Topic_Id = T.Id and P2.DateCreated = (select max(DateCreated) from forum.Post P1 where P1.Topic_Id = T.Id)) as LatestPostUser,
		(select U2.USER_IDX from forum.Post P2, T_OE_USERS U2 where P2.MembershipUser_Id=U2.USER_IDX and P2.Topic_Id = T.Id and P2.DateCreated = (select max(DateCreated) from forum.Post P1 where P1.Topic_Id = T.Id)) as LatestPostUserIDX
		from forum.Topic T, forum.Post P , T_OE_USERS U
		where T.Id = P.Topic_Id
		and T.MembershipUser_Id = U.USER_IDX
		and P.IsTopicStarter = 1
		and (select max(DateCreated) from forum.Post P1 where P1.Topic_Id = T.Id) > GetDate()-@daysSince
		and (select count(*) from forum.Topic_Tags TT, T_OE_USER_EXPERTISE UE where TT.TopicTag=UE.EXPERTISE_TAG and UE.USER_IDX=@useridx and TT.Topic_Id = T.Id) > 0;
	END
	else
	BEGIN
		select T.Id, T.Name, T.Slug, P.PostContent, U.FNAME, U.LNAME, U.USER_IDX as TopicUserIDX,
		(select max(DateCreated) from forum.Post P1 where P1.Topic_Id = T.Id) as LatestPostDate,
		(select U2.FNAME + ' ' + U2.LNAME from forum.Post P2, T_OE_USERS U2 where P2.MembershipUser_Id=U2.USER_IDX and P2.Topic_Id = T.Id and P2.DateCreated = (select max(DateCreated) from forum.Post P1 where P1.Topic_Id = T.Id)) as LatestPostUser,
		(select U2.USER_IDX from forum.Post P2, T_OE_USERS U2 where P2.MembershipUser_Id=U2.USER_IDX and P2.Topic_Id = T.Id and P2.DateCreated = (select max(DateCreated) from forum.Post P1 where P1.Topic_Id = T.Id)) as LatestPostUserIDX
		from forum.Topic T, forum.Post P , T_OE_USERS U
		where T.Id = P.Topic_Id
		and T.MembershipUser_Id = U.USER_IDX
		and P.IsTopicStarter = 1
		and (select max(DateCreated) from forum.Post P1 where P1.Topic_Id = T.Id) > GetDate()-@daysSince
		and (select count(*) from forum.Topic_Tags TT, T_OE_USER_EXPERTISE UE where TT.TopicTag=UE.EXPERTISE_TAG and UE.USER_IDX=@useridx and TT.Topic_Id = T.Id and TT.TopicTag=@tagFilter) > 0;
	END
END


GO


  CREATE VIEW RPT_MON_PROJ_VOTE
  as
  select YEAR(V.DATE_VOTED) as YR, MONTH(DATE_VOTED) as MON, P.PROJECT_IDX,   
  (select top 1 O.ORG_NAME from T_OE_ORGANIZATION O, T_OE_PROJECT_ORGS PO where O.ORG_IDX=PO.ORG_IDX and PO.PROJECT_IDX = P.PROJECT_IDX) as ORG, 
  P.PROJ_NAME, count(*) as CNT
  from T_OE_PROJECT_VOTES V ,T_OE_PROJECTS P
  where V.PROJECT_IDX = P.PROJECT_IDX
  group by P.PROJECT_IDX, P.PROJ_NAME, YEAR(V.DATE_VOTED), MONTH(DATE_VOTED)
  --order by YEAR(V.DATE_VOTED), MONTH(DATE_VOTED);


  GO

  CREATE VIEW RPT_MON_TOPIC_VOTE 
  as
  select YEAR(V.DateVoted) as YR, MONTH(V.DateVoted) as MON, T.Name, count(*) as CNT
  from forum.Vote V, forum.Post P, forum.Topic T
  where V.Post_Id = P.Id
  and P.Topic_Id = T.Id
  group by T.Name, YEAR(V.DateVoted), MONTH(V.DateVoted)
  --order by YEAR(V.DateVoted), MONTH(V.DateVoted)

  GO
  
   CREATE VIEW USER_DEFINED_TAGS AS
  SELECT Z.PROJECT_TAG_NAME, Z.UserTagType, Z.CNT
  from 
  (select PROJECT_TAG_NAME, COUNT(*) as CNT, 'Project' as 'UserTagType'
  from [T_OE_PROJECT_TAGS] PT left join T_OE_REF_TAGS RT on PT.PROJECT_TAG_NAME=RT.TAG_NAME and RT.TAG_CAT_NAME='Tags'
  where RT.TAG_IDX is null
  group by PROJECT_TAG_NAME
  UNION ALL
  select [EXPERTISE_TAG], COUNT(*) as CNT, 'Expertise' as 'UserTagType'
  from [dbo].[T_OE_USER_EXPERTISE] PT left join T_OE_REF_TAGS RT on PT.[EXPERTISE_TAG]=RT.TAG_NAME and RT.TAG_CAT_NAME='Tags'
  where RT.TAG_IDX is null
  group by [EXPERTISE_TAG]
  UNION ALL
  select [TopicTag], COUNT(*)as CNT, 'Forum Topic'  as 'UserTagType'
  from [forum].[Topic_Tags] PT left join T_OE_REF_TAGS RT on PT.[TopicTag]=RT.TAG_NAME and RT.TAG_CAT_NAME='Tags'
  where RT.TAG_IDX is null
  group by [TopicTag]
  ) Z