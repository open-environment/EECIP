CREATE PROCEDURE [dbo].[SP_RECENT_FORUM_BY_USER_TAG] 
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

CREATE PROCEDURE [dbo].[SP_RECENT_FORUM_FALLBACK] 
	@daysSince int
AS
BEGIN

	--DECLARE @daysSince int;	
	--set @daysSince = 300;


	select T.Id, T.Name, T.Slug, P.PostContent, U.FNAME, U.LNAME, U.USER_IDX as TopicUserIDX,
	(select max(DateCreated) from forum.Post P1 where P1.Topic_Id = T.Id) as LatestPostDate,
	(select U2.FNAME + ' ' + U2.LNAME from forum.Post P2, T_OE_USERS U2 where P2.MembershipUser_Id=U2.USER_IDX and P2.Topic_Id = T.Id and P2.DateCreated = (select max(DateCreated) from forum.Post P1 where P1.Topic_Id = T.Id)) as LatestPostUser,
	(select U2.USER_IDX from forum.Post P2, T_OE_USERS U2 where P2.MembershipUser_Id=U2.USER_IDX and P2.Topic_Id = T.Id and P2.DateCreated = (select max(DateCreated) from forum.Post P1 where P1.Topic_Id = T.Id)) as LatestPostUserIDX
	from forum.Topic T, forum.Post P , T_OE_USERS U
	where T.Id = P.Topic_Id
	and T.MembershipUser_Id = U.USER_IDX
	and P.IsTopicStarter = 1
	and (select max(DateCreated) from forum.Post P1 where P1.Topic_Id = T.Id) > GetDate()-@daysSince;

END