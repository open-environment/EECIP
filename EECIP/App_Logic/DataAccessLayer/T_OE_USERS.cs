//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class T_OE_USERS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_OE_USERS()
        {
            this.T_OE_PROJECT_VOTES = new HashSet<T_OE_PROJECT_VOTES>();
            this.T_OE_USER_EXPERTISE = new HashSet<T_OE_USER_EXPERTISE>();
            this.T_OE_USER_ROLES = new HashSet<T_OE_USER_ROLES>();
            this.Favourites = new HashSet<Favourite>();
            this.MembershipUserPoints = new HashSet<MembershipUserPoint>();
            this.MembershipUser_Badge = new HashSet<MembershipUser_Badge>();
            this.Polls = new HashSet<Poll>();
            this.PollVotes = new HashSet<PollVote>();
            this.PostFiles = new HashSet<PostFile>();
            this.Topics = new HashSet<Topic>();
            this.MembershipUserPoints1 = new HashSet<MembershipUserPoint>();
            this.TopicNotifications = new HashSet<TopicNotification>();
            this.Votes = new HashSet<Vote>();
            this.Votes1 = new HashSet<Vote>();
            this.Posts = new HashSet<Post>();
        }
    
        public int USER_IDX { get; set; }
        public string USER_ID { get; set; }
        public string PWD_HASH { get; set; }
        public string PWD_SALT { get; set; }
        public string FNAME { get; set; }
        public string LNAME { get; set; }
        public string EMAIL { get; set; }
        public bool INITAL_PWD_FLAG { get; set; }
        public System.DateTime EFFECTIVE_DT { get; set; }
        public Nullable<System.DateTime> LASTLOGIN_DT { get; set; }
        public string PHONE { get; set; }
        public string PHONE_EXT { get; set; }
        public string JOB_TITLE { get; set; }
        public bool NODE_ADMIN { get; set; }
        public byte[] USER_AVATAR { get; set; }
        public Nullable<int> LOG_ATMPT { get; set; }
        public Nullable<System.DateTime> LOCKOUT_END_DATE_UTC { get; set; }
        public bool LOCKOUT_ENABLED { get; set; }
        public Nullable<System.Guid> ORG_IDX { get; set; }
        public bool ACT_IND { get; set; }
        public Nullable<int> CREATE_USERIDX { get; set; }
        public Nullable<System.DateTime> CREATE_DT { get; set; }
        public Nullable<int> MODIFY_USERIDX { get; set; }
        public Nullable<System.DateTime> MODIFY_DT { get; set; }
        public bool SYNC_IND { get; set; }
        public string LINKEDIN { get; set; }
        public bool ALLOW_GOVERNANCE { get; set; }
        public bool EXCLUDE_POINTS_IND { get; set; }
        public bool NOTIFY_DISCUSSION_IND { get; set; }
        public bool NOTIFY_BADGE_IND { get; set; }
        public bool NOTIFY_NEWSLETTER { get; set; }
        public bool NEW_USER_EMAIL_IND { get; set; }
        public bool PROJECT_UPDATE_OPTOUT_IND { get; set; }
    
        public virtual T_OE_ORGANIZATION T_OE_ORGANIZATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_OE_PROJECT_VOTES> T_OE_PROJECT_VOTES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_OE_USER_EXPERTISE> T_OE_USER_EXPERTISE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_OE_USER_ROLES> T_OE_USER_ROLES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Favourite> Favourites { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MembershipUserPoint> MembershipUserPoints { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MembershipUser_Badge> MembershipUser_Badge { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Poll> Polls { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PollVote> PollVotes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PostFile> PostFiles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Topic> Topics { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MembershipUserPoint> MembershipUserPoints1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TopicNotification> TopicNotifications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vote> Votes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vote> Votes1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Post> Posts { get; set; }
    }
}
