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
    
    public partial class PostFile
    {
        public System.Guid Id { get; set; }
        public string Filename { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<System.Guid> Post_Id { get; set; }
        public byte[] FileContent { get; set; }
        public string FileDecription { get; set; }
        public int MembershipUser_Id { get; set; }
    
        public virtual T_OE_USERS T_OE_USERS { get; set; }
        public virtual Post Post { get; set; }
    }
}
