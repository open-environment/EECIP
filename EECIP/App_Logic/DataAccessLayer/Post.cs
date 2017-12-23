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
    
    public partial class Post
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Post()
        {
            this.Topics = new HashSet<Topic>();
            this.Votes = new HashSet<Vote>();
            this.Favourites = new HashSet<Favourite>();
        }
    
        public System.Guid Id { get; set; }
        public string PostContent { get; set; }
        public System.DateTime DateCreated { get; set; }
        public int VoteCount { get; set; }
        public System.DateTime DateEdited { get; set; }
        public bool IsSolution { get; set; }
        public Nullable<bool> IsTopicStarter { get; set; }
        public Nullable<bool> FlaggedAsSpam { get; set; }
        public string IpAddress { get; set; }
        public Nullable<bool> Pending { get; set; }
        public string SearchField { get; set; }
        public Nullable<System.Guid> InReplyTo { get; set; }
        public System.Guid Topic_Id { get; set; }
        public int MembershipUser_Id { get; set; }
    
        public virtual T_OE_USERS T_OE_USERS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Topic> Topics { get; set; }
        public virtual Topic Topic { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vote> Votes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Favourite> Favourites { get; set; }
    }
}