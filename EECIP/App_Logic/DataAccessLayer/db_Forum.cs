using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using EECIP.App_Logic.BusinessLogicLayer;
using System.Data.Entity;
using EECIP.Models;
using System.ComponentModel.DataAnnotations;

namespace EECIP.App_Logic.DataAccessLayer
{
    public class CategoryDisplay {
        public Category _category { get; set; }
        public string LatestTopicName { get; set; }
        public string LatestTopicUserName { get; set; }
        public string LatestTopicDate { get; set; }
        public int LatestTopicUserIDX { get; set; }
        public int PostCount { get; set; }
        public int TopicCount { get; set; }
    }
    public class CategoryWithSubCategories
    {
        public Category Category { get; set; }
        public List<Category> SubCategories { get; set; }
    }

    public class TopicOverviewDisplay
    {
        public Topic _topic { get; set; }
        public string topicCreator { get; set; }
        public int postCount { get; set; }
        public int upVoteCount { get; set; }
        public int downVoteCount { get; set; }
        public int Views { get; set; }
        public string LastUpdateDatePretty { get; set; }
        public int? LastPostUserIDX { get; set; }
        public string LastPostUserName { get; set; }
        public string PostContent { get; set; }
    }

    public class UserBadgeDisplay {
        public Badge _Badge { get; set; }
        public bool UserEarnedInd { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EarnedDate { get; set; }
    }

    public class db_Forum
    {

        //****************************** BADGES  **********************************************
        public static List<UserBadgeDisplay> GetBadgesForUser(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Badges
                            join b in ctx.MembershipUser_Badge on a.Id equals b.Badge_Id
                                into sr
                            from x in sr.DefaultIfEmpty()  //left join
                            where (x == null ? true : x.MembershipUser_Id == UserIDX)
                            select new UserBadgeDisplay
                            {
                                _Badge = a,
                                UserEarnedInd = (x != null),
                                EarnedDate = x.DateEarned
                            }).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<Badge> GetBadges()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Badges
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Guid? InsertUpdatetBadge(Guid? id, string type, string name, string displayName, string description, string image, int? awardsPoints)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    Badge e = (from c in ctx.Badges
                               where c.Id == id 
                               select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new Badge();
                    }


                    if (type != null) e.Type = type;
                    if (name != null) e.Name = name;
                    if (displayName != null) e.DisplayName = displayName;
                    if (description != null) e.Description = description;
                    if (image != null) e.Image = image;
                    if (awardsPoints != null) e.AwardsPoints = awardsPoints;

                    if (insInd)
                        ctx.Badges.Add(e);

                    ctx.SaveChanges();
                    return e.Id;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }



        //****************************** CATEGORIES **********************************************
        public static List<Category> GetCategory()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Categories
                            orderby a.SortOrder
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<CategoryDisplay> GetCategoriesMain()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.Categories
                            where a.Category_Id == null
                            orderby a.SortOrder, a.DateCreated
                            select new CategoryDisplay {
                                _category = a
                            }).ToList();

