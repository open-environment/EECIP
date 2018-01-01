using System.ComponentModel.DataAnnotations;
using EECIP.App_Logic.DataAccessLayer;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using System;
using System.ComponentModel;
using System.Linq;

namespace EECIP.Models
{
    public static class ddlForumHelpers
    {
        public static IEnumerable<SelectListItem> get_ddl_categories()
        {
            return db_Forum.GetCategory().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
        }
    }

    public class vmForumAdminCategories
    {
        public List<Category> Categories { get; set; }

    }

    public class vmForumAdminCategory
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Category Name")]
        //[Required]
        [StringLength(600)]
        public string Name { get; set; }

        [DisplayName("Category Description")]
        [DataType(DataType.MultilineText)]
        [UIHint("forumeditor"), AllowHtml]
        public string Description { get; set; }

        [DisplayName("Category Colour")]
        [UIHint("colourpicker"), AllowHtml]
        public string CategoryColour { get; set; }

        [DisplayName("Lock The Category")]
        public bool IsLocked { get; set; }

        [DisplayName("Moderate all topics in this Category")]
        public bool ModerateTopics { get; set; }

        [DisplayName("Moderate all posts in this Category")]
        public bool ModeratePosts { get; set; }

        [DisplayName("Sort Order")]
        [Range(0, int.MaxValue)]
        public int SortOrder { get; set; }

        [DisplayName("Parent Category")]
        public Guid? ParentCategory { get; set; }

        public IEnumerable<SelectListItem> AllCategories { get; set; }

        [DisplayName("Page Title")]
        [MaxLength(80)]
        public string PageTitle { get; set; }

        [DisplayName("Meta Desc")]
        [MaxLength(200)]
        public string MetaDesc { get; set; }

        [DisplayName("Category Image")]
        public HttpPostedFileBase[] Files { get; set; }
        public string Image { get; set; }


        //initialize
        public vmForumAdminCategory(){
            AllCategories = ddlForumHelpers.get_ddl_categories();
        }
    }


    public class vmForumTopicCreate
    {
        [Required]
        [StringLength(100)]
        [DisplayName("Title")]
        public string Name { get; set; }

        [UIHint("forumeditor"), AllowHtml]
        [StringLength(6000)]
        public string Content { get; set; }

        [DisplayName("Is Sticky Topic")]
        public bool IsSticky { get; set; }

        [DisplayName("Lock Topic")]
        public bool IsLocked { get; set; }

        [Required]
        [DisplayName("Choose Category")]
        public Guid Category { get; set; }

        public List<string> SelectedTags { get; set; }
        public IEnumerable<SelectListItem> AllTags { get; set; }


        [DisplayName("Close Poll After Specified Amount Of Days?")]
        public int PollCloseAfterDays { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public List<string> PollAnswers { get; set; }

        [DisplayName("Subscribe to Topic")]
        public bool SubscribeToTopic { get; set; }

        [DisplayName("Upload File")]
        public HttpPostedFileBase[] Files { get; set; }

        // Permissions stuff
        public CheckCreateTopicPermissions OptionalPermissions { get; set; }

        // Edit Properties
        [HiddenInput]
        public Guid Id { get; set; }

        [HiddenInput]
        public Guid TopicId { get; set; }

        public bool IsTopicStarter { get; set; }

    }

    public class vmForumTopicView
    {
        public Topic Topic { get; set; }
        public List<string> TopicTags { get; set; }
        //public bool MemberIsOnline { get; set; }
        public string LastPostDatePretty { get; set; }

        // Poll
        //public vmForumPoll Poll { get; set; }

        // Post Stuff
        public  vmForumPost StarterPost { get; set; }
        public List<vmForumPost> Posts { get; set; }
        //public int? PageIndex { get; set; }
        //public int? TotalCount { get; set; }
        //public int? TotalPages { get; set; }

        // Permissions
        //public bool DisablePosting { get; set; }

        // Subscription
        public bool IsSubscribed { get; set; }

        // Votes
        //public int VotesUp { get; set; }
        //public int VotesDown { get; set; }

        // Quote/Reply
        //public string QuotedPost { get; set; }
        //public Guid? ReplyTo { get; set; }
        //public string ReplyToUsername { get; set; }

        // Stats
        public int Answers { get; set; }
        public int Views { get; set; }

        // Misc
        public bool ShowUnSubscribedLink { get; set; }


        //new post
        [DataType(DataType.MultilineText)]
        [UIHint("forumeditor"), AllowHtml]
        public string NewPostContent { get; set; }
    }
    
    public class vmForumPoll
    {
        public Poll Poll { get; set; }
        public bool UserHasAlreadyVoted { get; set; }
        public int TotalVotesInPoll { get; set; }
        public bool UserAllowedToVote { get; set; }
    }

    public class vmForumEditPost
    {

    }

    public class vmForumPost
    {
        public Post Post { get; set; }
        public string PermaLink { get; set; }
        public Topic ParentTopic { get; set; }
        public int UpVoteCount { get; set; }
        public int DownVoteCount { get; set; }
        public bool AllowedToVote { get; set; }
        public bool HasVotedUp { get; set; }
        public bool HasVotedDown { get; set; }
        public bool MemberHasFavourited { get; set; }
        public int UserIDX { get; set; }
        public string PosterName { get; set; }
        public List<vmPostFileDisplay> PostFiles { get; set; }

    }

    public class vmPostFileDisplay {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public DateTime? DateCreated { get; set; }
        public Guid? PostId { get; set; }
        public string FileDescription { get; set; }
        public int MembershipUser_Id { get; set; }

    }

    public class vmPostDisplayType {
        public Post Post { get; set; }
        public string PosterDisplayName { get; set; }
    }

    public class vmForumMainCategoriesList {
        public List<CategoryDisplay> Categories { get; set; }
    }

    public class vmForumCategoryView {
        public CategoryWithSubCategories CategoryWithSub { get; set; }

        // Topic info
        public List<TopicOverviewDisplay> CategoryTopics { get; set; }

    }

    public class vmForumLatestTopicsView {

        public List<TopicOverviewDisplay> _topics { get; set; }
        public int numRecs { get; set; }
        public int currentPage { get; set; }
    }


    public class vmForumPopularTags {
        public Dictionary<string, int> popularTags { get; set; }
    }

    public class CheckCreateTopicPermissions
    {
        public bool CanUploadFiles { get; set; }
        public bool CanStickyTopic { get; set; }
        public bool CanLockTopic { get; set; }
        public bool CanCreatePolls { get; set; }
        public bool CanInsertImages { get; set; }
    }



    public class vmForumAttachFilesToPost
    {
        public HttpPostedFileBase[] Files { get; set; }
        public Guid UploadPostId { get; set; }
    }

}