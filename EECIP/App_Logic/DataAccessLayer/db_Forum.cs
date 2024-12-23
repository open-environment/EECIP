﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        public Post _postStart { get; set; }
        public vmPostDisplayType _postLatest { get; set; }
        public string topicCreator { get; set; }
        public int postCount { get; set; }
        public int upVoteCount { get; set; }
        public int downVoteCount { get; set; }
        public List<Topic_Tags> topicTags { get; set; }
        public string CategoryName { get; set; }
        public string CategorySlug { get; set; }

    }

    public class UserBadgeDisplay {
        public Badge _Badge { get; set; }
        public bool UserEarnedInd { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EarnedDate { get; set; }
    }

    public class UserMostPointsDisplay
    {
        public T_OE_USERS _User { get; set; }
        public int UserPoints { get; set; }
        public string OrgName { get; set; }
    }


    public class PollAnswerWithVotesDisplay
    {
        public Guid PollAnswerID { get; set; }
        public string PollAnswer { get; set; }
        public int PollAnswerVoteCount { get; set; }
    }

    public class MembershipUserPointsDisplay
    {
        public Guid Id { get; set; }
        public int Points { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateAdded { get; set; }
        public string PointsFor { get; set; }
        public string PointsForDetail { get; set; }
        public int UserIDX { get; set; }
        public string Image { get; set; }
    }


    public partial class SP_RECENT_FORUM_BY_USER_TAG_Result_Expanded
    {
        public SP_RECENT_FORUM_BY_USER_TAG_Result SP_RECENT_FORUM_BY_USER_TAG_Result { get; set; }
        public List<string> tags { get; set; }
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
                            join b in ctx.MembershipUser_Badge on a.Id equals b.Badge_Id into sr from x in sr.Where(f => f.MembershipUser_Id == UserIDX).DefaultIfEmpty()  //left join
                            where a.ACT_IND == true
                            orderby x.DateEarned descending, a.SORT_SEQ ascending
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

        public static List<UserBadgeDisplay> GetBadgesForUserNoLeftJoin(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Badges
                            join b in ctx.MembershipUser_Badge on a.Id equals b.Badge_Id
                            where b.MembershipUser_Id == UserIDX
                            orderby b.DateEarned descending
                            select new UserBadgeDisplay
                            {
                                _Badge = a,
                                UserEarnedInd = true,
                                EarnedDate = b.DateEarned
                            }).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int GetBadgesForUserCount(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.MembershipUser_Badge
                            where a.MembershipUser_Id == UserIDX
                            select a).Count();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
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
                            orderby a.SORT_SEQ
                            select a).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Badge GetBadgeByName(string badgeName)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Badges
                            where a.Name == badgeName
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Guid? InsertUpdateBadge(Guid? id, string type, string name, string displayName, string description, string image, int? awardsPoints)
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

        public static int AssignBadgeToUser(int UserIDX, Guid badgeID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    MembershipUser_Badge e = (from c in ctx.MembershipUser_Badge
                               where c.MembershipUser_Id == UserIDX
                               && c.Badge_Id == badgeID
                               select c).FirstOrDefault();

                    if (e != null)
                        return -1; //badge already earned
                    else
                    {
                        e = new MembershipUser_Badge();
                        e.MembershipUser_Id = UserIDX;
                        e.Badge_Id = badgeID;
                        e.DateEarned = DateTime.UtcNow;
                        ctx.MembershipUser_Badge.Add(e);
                        ctx.SaveChanges();
                        return 1;
                    }
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }



        //****************************** MEMBERSHIP USER BADGE **********************************************
        public static int InsertUpdateMembershipUser_Badge(int membershipUser_Id, Guid badge_Id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    MembershipUser_Badge e = (from c in ctx.MembershipUser_Badge
                                              where c.MembershipUser_Id == membershipUser_Id
                                              && c.Badge_Id == badge_Id
                                              select c).FirstOrDefault();

                    if (e == null)
                    {
                        //earning a badge
                        e = new MembershipUser_Badge();
                        e.MembershipUser_Id = membershipUser_Id;
                        e.Badge_Id = badge_Id;
                        e.DateEarned = System.DateTime.UtcNow;
                        ctx.MembershipUser_Badge.Add(e);
                        ctx.SaveChanges();

                        return 1;
                    }
                    else
                        return -1;  //indicates badge already earned
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        /// <summary>
        /// Awards a Badge, adds badge points to user, and creates notification
        /// </summary>
        /// <param name="membershipUser_Id"></param>
        /// <param name="badgeName"></param>
        /// <returns></returns>
        public static bool EarnBadgeController(int membershipUser_Id, string badgeName)
        {
            //0. get badge name
            Badge _b = db_Forum.GetBadgeByName(badgeName);
            if (_b != null)
            {
                //1. award badge
                int SuccID = InsertUpdateMembershipUser_Badge(membershipUser_Id, _b.Id);

                //2. add points to user
                if (SuccID == 1)
                {
                    Guid? UserPointID = InsertUpdateMembershipUserPoints(null, _b.AwardsPoints, System.DateTime.UtcNow, 3, _b.Id, null, membershipUser_Id);
                    if (UserPointID != null)
                    {
                        //3. send notification
                        Guid? NotifyID = db_EECIP.InsertUpdateT_OE_USER_NOTIFICATION(null, membershipUser_Id, System.DateTime.UtcNow, "Badge", "New Badge Earned", "You have earned the " + _b.DisplayName + " badge.", false, 0, true, 0, true);

                        if (NotifyID != null)
                            return true;
                    }
                }

            }

            //if got this far, failure
            return false;
        }

        public static bool EarnBadgePostTopicEvent(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    //get post count of user
                    int TopicCount = db_Forum.GetTopicCountByUser(UserIDX);

                    if (TopicCount == 1)
                        return EarnBadgeController(UserIDX, "OneTopic");
                    else if (TopicCount == 5)
                        return EarnBadgeController(UserIDX, "FiveTopics");
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return false;
                }
            }
        }

        public static bool EarnBadgeUpVotePost(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    //get post count of user
                    int UpvoteCount = db_Forum.GetVotes_TotalByUser(UserIDX, true, false);

                    if (UpvoteCount == 1)
                        return EarnBadgeController(UserIDX, "UserVoteUp");
                    else if (UpvoteCount == 5)
                        return EarnBadgeController(UserIDX, "UserVoteUpFive");
                    else if (UpvoteCount == 10)
                        return EarnBadgeController(UserIDX, "UserVoteUpTen");
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return false;
                }
            }
        }
        
        public static bool EarnBadgeUpVoteProject(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    //get post count of user
                    int UpvoteCount = db_EECIP.GetT_OE_PROJECT_VOTES_TotalByUser(UserIDX);

                    if (UpvoteCount == 1)
                        return EarnBadgeController(UserIDX, "UserProjectLike");
                    else if (UpvoteCount == 5)
                        return EarnBadgeController(UserIDX, "UserProjectLikeFive");
                    else if (UpvoteCount == 10)
                        return EarnBadgeController(UserIDX, "UserProjectLikeTen");
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return false;
                }
            }
        }

        public static bool EarnBadgeAddProject(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    //get post count of user
                    int projCount = db_EECIP.GetT_OE_PROJECTS_CountAddedByUserIDX(UserIDX);

                    if (projCount == 1)
                        return EarnBadgeController(UserIDX, "ProjectProfile");
                    else if (projCount == 5)
                        return EarnBadgeController(UserIDX, "ProjectProfileFive");
                    else if (projCount == 10)
                        return EarnBadgeController(UserIDX, "ProjectProfileTen");
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return false;
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

        /// <summary>
        /// Method returns the forum categories ordered with indentation for display in a dropdown to simulate a tree-like display structure
        /// </summary>
        /// <returns></returns>
        public static List<Category> GetCategory2()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    //new empty container for categories
                    var orderedCategories = new List<Category>();

                    //getting all categories from db once
                    var allCats = ctx.Categories
                        .AsNoTracking()
                        .OrderBy(x => x.SortOrder)
                        .ToList();

                    foreach (var parentCategory in allCats.Where(x => x.Category_Id == null).OrderBy(x => x.SortOrder))
                    {
                        // Add the main category
                        orderedCategories.Add(new Category {
                            Id = parentCategory.Id,
                            Name = parentCategory.Name,
                            SortOrder = parentCategory.SortOrder
                        });

                        // Add subcategories under this
                        orderedCategories.AddRange(GetSubCategories(parentCategory.Id, allCats));
                    }

                    return orderedCategories;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<Category> GetSubCategories(Guid parent_cat_id, List<Category> allCategories, int level = 1)
        {
            string lvl = new String('-', level);

            var catsToReturn = new List<Category>();

            var cats = allCategories.Where(x => x.Category_Id == parent_cat_id).OrderBy(x => x.SortOrder);
            foreach (var cat in cats)
            {
                catsToReturn.Add(new Category {
                    Id = cat.Id,
                    Name = lvl + cat.Name,
                    SortOrder = cat.SortOrder
                });
                catsToReturn.AddRange(GetSubCategories(cat.Id, allCategories, level + 1));
            }

            return catsToReturn;
        }

        public static List<Category> GetSubCategories_Simple(Guid parent_cat_id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Categories.AsNoTracking()
                            where a.Category_Id == parent_cat_id
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

        public static int DeleteCategory(Guid CategoryID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    //then delete the Post
                    Category rec = (from a in ctx.Categories
                                where a.Id == CategoryID
                                select a).FirstOrDefault();

                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
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


        //********************************** TOPICS *************************************************
        public static Topic InsertUpdateTopic(vmForumTopicCreate model, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    Topic e = (from c in ctx.Topics
                                  where c.Id == model.TopicId
                                  select c).FirstOrDefault();

                    //insert case
                    if (e == null)
                    {
                        insInd = true;
                        e = new Topic
                        {
                            Id = Guid.NewGuid(),
                            CreateDate = System.DateTime.UtcNow,
                            Views = 0,
                            Solved = false,
                            Post_Id = null,
                            Poll_Id = null,
                            MembershipUser_Id = UserIDX,
                            Slug = GetUniqueTopicSlug(model.Name)
                        };
                    }

                    //insert or update case
                    if (model.Name != null) e.Name = HttpUtility.HtmlDecode(Utils.SafePlainText(model.Name));  //sanitize 
                    if (e.SolvedReminderSent != null) e.SolvedReminderSent = null;
                    e.IsSticky = model.IsSticky;
                    e.IsLocked = model.IsLocked;
                    e.Pending = null;
                    e.Category_Id = model.Category;

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

        public static Topic GetTopic_ByID(Guid id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Topics.AsNoTracking()
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

        public static Topic GetTopic_ByPostID(Guid PostId)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Topics.AsNoTracking()
                            join b in ctx.Posts.AsNoTracking() on a.Id equals b.Topic_Id
                            where b.Id == PostId
                            select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Guid? SetTopicAnswer(Guid Id, bool Answered)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Topic e = (from c in ctx.Topics
                               where c.Id == Id
                               select c).FirstOrDefault();
      
                    e.Solved = Answered;
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

        public static Guid? SetTopicPollID(Guid Id, Guid Poll_ID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Topic e = (from c in ctx.Topics
                               where c.Id == Id
                               select c).FirstOrDefault();

                    e.Poll_Id = Poll_ID;
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

        public static Guid? UpdateTopic_SetSynced(Guid topicID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Topic e = (from c in ctx.Topics
                               where c.Id == topicID
                               select c).FirstOrDefault();

                    e.SYNC_IND = true;

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

        public static Guid? UpdateTopic_SetLastPostDate(Guid topicID, DateTime? overrideDate)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Topic e = (from c in ctx.Topics
                              where c.Id == topicID
                              select c).FirstOrDefault();

                    e.LAST_POST_DT = overrideDate ?? System.DateTime.Now;

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

        public static bool UpdateTopic_SetAllUnsynced()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.Topics
                               where a.SYNC_IND == true
                               select a).ToList();

                    xxx.ForEach(a => a.SYNC_IND = false);
                    ctx.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return false;
                }
            }
        }

        public static bool PassedTopicFloodTest(string topicTitle, int UserIDX)
        {
            topicTitle = Utils.SafePlainText(topicTitle);
            var floodWindow = System.DateTime.UtcNow.AddMinutes(-1);

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

        public static List<Topic> GetTopicList_ByCategory(Guid cat_id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Topics.AsNoTracking()
                            where a.Category_Id == cat_id
                            select a).ToList();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
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

        public static int GetTopicCount(string tag)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    if (tag == null)
                        return (from a in ctx.Topics.AsNoTracking()
                            select a).Count();
                    else
                        return (from a in ctx.Topics.AsNoTracking()
                                join d in ctx.Topic_Tags on a.Id equals d.Topic_Id
                                where d.TopicTag == tag
                                select a).Count(); 

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int GetTopicCountPostedIn(string tag, List<Guid> PostedIn)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    if (tag == null)
                        return (from a in ctx.Topics.AsNoTracking()
                                where (PostedIn.Any(x => a.Id.Equals(x)))
                                select a).Count();
                    else
                        return (from a in ctx.Topics.AsNoTracking()
                                join d in ctx.Topic_Tags on a.Id equals d.Topic_Id
                                where d.TopicTag == tag
                                && (PostedIn.Any(x => a.Id.Equals(x)))
                                select a).Count();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int GetTopicCountFollowing(string tag, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    if (tag == null)
                        return (from a in ctx.Topics.AsNoTracking()
                                join b in ctx.TopicNotifications on a.Id equals b.Topic_Id
                                where b.MembershipUser_Id == UserIDX
                                select a).Count();
                    else
                        return (from a in ctx.Topics.AsNoTracking()
                                join b in ctx.TopicNotifications on a.Id equals b.Topic_Id
                                join d in ctx.Topic_Tags on a.Id equals d.Topic_Id
                                where d.TopicTag == tag
                                && b.MembershipUser_Id == UserIDX
                                select a).Count();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int GetTopicCountByUser(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Topics.AsNoTracking()
                            where a.MembershipUser_Id == UserIDX
                            select a).Count();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static List<TopicOverviewDisplay> GetTopicsByCategory(Guid? cat_id, string tag, int pageIndex)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    List <TopicOverviewDisplay> xxx = null;

                    if (tag == null)
                    {
                        xxx = (from a in ctx.Topics.AsNoTracking()
                                join b in ctx.Posts on a.Id equals b.Topic_Id
                                join c in ctx.T_OE_USERS on a.MembershipUser_Id equals c.USER_IDX
                                join d in ctx.Categories on a.Category_Id equals d.Id
                                where b.IsTopicStarter == true
                                && (cat_id != null ? a.Category_Id == cat_id : true)
                                orderby a.IsSticky descending, a.LAST_POST_DT descending
                                select new TopicOverviewDisplay
                                {
                                    _topic = a,
                                    _postStart = b,
                                    _postLatest = (from v1 in ctx.Posts join v2 in ctx.T_OE_USERS on v1.MembershipUser_Id equals v2.USER_IDX where v1.Topic_Id == a.Id orderby v1.DateCreated descending select new vmPostDisplayType { Post = v1, PosterDisplayName = v2.FNAME + " " + v2.LNAME }).FirstOrDefault(),
                                    topicCreator = c.FNAME + " " + c.LNAME,
                                    postCount = (from v1 in ctx.Posts where v1.Topic_Id == a.Id select v1).Count(),
                                    upVoteCount = (from v1 in ctx.Votes where v1.Post_Id == b.Id && v1.Amount > 0 select v1).Count(),
                                    downVoteCount = (from v1 in ctx.Votes where v1.Post_Id == b.Id && v1.Amount < 0 select v1).Count(),
                                    topicTags = (from v1 in ctx.Topic_Tags where v1.Topic_Id == a.Id select v1).ToList(),
                                    CategoryName = d.Name,
                                    CategorySlug = d.Slug
                                }).Skip((pageIndex - 1) * 10).Take(10).ToList();

                    }
                    else
                        xxx = (from a in ctx.Topics.AsNoTracking()
                                join b in ctx.Posts on a.Id equals b.Topic_Id
                                join c in ctx.T_OE_USERS on a.MembershipUser_Id equals c.USER_IDX
                                join d in ctx.Topic_Tags on a.Id equals d.Topic_Id
                                join e in ctx.Categories on a.Category_Id equals e.Id
                                where b.IsTopicStarter == true
                                && (cat_id != null ? a.Category_Id == cat_id : true)
                                && d.TopicTag == tag
                                orderby a.IsSticky descending, a.LAST_POST_DT descending
                                select new TopicOverviewDisplay
                                {
                                    _topic = a,
                                    _postStart = b,
                                    _postLatest = (from v1 in ctx.Posts join v2 in ctx.T_OE_USERS on v1.MembershipUser_Id equals v2.USER_IDX where v1.Topic_Id == a.Id orderby v1.DateCreated descending select new vmPostDisplayType { Post = v1, PosterDisplayName = v2.FNAME + " " + v2.LNAME }).FirstOrDefault(),
                                    topicCreator = c.FNAME + " " + c.LNAME,
                                    postCount = (from v1 in ctx.Posts where v1.Topic_Id == a.Id select v1).Count(),
                                    upVoteCount = (from v1 in ctx.Votes where v1.Post_Id == b.Id && v1.Amount > 0 select v1).Count(),
                                    downVoteCount = (from v1 in ctx.Votes where v1.Post_Id == b.Id && v1.Amount < 0 select v1).Count(),
                                    topicTags = (from v1 in ctx.Topic_Tags where v1.Topic_Id == a.Id select v1).ToList(),
                                    CategoryName = e.Name,
                                    CategorySlug = e.Slug
                                }).Skip((pageIndex - 1) * 10).Take(10).ToList();

                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<TopicOverviewDisplay> GetTopicsByCategoryPostedIn(Guid? cat_id, string tag, int pageIndex, List<Guid> PostedIn)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    if (tag == null)
                    {
                        return (from a in ctx.Topics.AsNoTracking()
                               join b in ctx.Posts on a.Id equals b.Topic_Id
                               join c in ctx.T_OE_USERS on a.MembershipUser_Id equals c.USER_IDX
                               join d in ctx.Categories on a.Category_Id equals d.Id
                               where b.IsTopicStarter == true
                               && (cat_id != null ? a.Category_Id == cat_id : true)
                               && (PostedIn.Any(x => a.Id.Equals(x)))
                               orderby b.DateCreated descending
                               select new TopicOverviewDisplay
                               {
                                   _topic = a,
                                   _postStart = b,
                                   _postLatest = (from v1 in ctx.Posts join v2 in ctx.T_OE_USERS on v1.MembershipUser_Id equals v2.USER_IDX where v1.Topic_Id == a.Id orderby v1.DateCreated descending select new vmPostDisplayType { Post = v1, PosterDisplayName = v2.FNAME + " " + v2.LNAME }).FirstOrDefault(),
                                   topicCreator = c.FNAME + " " + c.LNAME,
                                   postCount = (from v1 in ctx.Posts where v1.Topic_Id == a.Id select v1).Count(),
                                   upVoteCount = (from v1 in ctx.Votes where v1.Post_Id == b.Id && v1.Amount > 0 select v1).Count(),
                                   downVoteCount = (from v1 in ctx.Votes where v1.Post_Id == b.Id && v1.Amount < 0 select v1).Count(),
                                   topicTags = (from v1 in ctx.Topic_Tags where v1.Topic_Id == a.Id select v1).ToList(),
                                   CategoryName = d.Name,
                                   CategorySlug = d.Slug
                               }).Skip((pageIndex - 1) * 10).Take(10).ToList();

                    }
                    else
                        return (from a in ctx.Topics.AsNoTracking()
                               join b in ctx.Posts on a.Id equals b.Topic_Id
                               join c in ctx.T_OE_USERS on a.MembershipUser_Id equals c.USER_IDX
                               join d in ctx.Topic_Tags on a.Id equals d.Topic_Id
                               join e in ctx.Categories on a.Category_Id equals e.Id
                               where b.IsTopicStarter == true
                               && (cat_id != null ? a.Category_Id == cat_id : true)
                               && d.TopicTag == tag
                               && (PostedIn.Any(x => a.Id.Equals(x)))
                               orderby b.DateCreated descending
                               select new TopicOverviewDisplay
                               {
                                   _topic = a,
                                   _postStart = b,
                                   _postLatest = (from v1 in ctx.Posts join v2 in ctx.T_OE_USERS on v1.MembershipUser_Id equals v2.USER_IDX where v1.Topic_Id == a.Id orderby v1.DateCreated descending select new vmPostDisplayType { Post = v1, PosterDisplayName = v2.FNAME + " " + v2.LNAME }).FirstOrDefault(),
                                   topicCreator = c.FNAME + " " + c.LNAME,
                                   postCount = (from v1 in ctx.Posts where v1.Topic_Id == a.Id select v1).Count(),
                                   upVoteCount = (from v1 in ctx.Votes where v1.Post_Id == b.Id && v1.Amount > 0 select v1).Count(),
                                   downVoteCount = (from v1 in ctx.Votes where v1.Post_Id == b.Id && v1.Amount < 0 select v1).Count(),
                                   topicTags = (from v1 in ctx.Topic_Tags where v1.Topic_Id == a.Id select v1).ToList(),
                                   CategoryName = e.Name,
                                   CategorySlug = e.Slug
                               }).Skip((pageIndex - 1) * 10).Take(10).ToList();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<TopicOverviewDisplay> GetTopicsByCategoryFollowing(Guid? cat_id, string tag, int pageIndex, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    if (tag == null)
                    {
                        return (from a in ctx.Topics.AsNoTracking()
                                join b in ctx.Posts on a.Id equals b.Topic_Id
                                join c in ctx.T_OE_USERS on a.MembershipUser_Id equals c.USER_IDX
                                join d in ctx.Categories on a.Category_Id equals d.Id
                                join e in ctx.TopicNotifications on a.Id equals e.Topic_Id
                                where b.IsTopicStarter == true
                                && (cat_id != null ? a.Category_Id == cat_id : true)
                                && e.MembershipUser_Id == UserIDX
                                orderby b.DateCreated descending
                                select new TopicOverviewDisplay
                                {
                                    _topic = a,
                                    _postStart = b,
                                    _postLatest = (from v1 in ctx.Posts join v2 in ctx.T_OE_USERS on v1.MembershipUser_Id equals v2.USER_IDX where v1.Topic_Id == a.Id orderby v1.DateCreated descending select new vmPostDisplayType { Post = v1, PosterDisplayName = v2.FNAME + " " + v2.LNAME }).FirstOrDefault(),
                                    topicCreator = c.FNAME + " " + c.LNAME,
                                    postCount = (from v1 in ctx.Posts where v1.Topic_Id == a.Id select v1).Count(),
                                    upVoteCount = (from v1 in ctx.Votes where v1.Post_Id == b.Id && v1.Amount > 0 select v1).Count(),
                                    downVoteCount = (from v1 in ctx.Votes where v1.Post_Id == b.Id && v1.Amount < 0 select v1).Count(),
                                    topicTags = (from v1 in ctx.Topic_Tags where v1.Topic_Id == a.Id select v1).ToList(),
                                    CategoryName = d.Name,
                                    CategorySlug = d.Slug
                                }).Skip((pageIndex - 1) * 10).Take(10).ToList();

                    }
                    else
                        return (from a in ctx.Topics.AsNoTracking()
                                join b in ctx.Posts on a.Id equals b.Topic_Id
                                join c in ctx.T_OE_USERS on a.MembershipUser_Id equals c.USER_IDX
                                join d in ctx.Topic_Tags on a.Id equals d.Topic_Id
                                join e in ctx.Categories on a.Category_Id equals e.Id
                                join f in ctx.TopicNotifications on a.Id equals f.Topic_Id
                                where b.IsTopicStarter == true
                                && (cat_id != null ? a.Category_Id == cat_id : true)
                                && d.TopicTag == tag
                                && f.MembershipUser_Id == UserIDX
                                orderby b.DateCreated descending
                                select new TopicOverviewDisplay
                                {
                                    _topic = a,
                                    _postStart = b,
                                    _postLatest = (from v1 in ctx.Posts join v2 in ctx.T_OE_USERS on v1.MembershipUser_Id equals v2.USER_IDX where v1.Topic_Id == a.Id orderby v1.DateCreated descending select new vmPostDisplayType { Post = v1, PosterDisplayName = v2.FNAME + " " + v2.LNAME }).FirstOrDefault(),
                                    topicCreator = c.FNAME + " " + c.LNAME,
                                    postCount = (from v1 in ctx.Posts where v1.Topic_Id == a.Id select v1).Count(),
                                    upVoteCount = (from v1 in ctx.Votes where v1.Post_Id == b.Id && v1.Amount > 0 select v1).Count(),
                                    downVoteCount = (from v1 in ctx.Votes where v1.Post_Id == b.Id && v1.Amount < 0 select v1).Count(),
                                    topicTags = (from v1 in ctx.Topic_Tags where v1.Topic_Id == a.Id select v1).ToList(),
                                    CategoryName = e.Name,
                                    CategorySlug = e.Slug
                                }).Skip((pageIndex - 1) * 10).Take(10).ToList();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<SP_RECENT_FORUM_BY_USER_TAG_Result_Expanded> GetLatestTopicPostsMatchingInterestNew(int UserIDX, int daysSince, int recCount, string tagFilter)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    if (tagFilter == "") tagFilter = null;

                    var x = (from a in ctx.SP_RECENT_FORUM_BY_USER_TAG(UserIDX, daysSince, tagFilter)
                             orderby a.LatestPostDate descending
                             select a).Take(recCount).ToList();
                    
                    List<SP_RECENT_FORUM_BY_USER_TAG_Result_Expanded> _topicsExpandList = new List<SP_RECENT_FORUM_BY_USER_TAG_Result_Expanded>();

                    foreach (SP_RECENT_FORUM_BY_USER_TAG_Result _topic in x)
                    {
                        List<string> _tags = db_Forum.GetTopicTags_MatchingUserAndTopic(_topic.Id, UserIDX);

                        _topicsExpandList.Add(new SP_RECENT_FORUM_BY_USER_TAG_Result_Expanded
                        {
                            SP_RECENT_FORUM_BY_USER_TAG_Result = _topic,
                            tags = _tags
                        });
                    }

                    return _topicsExpandList;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<SP_RECENT_FORUM_BY_USER_TAG_Result_Expanded> GetLatestTopicPostsFallback(int daysSince, int recCount)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var x = (from a in ctx.SP_RECENT_FORUM_FALLBACK(daysSince)
                             orderby a.LatestPostDate descending
                             select a).Take(recCount).ToList();



                    List<SP_RECENT_FORUM_BY_USER_TAG_Result_Expanded> _topicsExpandList = new List<SP_RECENT_FORUM_BY_USER_TAG_Result_Expanded>();

                    foreach (SP_RECENT_FORUM_BY_USER_TAG_Result _topic in x)
                    {
                        _topicsExpandList.Add(new SP_RECENT_FORUM_BY_USER_TAG_Result_Expanded
                        {
                            SP_RECENT_FORUM_BY_USER_TAG_Result = _topic,
                        });
                    }

                    return _topicsExpandList;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<string> GetTopicTags_MatchingUserAndTopic(Guid TopicID, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Topic_Tags
                            join b in ctx.T_OE_USER_EXPERTISE on a.TopicTag equals b.EXPERTISE_TAG 
                            where a.Topic_Id == TopicID
                            && b.USER_IDX == UserIDX
                            select a.TopicTag.ToString()).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }


        public static List<EECIP_Index> GetTopic_ReadyToSync(Guid? TopicID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.Topics
                               join b in ctx.Posts on a.Id equals b.Topic_Id
                               join u in ctx.T_OE_USERS on a.MembershipUser_Id equals u.USER_IDX
                               join o in ctx.T_OE_ORGANIZATION on u.ORG_IDX equals o.ORG_IDX into sr2 from x2 in sr2.DefaultIfEmpty() //left join
                               join s in ctx.T_OE_REF_STATE on x2.STATE_CD equals s.STATE_CD into sr1 from x1 in sr1.DefaultIfEmpty() //left join
                               where a.SYNC_IND == false
                               && b.IsTopicStarter == true
                               && (TopicID == null ? true : a.Id == TopicID)
                               select new EECIP_Index
                               {
                                   Agency = x2.ORG_NAME,
                                   AgencyAbbreviation = x2.ORG_ABBR,
                                   OrgType = x2.ORG_TYPE,
                                   State = (x2.ORG_TYPE == "State" ? x1.STATE_NAME : null),
                                   //State_or_Tribal = (o.ORG_TYPE == "State" ? x1.STATE_NAME : o.ORG_TYPE),
                                   KeyID = a.Id.ToString(),
                                   DataType = "Discussion",
                                   Record_Source = "User supplied",
                                   Name = a.Name,
                                   Description = b.PostContent,
                                   LastUpdated = b.DateEdited 
                               }).Take(250).ToList();

                    foreach (EECIP_Index e in xxx)
                        e.Tags = db_Forum.GetTopicTags_ByAttributeSelected(new Guid(e.KeyID), "Topic Tag").ToArray();


                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<EECIP_Index> GetPost_ReadyToSync(Guid? PostID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.Topics
                               join b in ctx.Posts on a.Id equals b.Topic_Id
                               join u in ctx.T_OE_USERS on b.MembershipUser_Id equals u.USER_IDX
                               join o in ctx.T_OE_ORGANIZATION on u.ORG_IDX equals o.ORG_IDX into sr2 from oleft in sr2.DefaultIfEmpty() //left join
                               join s in ctx.T_OE_REF_STATE on oleft.STATE_CD equals s.STATE_CD into sr1 from sleft in sr1.DefaultIfEmpty() //left join
                               where b.SYNC_IND == false
                               && (PostID == null ? true : b.Id == PostID)
                               select new EECIP_Index
                               {
                                   Agency = oleft.ORG_NAME ?? "None",
                                   AgencyAbbreviation = oleft.ORG_ABBR,
                                   OrgType = oleft.ORG_TYPE,
                                   State = (oleft.ORG_TYPE == "State" ? sleft.STATE_NAME : null),
                                   //State_or_Tribal = (o.ORG_TYPE == "State" ? x1.STATE_NAME : o.ORG_TYPE),
                                   KeyID = b.Id.ToString(),
                                   DataType = "Discussion",
                                   Record_Source = "User supplied",
                                   Name = a.Name,
                                   Description = u.FNAME + " " + u.LNAME + ": " + b.PostContent,
                                   LastUpdated = b.DateEdited,
                                   LikeCount = (from vv in ctx.Votes where vv.Post_Id == b.Id select vv).Count()
                               }).Take(250).ToList();

                    foreach (EECIP_Index e in xxx)
                        e.Tags = db_Forum.GetTopicTags_ByAttributeSelected(new Guid(e.KeyID), "Topic Tag").ToArray();


                    return xxx;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int OrphanTopic(Guid TopicID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Topic rec = (from a in ctx.Topics
                                 where a.Id == TopicID
                                 select a).FirstOrDefault();

                    rec.Post_Id = null;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    //temp
                    if (ex.InnerException != null)
                        db_Ref.InsertT_OE_SYS_LOG("Forum ERROR", ex.InnerException.ToString().SubStringPlus(0, 2000));

                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int DeleteTopic(Guid TopicID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Topic rec = (from a in ctx.Topics
                                where a.Id == TopicID
                                select a).FirstOrDefault();

                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    //temp
                    if (ex.InnerException != null)
                        db_Ref.InsertT_OE_SYS_LOG("Forum ERROR", ex.InnerException.ToString().SubStringPlus(0, 2000));


                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }



        //********************************** TOPIC TAGS*************************************************
        public static List<string> GetTopicTags_ByAttributeSelected(Guid TopicID, string aTTRIBUTE)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Topic_Tags
                            where a.Topic_Id == TopicID
                            && a.TopicTagAttribute == aTTRIBUTE
                            select a.TopicTag.ToString()).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int InsertUpdateTopicTags(Guid topic_id, string aTTRIBUTE, string tAG)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {

                try
                {

                    Topic_Tags e = new Topic_Tags();
                    e.Topic_Id = topic_id;
                    e.TopicTagAttribute = aTTRIBUTE;
                    e.TopicTag = tAG;

                    ctx.Topic_Tags.Add(e);

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

        public static int DeleteTopicTags(Guid TopicId, string aTTRIBUTE)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    ctx.Database.ExecuteSqlCommand("DELETE FROM [forum].[Topic_Tags] where Topic_Id = '" + TopicId + "' and TopicTagAttribute = '" + aTTRIBUTE + "'");

                    return 1;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static List<string> GetTopicTags_ByAttributeAll(Guid topic_id, string aTTRIBUTE)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xx1 = (from a in ctx.Topic_Tags
                               where a.Topic_Id == topic_id
                               && a.TopicTagAttribute == "Topic Tag"
                               select a.TopicTag);

                    var ss1 = xx1.ToList();

                    var xx2 = (from a in ctx.T_OE_REF_TAGS
                               where a.TAG_CAT_NAME == aTTRIBUTE
                               select a.TAG_NAME);
                    var ss2 = xx2.ToList();

                    return xx1.Union(xx2).ToList();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Get a specified amount of the most popular tags, ordered by use amount
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="allowedCategories"></param>
        /// <returns></returns>
        public static Dictionary<string, int> GetPopularTags()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var tags = ctx.Topic_Tags
                        .GroupBy(x => x.TopicTag)
                        .Select(g => new { g.Key, Count = g.Count() }).Take(20);

                    return tags.ToDictionary(tag => tag.Key, tag => tag.Count);
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int UpdateTopicTagsBulk(string prevTag, string newTag)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    ctx.Database.ExecuteSqlCommand("UPDATE [forum].[Topic_Tags] set TopicTag = '" + newTag + "' where TopicTag = '" + prevTag + "'");

                    return 1;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
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

        public static List<TopicNotification> GetTopicNotification_ByTopic(Guid topic_id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.TopicNotifications.AsNoTracking()
                               where a.Topic_Id == topic_id
                               select a).ToList();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<TopicNotification> GetTopicNotification_ByUserIDX(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.TopicNotifications.AsNoTracking()
                            where a.MembershipUser_Id == UserIDX
                            select a).ToList();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteTopicNotification(Guid topic_id, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    TopicNotification rec = (from a in ctx.TopicNotifications
                                where a.Topic_Id == topic_id
                                && a.MembershipUser_Id == UserIDX
                                select a).FirstOrDefault();

                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
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




        //************************************POST **************************************************
        public static Guid? InsertUpdatePost(Guid? id, string postContent, int? voteCount, bool? isSolution, bool? isTopicStarter, bool? flaggedAsSpam, bool? pending, 
            string searchField, Guid? inReplyTo, Guid? topicId, int? UserIDX, bool? editPost = false)
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
                        e.DateEdited = System.DateTime.UtcNow;
                        e.VoteCount = 0;
                        e.IpAddress = Utils.GetUsersIpAddress();

                        if (topicId == null || UserIDX == null)  //error if this info not supplied on create case
                            return null;

                        e.Topic_Id = topicId ?? Guid.Empty;
                        e.MembershipUser_Id = UserIDX ?? 0;
                    }
                    else if (editPost == true)
                    {
                        e.DateEdited = System.DateTime.UtcNow;
                        e.SYNC_IND = false;
                    }

                    if (postContent != null) e.PostContent = Utils.GetSafeHtml(postContent);   //sanitize
                    if (voteCount != null) e.VoteCount = voteCount ?? 0;
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

        public static Guid? UpdatePost_SetSynced(Guid postID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Post e = (from c in ctx.Posts
                               where c.Id == postID
                               select c).FirstOrDefault();

                    e.SYNC_IND = true;

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

        public static bool UpdatePost_SetAllUnsynced()
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.Posts
                               where a.SYNC_IND == true
                               select a).ToList();

                    xxx.ForEach(a => a.SYNC_IND = false);
                    ctx.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return false;
                }
            }
        }

        public static Post GetPost_ByID(Guid post_id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return  (from a in ctx.Posts.AsNoTracking()
                               where a.Id == post_id
                               select a).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<Post> GetPost_ByTopicID(Guid topic_id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Posts.AsNoTracking()
                            join b in ctx.Topics on a.Topic_Id equals b.Id
                            where a.Topic_Id == topic_id
                            select a).ToList();

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
                               join c in ctx.T_OE_USERS on a.MembershipUser_Id equals c.USER_IDX
                            where a.Topic_Id == topic_id
                            && a.IsTopicStarter == true
                            select new vmForumPost {
                                Post = a,
                                ParentTopic = b,
                                UpVoteCount = (from v1 in ctx.Votes where v1.Post_Id == a.Id && v1.Amount > 0 select v1).Count(),
                                DownVoteCount = (from v1 in ctx.Votes where v1.Post_Id == a.Id && v1.Amount < 0 select v1).Count(),
                                UserIDX = UserIDX,
                                PosterName = c.FNAME + " " + c.LNAME,
                                HasVotedUp = (from v1 in ctx.Votes where v1.Post_Id == a.Id && v1.VotedByMembershipUser_Id == UserIDX && v1.Amount > 0 select v1).ToList().Count() > 0,
                                HasVotedDown = (from v1 in ctx.Votes where v1.Post_Id == a.Id && v1.VotedByMembershipUser_Id == UserIDX && v1.Amount < 0 select v1).ToList().Count() > 0,
                                PostFiles = (from v1 in ctx.PostFiles
                                             where v1.Post_Id == a.Id
                                             select new vmPostFileDisplay
                                             {
                                                 Id = v1.Id,
                                                 FileName = v1.Filename,
                                                 DateCreated = v1.DateCreated,
                                                 PostId = v1.Post_Id,
                                                 FileDescription = v1.FileDecription,
                                                 MembershipUser_Id = v1.MembershipUser_Id
                                             }).ToList()
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

        public static List<Guid> GetTopicIDs_InWhichUserPosted(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Posts.AsNoTracking()
                               where a.MembershipUser_Id == UserIDX 
                               select a.Topic_Id).Distinct().ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<vmForumPost> GetPost_NonStarterForTopic(Guid topic_id, int UserIDX, string orderBy)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.Posts.AsNoTracking()
                               join b in ctx.Topics on a.Topic_Id equals b.Id
                               join c in ctx.T_OE_USERS on a.MembershipUser_Id equals c.USER_IDX
                               where a.Topic_Id == topic_id
                               && a.IsTopicStarter == false
                               select new vmForumPost
                               {
                                   Post = a,
                                   ParentTopic = b,
                                   UpVoteCount = (from v1 in ctx.Votes where v1.Post_Id == a.Id && v1.Amount > 0 select v1).Count(),
                                   DownVoteCount = (from v1 in ctx.Votes where v1.Post_Id == a.Id && v1.Amount < 0 select v1).Count(),
                                   UserIDX = UserIDX,
                                   PosterName = c.FNAME + " " + c.LNAME,
                                   HasVotedUp = (from v1 in ctx.Votes where v1.Post_Id == a.Id && v1.VotedByMembershipUser_Id == UserIDX && v1.Amount > 0 select v1).ToList().Count() > 0,
                                   HasVotedDown = (from v1 in ctx.Votes where v1.Post_Id == a.Id && v1.VotedByMembershipUser_Id == UserIDX && v1.Amount < 0 select v1).ToList().Count() > 0,
                                   PostFiles = (from v1 in ctx.PostFiles
                                                where v1.Post_Id == a.Id
                                                select new vmPostFileDisplay
                                                {
                                                    Id = v1.Id,
                                                    FileName = v1.Filename,
                                                    DateCreated = v1.DateCreated,
                                                    PostId = v1.Post_Id,
                                                    FileDescription = v1.FileDecription,
                                                    MembershipUser_Id = v1.MembershipUser_Id
                                                }).ToList()
                               }).ToList();


                    if (orderBy == "votes")
                        return xxx.OrderByDescending(x => x.UpVoteCount).ToList();
                    else if (orderBy == "newest")
                        return xxx.OrderByDescending(x => x.Post.DateCreated).ToList();
                    else 
                        return xxx.OrderBy(x => x.Post.DateCreated).ToList();

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

        public static bool PassedPostFloodTest(int UserIDX)
        {
            var timeNow = DateTime.UtcNow;
            var floodWindow = timeNow.AddSeconds(-30);

            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.Posts.AsNoTracking()
                               where a.MembershipUser_Id == UserIDX
                               && a.DateCreated >= floodWindow
                               && a.IsTopicStarter == false
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

        public static int DeletePost(Guid PostID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    //first delete any Post Files
                    List<PostFile> _postFiles = (from a in ctx.PostFiles
                                           where a.Post_Id == PostID
                                           select a).ToList();

                    if (_postFiles != null && _postFiles.Count > 0)
                    {
                        foreach (PostFile _postFile in _postFiles)
                        {
                            DeletePostFile(_postFile.Id);
                        }
                    }

                    //then delete the Post
                    Post rec = (from a in ctx.Posts
                                where a.Id == PostID
                                select a).FirstOrDefault();

                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    //temp
                    if (ex.InnerException != null)
                        db_Ref.InsertT_OE_SYS_LOG("Form ERROR", ex.InnerException.ToString().SubStringPlus(0, 2000));


                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static Post GetPost_LatestOfTopic(Guid topic_id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Posts.AsNoTracking()
                               where a.Topic_Id == topic_id
                               orderby a.DateEdited descending
                               select a).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }


        //*************************************POLL *********************************
        public static Guid? InsertUpdatePoll(Guid? id, bool? isClosed, DateTime? dateCreated, int? closePollAfterDays, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    bool InsInd = false;

                    //get from ID
                    Poll e = (from c in ctx.Polls
                                           where c.Id == id
                                           select c).FirstOrDefault();

                    //insert case
                    if (e == null)
                    {
                        InsInd = true;
                        e = new Poll();
                        e.Id = Guid.NewGuid();
                    }

                    if (isClosed != null) e.IsClosed = isClosed.ConvertOrDefault<bool>();
                    if (dateCreated != null) e.DateCreated = dateCreated.ConvertOrDefault<DateTime>();
                    if (closePollAfterDays != null) e.ClosePollAfterDays = closePollAfterDays;
                    e.MembershipUser_Id = UserIDX;

                    if (InsInd)
                        ctx.Polls.Add(e);

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

        public static Poll GetPoll_ByID(Guid Id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Polls.AsNoTracking()
                            where a.Id == Id
                            select a).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }



        //*************************************POLL ANSWER*********************************
        public static Guid? InsertUpdatePollAnswer(Guid? id, string answer, Guid? poll_ID)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    bool InsInd = false;

                    //get from ID
                    PollAnswer e = (from c in ctx.PollAnswers
                                    where c.Id == id
                                    select c).FirstOrDefault();

                    //insert case
                    if (e == null)
                    {
                        InsInd = true;
                        e = new PollAnswer();
                        e.Id = Guid.NewGuid();
                    }

                    if (answer != null) e.Answer = answer;
                    if (poll_ID != null) e.Poll_Id = poll_ID.ConvertOrDefault<Guid>();

                    if (InsInd)
                        ctx.PollAnswers.Add(e);

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

        public static List<PollAnswer> GetPollAnswers_ByPollID(Guid PollId)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.PollAnswers.AsNoTracking()
                            where a.Poll_Id == PollId
                            select a).ToList();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<PollAnswerWithVotesDisplay> GetPollAnswersWithVotes_ByPollID(Guid PollId)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.PollAnswers.AsNoTracking()
                            where a.Poll_Id == PollId
                            select new PollAnswerWithVotesDisplay {
                                PollAnswerID = a.Id,
                                PollAnswer = a.Answer,
                                PollAnswerVoteCount = (from v1 in ctx.PollVotes where v1.PollAnswer_Id == a.Id select v1).Count()
                            }).ToList();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }



        //*************************************POLL VOTES*********************************
        public static int GetPollVotes_ByPolAnswerID(Guid PollAnswerId)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.PollVotes.AsNoTracking()
                            where a.PollAnswer_Id == PollAnswerId
                            select a).Count();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int GetPollVotes_ByPollID(Guid PollId)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.PollVotes.AsNoTracking()
                            join b in ctx.PollAnswers.AsNoTracking() on a.PollAnswer_Id equals b.Id
                            where b.Poll_Id == PollId
                            select a).Count();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static bool HasUserVotedInPoll(Guid PollId, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    int xxx = (from a in ctx.PollVotes.AsNoTracking()
                            join b in ctx.PollAnswers.AsNoTracking() on a.PollAnswer_Id equals b.Id
                            where b.Poll_Id == PollId
                            && a.MembershipUser_Id == UserIDX
                            select a).Count();

                    return xxx > 0;

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return false;
                }
            }
        }


        public static Guid? InsertUpdatePollVote(Guid? id, Guid poll_answer_ID, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    bool InsInd = false;

                    //get from ID
                    PollVote e = (from c in ctx.PollVotes
                                    where c.Id == id
                                    select c).FirstOrDefault();

                    //otherwise get from answer_id and userIDX
                    if (e == null)
                    {
                        e = (from c in ctx.PollVotes
                             where c.PollAnswer_Id == poll_answer_ID
                             && c.MembershipUser_Id == UserIDX
                             select c).FirstOrDefault();
                    }

                    //insert case
                    if (e == null)
                    {
                        InsInd = true;
                        e = new PollVote();
                        e.Id = Guid.NewGuid();
                    }

                    e.PollAnswer_Id = poll_answer_ID;
                    e.MembershipUser_Id = UserIDX;

                    if (InsInd)
                        ctx.PollVotes.Add(e);

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



        //*************************************POST FILES*********************************
        public static List<vmPostFileDisplay> GetPostFileDisplay_ByPostID(Guid post_id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.PostFiles.AsNoTracking()
                            where a.Post_Id == post_id
                            select new vmPostFileDisplay {
                                Id = a.Id,
                                FileName = a.Filename,
                                DateCreated = a.DateCreated,
                                PostId = a.Post_Id,
                                FileDescription = a.FileDecription,
                                MembershipUser_Id = a.MembershipUser_Id
                            }).ToList();

                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Guid? InsertUpdatePostFile(Guid? id, string fileName, Guid? postID, byte[] fileContent, string fileDescription, string fileType, int? UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Boolean insInd = false;

                    PostFile e = (from c in ctx.PostFiles
                                  where c.Id == id
                              select c).FirstOrDefault();

                    //insert case
                    if (e == null)
                    {
                        insInd = true;
                        e = new PostFile();
                        e.Id = Guid.NewGuid();
                        e.DateCreated = System.DateTime.UtcNow;
                        e.MembershipUser_Id = UserIDX ?? 0;
                    }

                    if (fileName != null) e.Filename = fileName;
                    if (postID != null) e.Post_Id = postID;
                    if (fileContent != null) e.FileContent = fileContent;
                    if (fileDescription != null) e.FileDecription = fileDescription;
                    if (fileType != null) e.FileContentType = fileType;

                    if (insInd)
                        ctx.PostFiles.Add(e);

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

        public static PostFile GetPostFile_ByID(Guid id)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.PostFiles
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

        public static int DeletePostFile(Guid PostFile)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    PostFile rec = (from a in ctx.PostFiles
                                 where a.Id == PostFile
                                 select a).FirstOrDefault();

                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
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
        


        //*************************************MEMBERSHIP USER POINTS*********************************
        /// <summary>
        /// Adds points to a user
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

        public static List<UserMostPointsDisplay> GetMembershipUserPoints_MostPoints(int recCount, DateTime? startDt = null, DateTime? endDt = null)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {

                    if (endDt == null)
                        endDt = System.DateTime.Today.AddDays(1);
                    if (startDt == null)
                        startDt = System.DateTime.Today.AddDays(-3650);

                    var tags = ctx.MembershipUserPoints
                        .Where(x => x.DateAdded >= startDt)
                        .Where(x => x.DateAdded <= endDt)
                        .GroupBy(x => x.MembershipUser_Id)
                        .Select(g => new { g.Key, Sum = g.Sum(_ => _.Points) });

                    //var ddd = tags.ToList();

                    var yyy = (from a in ctx.T_OE_USERS
                               join o in ctx.T_OE_ORGANIZATION on a.ORG_IDX equals o.ORG_IDX into sr1 from x1 in sr1.DefaultIfEmpty() //left join
                               join b in tags on a.USER_IDX equals b.Key
                               where a.EXCLUDE_POINTS_IND == false
                               orderby b.Sum descending
                               select new UserMostPointsDisplay {
                                   _User = a,
                                   OrgName = x1.ORG_NAME,
                                   UserPoints = b.Sum
                               }).Take(recCount).ToList();

                    return yyy;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<MembershipUserPointsDisplay> GetMembershipUserPoints_ByUserID(int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {

                    var badge = (from a in ctx.MembershipUserPoints
                                join b in ctx.Badges on a.PointsForId equals b.Id
                                join u in ctx.T_OE_USERS on a.MembershipUser_Id equals u.USER_IDX
                                where a.PointsFor == 3
                                && u.EXCLUDE_POINTS_IND == false
                                && a.MembershipUser_Id == UserIDX
                                select new MembershipUserPointsDisplay
                                {
                                    Id = a.Id,
                                    Points = a.Points,
                                    DateAdded = a.DateAdded,
                                    PointsFor = "Badge Earned",
                                    PointsForDetail = b.DisplayName,
                                    UserIDX = a.MembershipUser_Id,
                                    Image = b.Image
                                }).ToList();

                    var post = (from a in ctx.MembershipUserPoints
                                 join b in ctx.Posts on a.PointsForId equals b.Id
                                 join c in ctx.Topics on b.Topic_Id equals c.Id
                                join u in ctx.T_OE_USERS on a.MembershipUser_Id equals u.USER_IDX
                                where a.PointsFor == 0
                                && u.EXCLUDE_POINTS_IND == false
                                && a.MembershipUser_Id == UserIDX
                                 select new MembershipUserPointsDisplay
                                 {
                                     Id = a.Id,
                                     Points = a.Points,
                                     DateAdded = a.DateAdded,
                                     PointsFor = "Post Made",
                                     PointsForDetail = c.Name,
                                     UserIDX = a.MembershipUser_Id,
                                     Image = null
                                 }).ToList();

                    return badge.Union(post).OrderByDescending(m => m.DateAdded).ToList();
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return null;
                }
            }
        }

        //****************************************VOTES **************************************************
        public static Guid? InsertVote(Guid post_id, int vOTED_BY_USER_IDX, int vOTE_AMOUNT)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    bool insInd = false;

                    Vote e = (from c in ctx.Votes
                              where c.Post_Id == post_id
                              && c.VotedByMembershipUser_Id == vOTED_BY_USER_IDX
                              select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new Vote();
                        e.Id = Guid.NewGuid();
                        e.Post_Id = post_id;

                        //get UserIDX for the post
                        Post _p = GetPost_ByID(post_id);
                        if (_p != null)
                            e.MembershipUser_Id = _p.MembershipUser_Id; 
                        e.VotedByMembershipUser_Id = vOTED_BY_USER_IDX;
                    }

                    e.DateVoted = System.DateTime.UtcNow;
                    e.Amount = vOTE_AMOUNT;

                    if (insInd)
                        ctx.Votes.Add(e);
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

        public static int GetVotes_TotalByPost(Guid Id, bool UpVoteOnly, bool DownVoteOnly)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.Votes
                            where a.Post_Id == Id
                            && (UpVoteOnly ? a.Amount > 0 : true)
                            && (DownVoteOnly ? a.Amount < 0 : true)
                            select (int?) a.Amount).Sum() ?? 0;

                    return Math.Abs(xxx);
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int GetVotes_TotalByUser(int UserIDX, bool UpVoteOnly, bool DownVoteOnly)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    var xxx = (from a in ctx.Votes
                               where a.VotedByMembershipUser_Id == UserIDX
                               && (UpVoteOnly ? a.Amount > 0 : true)
                               && (DownVoteOnly ? a.Amount < 0 : true)
                               select (int?)a.Amount).Sum() ?? 0;

                    return Math.Abs(xxx);
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static int DeleteVote(Guid Id, int vOTED_BY_USER_IDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    Vote rec = (from a in ctx.Votes
                                where a.Post_Id == Id
                                && a.VotedByMembershipUser_Id == vOTED_BY_USER_IDX
                                select a).FirstOrDefault();

                    ctx.Entry(rec).State = System.Data.Entity.EntityState.Deleted;
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

        public static bool HasVotedUp(Guid post_id, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Votes
                            where a.Post_Id == post_id
                            && a.VotedByMembershipUser_Id == UserIDX
                            && a.Amount > 0
                            select a).ToList().Count() > 0;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return true;
                }
            }
        }

        public static bool HasVotedDown(Guid post_id, int UserIDX)
        {
            using (EECIPEntities ctx = new EECIPEntities())
            {
                try
                {
                    return (from a in ctx.Votes
                            where a.Post_Id == post_id
                            && a.VotedByMembershipUser_Id == UserIDX
                            && a.Amount < 0
                            select a).ToList().Count() > 0;
                }
                catch (Exception ex)
                {
                    db_Ref.LogEFException(ex);
                    return true;
                }
            }
        }

    }
}