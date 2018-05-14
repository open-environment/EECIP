using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Collections.Generic;
using System.Web.Mvc;
using System;
using EECIP.App_Logic.DataAccessLayer;

namespace EECIP.Models
{
    public class vmAccountLogin
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class vmAccountRegister
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string UserName { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("UserName", ErrorMessage = "The email and confirmation email do not match.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Confirm email address")]
        public string ConfirmUserName { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        public IEnumerable<SelectListItem> ddl_Agencies { get; set; }

        public Guid? intSelOrgIDX { get; set; }

        public Boolean suggestAgencyInd { get; set; }
       
        public string suggestAgency { get; set; }
        public string termsConditions { get; set; }
    }


    public class vmAccountUserProfile
    {
        public int UserIDX { get; set; }
        public string UserID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }
        public string JobTitle { get; set; }
        public string LinkedIn { get; set; }
        public bool? NodeAdmin { get; set; }
        public bool ActInd { get; set; }
        public IEnumerable<SelectListItem> AllExpertise { get; set; }
        public List<string> SelectedExpertise { get; set; }
        public IEnumerable<SelectListItem> ddl_Agencies { get; set; }
        public Guid? OrgIDX { get; set; }
        public string OrgName  { get; set; }

        public HttpPostedFileBase imageBrowes { get; set; }  //handling the posted file from client (1 direction)
        public bool HasAvatar { get; set; }  //lets client know if an avatar exists in DB
        public string ImageUniqueStr { get; set; }

        public string uListInd { get; set; }
        public bool ExcludeBadges { get; set; }
        //initialize
        public vmAccountUserProfile()
        {
            ddl_Agencies = ddlHelpers.get_ddl_organizations(true, true);
        }
    }

    public class vmAccountChangePassword
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [StringLength(20, MinimumLength = 8)]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class vmAccountLostPassword
    {
        [Required]
        [Display(Name = "Email address")]
        public string Email { get; set; }
    }

    public class vmAccountVerify
    {
        [Required]
        [Display(Name = "Oauth")]
        public string OAuth { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(20, MinimumLength = 8)]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


    public class vmAccountUsers {
        public List<T_OE_USERS> users { get; set;}

    }

    public class vmAccountNotifications
    {
        public List<T_OE_USER_NOTIFICATION> notifications { get; set; }
    }
}