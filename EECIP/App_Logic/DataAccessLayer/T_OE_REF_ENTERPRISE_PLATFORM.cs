
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
    
public partial class T_OE_REF_ENTERPRISE_PLATFORM
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public T_OE_REF_ENTERPRISE_PLATFORM()
    {

        this.T_OE_ORGANIZATION_ENT_SVCS = new HashSet<T_OE_ORGANIZATION_ENT_SVCS>();

    }


    public int ENT_PLATFORM_IDX { get; set; }

    public string ENT_PLATFORM_NAME { get; set; }

    public string ENT_PLATFORM_DESC { get; set; }

    public string ENT_PLATFORM_EXAMPLE { get; set; }

    public Nullable<int> SEQ_NO { get; set; }

    public bool ACT_IND { get; set; }

    public Nullable<int> MODIFY_USERIDX { get; set; }

    public Nullable<System.DateTime> MODIFY_DT { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<T_OE_ORGANIZATION_ENT_SVCS> T_OE_ORGANIZATION_ENT_SVCS { get; set; }

}

}
