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
    
    public partial class T_OE_ORGANIZATION_TAGS
    {
        public System.Guid ORG_IDX { get; set; }
        public string ORG_ATTRIBUTE { get; set; }
        public int ORG_TAG_IDX { get; set; }
        public Nullable<int> CREATE_USERIDX { get; set; }
        public Nullable<System.DateTime> CREATE_DT { get; set; }
    
        public virtual T_OE_REF_TAGS T_OE_REF_TAGS { get; set; }
        public virtual T_OE_ORGANIZATION T_OE_ORGANIZATION { get; set; }
    }
}
