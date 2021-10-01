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
    
    public partial class T_OE_PROJECTS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_OE_PROJECTS()
        {
            this.T_OE_DOCUMENTS = new HashSet<T_OE_DOCUMENTS>();
            this.T_OE_PROJECT_TAGS = new HashSet<T_OE_PROJECT_TAGS>();
            this.T_OE_PROJECT_URLS = new HashSet<T_OE_PROJECT_URLS>();
            this.T_OE_PROJECT_VOTES = new HashSet<T_OE_PROJECT_VOTES>();
            this.T_OE_PROJECT_ORGS = new HashSet<T_OE_PROJECT_ORGS>();
        }
    
        public System.Guid PROJECT_IDX { get; set; }
        public Nullable<System.Guid> ORG_IDX { get; set; }
        public string PROJ_NAME { get; set; }
        public string PROJ_DESC { get; set; }
        public Nullable<int> MEDIA_TAG { get; set; }
        public Nullable<int> START_YEAR { get; set; }
        public string PROJ_STATUS { get; set; }
        public Nullable<int> DATE_LAST_UPDATE { get; set; }
        public string RECORD_SOURCE { get; set; }
        public string PROJECT_URL { get; set; }
        public Nullable<int> MOBILE_IND { get; set; }
        public string MOBILE_DESC { get; set; }
        public Nullable<int> ADV_MON_IND { get; set; }
        public string ADV_MON_DESC { get; set; }
        public Nullable<int> BP_MODERN_IND { get; set; }
        public string BP_MODERN_DESC { get; set; }
        public string COTS { get; set; }
        public string VENDOR { get; set; }
        public bool ACT_IND { get; set; }
        public bool SYNC_IND { get; set; }
        public Nullable<int> CREATE_USERIDX { get; set; }
        public Nullable<System.DateTime> CREATE_DT { get; set; }
        public Nullable<int> MODIFY_USERIDX { get; set; }
        public Nullable<System.DateTime> MODIFY_DT { get; set; }
        public string IMPORT_ID { get; set; }
        public string PROJECT_CONTACT { get; set; }
        public Nullable<int> PROJECT_CONTACT_IDX { get; set; }
        public string PROJ_DESC_HTML { get; set; }
        public Nullable<System.DateTime> LAST_NOTIFY_DT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_OE_DOCUMENTS> T_OE_DOCUMENTS { get; set; }
        public virtual T_OE_ORGANIZATION T_OE_ORGANIZATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_OE_PROJECT_TAGS> T_OE_PROJECT_TAGS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_OE_PROJECT_URLS> T_OE_PROJECT_URLS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_OE_PROJECT_VOTES> T_OE_PROJECT_VOTES { get; set; }
        public virtual T_OE_REF_TAGS T_OE_REF_TAGS { get; set; }
        public virtual T_OE_REF_TAGS T_OE_REF_TAGS1 { get; set; }
        public virtual T_OE_REF_TAGS T_OE_REF_TAGS2 { get; set; }
        public virtual T_OE_REF_TAGS T_OE_REF_TAGS3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_OE_PROJECT_ORGS> T_OE_PROJECT_ORGS { get; set; }
    }
}
