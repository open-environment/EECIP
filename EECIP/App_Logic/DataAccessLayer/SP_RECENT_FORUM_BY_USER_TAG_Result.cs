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
    
    public partial class SP_RECENT_FORUM_BY_USER_TAG_Result
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string PostContent { get; set; }
        public string FNAME { get; set; }
        public string LNAME { get; set; }
        public int TopicUserIDX { get; set; }
        public Nullable<System.DateTime> LatestPostDate { get; set; }
        public string LatestPostUser { get; set; }
        public Nullable<int> LatestPostUserIDX { get; set; }
    }
}