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
    
    public partial class T_OE_ORGANIZATION_ENT_SVCS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_OE_ORGANIZATION_ENT_SVCS()
        {
            this.T_OE_PROJECT_VOTES = new HashSet<T_OE_PROJECT_VOTES>();
            this.T_OE_DOCUMENTS = new HashSet<T_OE_DOCUMENTS>();
        }
    
        public int ORG_ENT_SVCS_IDX { get; set; }
        public System.Guid ORG_IDX { get; set; }
        public int ENT_PLATFORM_IDX { get; set; }
        public string PROJECT_NAME { get; set; }
        public string VENDOR { get; set; }
        public string IMPLEMENT_STATUS { get; set; }
        public string COMMENTS { get; set; }
        public Nullable<int> CREATE_USERIDX { get; set; }
        public Nullable<System.DateTime> CREATE_DT { get; set; }
        public Nullable<int> MODIFY_USERIDX { get; set; }
        public Nullable<System.DateTime> MODIFY_DT { get; set; }
        public bool SYNC_IND { get; set; }
        public string RECORD_SOURCE { get; set; }
        public string PROJECT_CONTACT { get; set; }
        public Nullable<bool> ACTIVE_INTEREST_IND { get; set; }
    
        public virtual T_OE_REF_ENTERPRISE_PLATFORM T_OE_REF_ENTERPRISE_PLATFORM { get; set; }
        public virtual T_OE_ORGANIZATION T_OE_ORGANIZATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_OE_PROJECT_VOTES> T_OE_PROJECT_VOTES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_OE_DOCUMENTS> T_OE_DOCUMENTS { get; set; }
    }
}
