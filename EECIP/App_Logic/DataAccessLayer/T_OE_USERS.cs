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
            this.T_OE_USER_ROLES = new HashSet<T_OE_USER_ROLES>();
            this.T_OE_USER_EXPERTISE = new HashSet<T_OE_USER_EXPERTISE>();
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
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_OE_USER_ROLES> T_OE_USER_ROLES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_OE_USER_EXPERTISE> T_OE_USER_EXPERTISE { get; set; }
        public virtual T_OE_ORGANIZATION T_OE_ORGANIZATION { get; set; }
    }
}
