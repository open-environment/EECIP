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
    
    public partial class STALE_PROJECTS_WITH_CONTACTS
    {
        public System.Guid PROJECT_IDX { get; set; }
        public string PROJ_NAME { get; set; }
        public string PROJ_STATUS { get; set; }
        public string ORG { get; set; }
        public string FNAME { get; set; }
        public string LNAME { get; set; }
        public string EMAIL { get; set; }
        public Nullable<System.DateTime> CREAT_UPDATE_DT { get; set; }
        public Nullable<System.DateTime> PROJECT_REMIND_DT { get; set; }
        public Nullable<System.DateTime> TRU_LAST_DT { get; set; }
        public Nullable<System.DateTime> TRU_NEXT_DT { get; set; }
    }
}
