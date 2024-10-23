﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EECIP.App_Logic.DataAccessLayer
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class EECIPEntities : DbContext
    {
        public EECIPEntities()
            : base("name=EECIPEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<T_OE_APP_SETTINGS> T_OE_APP_SETTINGS { get; set; }
        public virtual DbSet<T_OE_APP_SETTINGS_CUSTOM> T_OE_APP_SETTINGS_CUSTOM { get; set; }
        public virtual DbSet<T_OE_DOCUMENTS> T_OE_DOCUMENTS { get; set; }
        public virtual DbSet<T_OE_ORGANIZATION> T_OE_ORGANIZATION { get; set; }
        public virtual DbSet<T_OE_ORGANIZATION_EMAIL_RULE> T_OE_ORGANIZATION_EMAIL_RULE { get; set; }
        public virtual DbSet<T_OE_ORGANIZATION_ENT_SVCS> T_OE_ORGANIZATION_ENT_SVCS { get; set; }
        public virtual DbSet<T_OE_ORGANIZATION_TAGS> T_OE_ORGANIZATION_TAGS { get; set; }
        public virtual DbSet<T_OE_PROJECT_TAGS> T_OE_PROJECT_TAGS { get; set; }
        public virtual DbSet<T_OE_PROJECT_URLS> T_OE_PROJECT_URLS { get; set; }
        public virtual DbSet<T_OE_PROJECT_VOTES> T_OE_PROJECT_VOTES { get; set; }
        public virtual DbSet<T_OE_REF_ENTERPRISE_PLATFORM> T_OE_REF_ENTERPRISE_PLATFORM { get; set; }
        public virtual DbSet<T_OE_REF_ORG_TYPE> T_OE_REF_ORG_TYPE { get; set; }
        public virtual DbSet<T_OE_REF_REGION> T_OE_REF_REGION { get; set; }
        public virtual DbSet<T_OE_REF_STATE> T_OE_REF_STATE { get; set; }
        public virtual DbSet<T_OE_REF_SYNONYMS> T_OE_REF_SYNONYMS { get; set; }
        public virtual DbSet<T_OE_REF_TAG_CATEGORIES> T_OE_REF_TAG_CATEGORIES { get; set; }
        public virtual DbSet<T_OE_REF_TAGS> T_OE_REF_TAGS { get; set; }
        public virtual DbSet<T_OE_ROLES> T_OE_ROLES { get; set; }
        public virtual DbSet<T_OE_SYS_EMAIL_LOG> T_OE_SYS_EMAIL_LOG { get; set; }
        public virtual DbSet<T_OE_SYS_LOG> T_OE_SYS_LOG { get; set; }
        public virtual DbSet<T_OE_USER_EXPERTISE> T_OE_USER_EXPERTISE { get; set; }
        public virtual DbSet<T_OE_USER_NOTIFICATION> T_OE_USER_NOTIFICATION { get; set; }
        public virtual DbSet<T_OE_USER_ROLES> T_OE_USER_ROLES { get; set; }
        public virtual DbSet<T_OE_USERS> T_OE_USERS { get; set; }
        public virtual DbSet<Badge> Badges { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Favourite> Favourites { get; set; }
        public virtual DbSet<MembershipUser_Badge> MembershipUser_Badge { get; set; }
        public virtual DbSet<MembershipUserPoint> MembershipUserPoints { get; set; }
        public virtual DbSet<Poll> Polls { get; set; }
        public virtual DbSet<PollAnswer> PollAnswers { get; set; }
        public virtual DbSet<PollVote> PollVotes { get; set; }
        public virtual DbSet<PostFile> PostFiles { get; set; }
        public virtual DbSet<Topic> Topics { get; set; }
        public virtual DbSet<Topic_Tags> Topic_Tags { get; set; }
        public virtual DbSet<TopicNotification> TopicNotifications { get; set; }
        public virtual DbSet<Vote> Votes { get; set; }
        public virtual DbSet<T_OE_REF_EMAIL_TEMPLATE> T_OE_REF_EMAIL_TEMPLATE { get; set; }
        public virtual DbSet<T_OE_SYS_SEARCH_LOG> T_OE_SYS_SEARCH_LOG { get; set; }
        public virtual DbSet<T_OE_RPT_FRESHNESS> T_OE_RPT_FRESHNESS { get; set; }
        public virtual DbSet<T_OE_PROJECT_ORGS> T_OE_PROJECT_ORGS { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<RPT_MON_PROJ_VOTE> RPT_MON_PROJ_VOTE { get; set; }
        public virtual DbSet<RPT_MON_TOPIC_VOTE> RPT_MON_TOPIC_VOTE { get; set; }
        public virtual DbSet<USER_DEFINED_TAGS> USER_DEFINED_TAGS { get; set; }
        public virtual DbSet<T_OE_PROJECTS> T_OE_PROJECTS { get; set; }
        public virtual DbSet<STALE_PROJECTS_WITH_CONTACTS> STALE_PROJECTS_WITH_CONTACTS { get; set; }
    
        public virtual ObjectResult<SP_ENT_SVC_COUNT_DISPLAY_Result> SP_ENT_SVC_COUNT_DISPLAY()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_ENT_SVC_COUNT_DISPLAY_Result>("SP_ENT_SVC_COUNT_DISPLAY");
        }
    
        public virtual ObjectResult<SP_NEW_CONTENT_USER_AGE_Result> SP_NEW_CONTENT_USER_AGE()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_NEW_CONTENT_USER_AGE_Result>("SP_NEW_CONTENT_USER_AGE");
        }
    
        public virtual ObjectResult<SP_DISCUSSION_CREATE_COUNT_Result> SP_DISCUSSION_CREATE_COUNT()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_DISCUSSION_CREATE_COUNT_Result>("SP_DISCUSSION_CREATE_COUNT");
        }
    
        public virtual ObjectResult<SP_PROJECT_CREATE_COUNT_Result> SP_PROJECT_CREATE_COUNT()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_PROJECT_CREATE_COUNT_Result>("SP_PROJECT_CREATE_COUNT");
        }
    
        public virtual int SP_RPT_FRESHNESS_RECORD()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_RPT_FRESHNESS_RECORD");
        }
    
        public virtual ObjectResult<SP_RECENT_FORUM_BY_USER_TAG_Result> SP_RECENT_FORUM_BY_USER_TAG(Nullable<int> useridx, Nullable<int> daysSince, string tagFilter)
        {
            var useridxParameter = useridx.HasValue ?
                new ObjectParameter("useridx", useridx) :
                new ObjectParameter("useridx", typeof(int));
    
            var daysSinceParameter = daysSince.HasValue ?
                new ObjectParameter("daysSince", daysSince) :
                new ObjectParameter("daysSince", typeof(int));
    
            var tagFilterParameter = tagFilter != null ?
                new ObjectParameter("tagFilter", tagFilter) :
                new ObjectParameter("tagFilter", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_RECENT_FORUM_BY_USER_TAG_Result>("SP_RECENT_FORUM_BY_USER_TAG", useridxParameter, daysSinceParameter, tagFilterParameter);
        }
    
        public virtual ObjectResult<SP_RECENT_FORUM_BY_USER_TAG_Result> SP_RECENT_FORUM_FALLBACK(Nullable<int> daysSince)
        {
            var daysSinceParameter = daysSince.HasValue ?
                new ObjectParameter("daysSince", daysSince) :
                new ObjectParameter("daysSince", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_RECENT_FORUM_BY_USER_TAG_Result>("SP_RECENT_FORUM_FALLBACK", daysSinceParameter);
        }
    }
}
