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
    
    public partial class MembershipUserPoint
    {
        public System.Guid Id { get; set; }
        public int Points { get; set; }
        public System.DateTime DateAdded { get; set; }
        public int PointsFor { get; set; }
        public Nullable<System.Guid> PointsForId { get; set; }
        public string Notes { get; set; }
        public int MembershipUser_Id { get; set; }
    
        public virtual T_OE_USERS T_OE_USERS { get; set; }
        public virtual T_OE_USERS T_OE_USERS1 { get; set; }
    }
}