                    foreach (CategoryDisplay c in xxx)
                    {
                        Topic latestTopic = GetTopicLatestByCategory(c._category.Id);
                        if (latestTopic != null)
                        {
                            c.LatestTopicName = latestTopic.Name;
                            c.LatestTopicUserIDX = latestTopic.MembershipUser_Id;
                            c.LatestTopicDate = Utils.GetPrettyDate(latestTopic.CreateDate.ToString());
                            T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(c.LatestTopicUserIDX);
                            if (u != null)
                                c.LatestTopicUserName = u.FNAME + " " + u.LNAME;

                            c.TopicCount = GetTopicCountByCategory(c._category.Id);
                            c.PostCount = GetPostCountByCategory(c._category.Id);
                        }
                    }

                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Category GetCategoryByID(Guid id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Categories.AsNoTracking()
                            where a.Id == id
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int InsertUpdateCategory(vmForumAdminCategory model)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    Category e = (from c in ctx.Categories
                                  where c.Id == model.Id
                                  select c).FirstOrDefault();

                    //set ID
                    if (e == null)
                    {
                        e = new Category();
                        insInd = true;
                        e.Id = Guid.NewGuid();
                        e.DateCreated = System.DateTime.UtcNow;
                    }

                    e.Name = HttpUtility.HtmlDecode(Utils.SafePlainText(model.Name));  //sanitize 
                    e.Description = Utils.GetSafeHtml(model.Description);   //sanitize
                    e.IsLocked = model.IsLocked;
                    e.ModerateTopics = model.ModerateTopics;
                    e.ModeratePosts = model.ModeratePosts;
                    e.SortOrder = model.SortOrder;
                    e.Slug = Utils.CreateUrl(model.Name, "-");    // url slug generator
                    e.PageTitle = model.PageTitle;
                    if (e.PageTitle == null)
                        e.PageTitle = e.Name;  //set page title to name if not specified
                    e.MetaDescription = model.MetaDesc;
                    e.Colour = model.CategoryColour;
                    e.Image = model.Image;
                    e.Category_Id = model.ParentCategory;

                    if (insInd)
                        ctx.Categories.Add(e);

                    ctx.SaveChanges();
                    return 1;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static CategoryWithSubCategories GetCategory_fromSlug(string slug)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                slug = Utils.SafePlainText(slug);

                try
                {
                    return (from a in ctx.Categories.AsNoTracking()
                            where a.Slug == slug
                            select new CategoryWithSubCategories {
                                Category = a,
                                SubCategories = (from cats in ctx.Categories
                                                 where cats.Category_Id == a.Id
                                                 select cats).ToList()
                            }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        //********************************** TOPICS *************************************************
        public static Topic InsertUpdateTopic(vmForumTopicCreate model, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    Topic e = (from c in ctx.Topics
                                  where c.Id == model.Id
                                  select c).FirstOrDefault();

                    //insert case
                    if (e == null)
                    {
                        insInd = true;
                        e = new Topic
                        {
                            Id = Guid.NewGuid(),
                            CreateDate = System.DateTime.UtcNow,
                            Views = 0
                        };
                    }

                    e.Name = HttpUtility.HtmlDecode(Utils.SafePlainText(model.Name));  //sanitize 
                    e.Solved = false;
                    e.SolvedReminderSent = null;
                    e.Slug = GetUniqueTopicSlug(model.Name);    // url slug generator
                    //e.Slug = Utils.CreateUrl(model.Name, "-");    // url slug generator
                    e.IsSticky = model.IsSticky;
                    e.IsLocked = model.IsLocked;
                    e.Pending = null;
                    e.Category_Id = model.Category;
                    e.Post_Id = null;
                    e.Poll_Id = null;
                    e.MembershipUser_Id = UserIDX;

                    if (insInd)
                        ctx.Topics.Add(e);

                    ctx.SaveChanges();
                    return e;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static string GetUniqueTopicSlug(string topicName)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    string topicCleaned = Utils.CreateUrl(topicName, "-");

                    int iCount = (from a in ctx.Topics.AsNoTracking()
                                  where a.Slug == topicCleaned
                                  select a).ToList().Count();

                    if (iCount == 0)
                        return topicCleaned;
                    else
                        return string.Concat(topicCleaned, "-", iCount);
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return topicName;
                }
            }
        }

        public static Topic GetTopic_fromSlug(string slug)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Topics.AsNoTracking()
                                  where a.Slug == slug
                                  select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Guid? InsertUpdateTopic_withPost(Guid topicID, Guid postID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Topic e = (from c in ctx.Topics
                               where c.Id == topicID
                               select c).FirstOrDefault();

                    e.Post_Id = postID;

                    ctx.SaveChanges();
                    return e.Id;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static bool PassedTopicFloodTest(string topicTitle, int UserIDX)
        {
            topicTitle = Utils.SafePlainText(topicTitle);
            var floodWindow = System.DateTime.UtcNow.AddMinutes(-2);

            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.Topics.AsNoTracking()
                            where a.Name == topicTitle
                            && a.CreateDate >= floodWindow
                            select a).Count();

                    return (xxx == 0);
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return false;
                }
            }


        }

        public static string TopicURL(string slug)
        {
            return VirtualPathUtility.ToAbsolute($"~/thread/{HttpUtility.UrlEncode(HttpUtility.HtmlDecode(slug))}/");
        }

        public static bool TopicAddView(Guid topicID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Topic e = (from c in ctx.Topics
                               where c.Id == topicID
                               select c).FirstOrDefault();

                    if (e != null)
                    {
                        e.Views = e.Views + 1;
                        ctx.SaveChanges();
                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return false;
                }
            }


        }

        public static string GetTopic_LastPostPrettyDate(Guid topicID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.Posts.AsNoTracking()
                               where a.Topic_Id == topicID
                               orderby a.DateCreated descending
                               select a).FirstOrDefault();

                    if (xxx != null)
                    {
                        string d = xxx.DateCreated.ToString();
                        return Utils.GetPrettyDate(d);
                    }
                    return "";
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return "";
                }
            }
        }

        public static Topic GetTopicLatestByCategory(Guid cat_id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {

                    return (from a in ctx.Topics.AsNoTracking()
                                  where a.Category_Id == cat_id
                                  orderby a.CreateDate descending
                                  select a).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int GetTopicCountByCategory(Guid cat_id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {

                    return (from a in ctx.Topics.AsNoTracking()
                            where a.Category_Id == cat_id
                            select a).Count();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static List<TopicOverviewDisplay> GetTopicListOverviewByCategory(Guid cat_id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {

                    return (from a in ctx.Topics.AsNoTracking()
                            join b in ctx.Posts on a.Id equals b.Topic_Id
                            where a.Category_Id == cat_id
                            && b.IsTopicStarter == true
                            orderby a.CreateDate descending
                            select new TopicOverviewDisplay {
                                _topic = a,
                                PostContent = b.PostContent
                            }).ToList();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }


        //************************************TOPIC NOTIFICATION **************************************************
        public static Guid? InsertUpdateTopicNotification(Guid? id, Guid topic_id, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    //get from ID
                    TopicNotification e = (from c in ctx.TopicNotifications
                                           where c.Id == id
                                           select c).FirstOrDefault();

                    //try get from topic id and user
                    if (e != null)
                        e = (from c in ctx.TopicNotifications
                             where c.Topic_Id == topic_id
                             && c.MembershipUser_Id == UserIDX
                             select c).FirstOrDefault();

                    if (e != null)
                        return e.Id;


                    //insert case
                    e = new TopicNotification();
                    e.Id = Guid.NewGuid();
                    e.Topic_Id = topic_id;
                    e.MembershipUser_Id = UserIDX;

                    ctx.TopicNotifications.Add(e);
                    ctx.SaveChanges();
                    return e.Id;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static bool NotificationIsUserSubscribed(Guid topic_id, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.TopicNotifications.AsNoTracking()
                               where a.Topic_Id == topic_id
                               && a.MembershipUser_Id == UserIDX
                               select a).Count();

                    return (xxx > 0);
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return false;
                }
            }
        }




        //************************************POST **************************************************
        public static Guid? InsertUpdatePost(Guid? id, string postContent, int? voteCount, bool? isSolution, bool? isTopicStarter, bool? flaggedAsSpam, bool? pending, 
            string searchField, Guid? inReplyTo, Guid? topicId, int? UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    Post e = (from c in ctx.Posts
                               where c.Id == id
                               select c).FirstOrDefault();

                    //insert case
                    if (e == null)
                    {
                        e = new Post();
                        insInd = true;
                        e.Id = Guid.NewGuid();
                        e.DateCreated = System.DateTime.UtcNow;
                        e.VoteCount = 0;
                        e.IpAddress = Utils.GetUsersIpAddress();

                        if (topicId == null || UserIDX == null)  //error if this info not supplied on create case
                            return null;

                        e.Topic_Id = topicId ?? Guid.Empty;
                        e.MembershipUser_Id = UserIDX ?? 0;
                    }


                    e.PostContent = Utils.GetSafeHtml(postContent);   //sanitize
                    if (voteCount != null) e.VoteCount = voteCount ?? 0;
                    e.DateEdited = System.DateTime.UtcNow;
                    if (isSolution != null) e.IsSolution = isSolution ?? false;
                    if (isTopicStarter != null) e.IsTopicStarter = isTopicStarter;
                    if (flaggedAsSpam != null) e.FlaggedAsSpam = flaggedAsSpam;
                    if (pending != null) e.Pending = pending;
                    if (searchField != null) e.SearchField = searchField;
                    if (inReplyTo != null) e.InReplyTo = inReplyTo;

                    if (insInd)
                        ctx.Posts.Add(e);

                    ctx.SaveChanges();
                    return e.Id;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static vmForumPost GetPost_StarterForTopic(Guid topic_id, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.Posts.AsNoTracking()
                               join b in ctx.Topics on a.Topic_Id equals b.Id
                            where a.Topic_Id == topic_id
                            && a.IsTopicStarter == true
                            select new vmForumPost {
                                Post = a,
                                ParentTopic = b,
                                UpVoteCount = (from v1 in ctx.Votes where v1.Post_Id == a.Id && v1.Amount > 0 select v1).Count(),
                                DownVoteCount = (from v1 in ctx.Votes where v1.Post_Id == a.Id && v1.Amount < 0 select v1).Count(),
                                UserIDX = UserIDX
                            }).FirstOrDefault();


                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<vmForumPost> GetPost_NonStarterForTopic(Guid topic_id, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.Posts.AsNoTracking()
                               where a.Topic_Id == topic_id
                               && a.IsTopicStarter == false
                               select new vmForumPost
                               {
                                   Post = a,
                                   UpVoteCount = (from v1 in ctx.Votes where v1.Post_Id == a.Id && v1.Amount > 0 select v1).Count(),
                                   DownVoteCount = (from v1 in ctx.Votes where v1.Post_Id == a.Id && v1.Amount < 0 select v1).Count(),
                                   UserIDX = UserIDX
                               }).ToList();


                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int GetPostCountByCategory(Guid cat_id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {

                    return (from a in ctx.Topics.AsNoTracking()
                            join b in ctx.Posts.AsNoTracking() on a.Id equals b.Topic_Id
                            where a.Category_Id == cat_id
                            select b).Count();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }



        //*************************************MEMBERSHIP USER POINTS*********************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="points"></param>
        /// <param name="dateAdded"></param>
        /// <param name="pointsFor">Post=0, Vote=1, Solution=2, Badge=3, Tag=4, Spam=5, Profile=6, Manual=7</param>
        /// <param name="pointsForId"></param>
        /// <param name="notes"></param>
        /// <param name="UserIDX"></param>
        /// <returns></returns>
        public static Guid? InsertUpdateMembershipUserPoints(Guid? id, int? points, DateTime? dateAdded, int pointsFor, Guid? pointsForId, string notes, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;                

                    //if the points have already been awarded, return
                    MembershipUserPoint e = (from c in ctx.MembershipUserPoints
                                             where c.PointsFor == pointsFor
                                             && c.PointsForId == pointsForId
                                             && c.MembershipUser_Id == UserIDX
                                             select c).FirstOrDefault();

                    if (e != null)
                        return e.Id;


                    e = (from c in ctx.MembershipUserPoints
                               where c.Id == id
                               select c).FirstOrDefault();


                    //insert case
                    if (e == null)
                    {
                        insInd = true;
                        e = new MembershipUserPoint();
                        e.Id = Guid.NewGuid();
                        e.DateAdded = DateTime.UtcNow;
                    }

                    e.Points = points ?? 1;
                    e.PointsFor = pointsFor;
                    e.PointsForId = pointsForId;
                    e.Notes = notes;
                    e.MembershipUser_Id = UserIDX;
                 
                    if (insInd)
                        ctx.MembershipUserPoints.Add(e);

                    ctx.SaveChanges();
                    return e.Id;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }


    }
}