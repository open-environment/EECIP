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
    
    public partial class Favourite
    {
        public System.Guid Id { get; set; }
        public System.DateTime DateCreated { get; set; }
        public int MemberId { get; set; }
        public System.Guid PostId { get; set; }
        public System.Guid TopicId { get; set; }
    
        public virtual T_OE_USERS T_OE_USERS { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual Post Post { get; set; }
    }
}
