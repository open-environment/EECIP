
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
    
public partial class PollAnswer
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public PollAnswer()
    {

        this.PollVotes = new HashSet<PollVote>();

    }


    public System.Guid Id { get; set; }

    public string Answer { get; set; }

    public System.Guid Poll_Id { get; set; }



    public virtual Poll Poll { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<PollVote> PollVotes { get; set; }

}

}
