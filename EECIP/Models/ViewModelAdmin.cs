using System.ComponentModel.DataAnnotations;
using EECIP.App_Logic.DataAccessLayer;
using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel;
using System;

namespace EECIP.Models
{
    public class vmAdminRoleEdit
    {
        public T_OE_ROLES T_OE_ROLES { get; set; }
        public IEnumerable<SelectListItem> Users_In_Role { get; set; }
        public IEnumerable<string> Users_In_Role_Selected { get; set; }
        public IEnumerable<SelectListItem> Users_Not_In_Role { get; set; }
        public IEnumerable<string> Users_Not_In_Role_Selected { get; set; }
    }


    public class vmAdminSettings
    {
        public List<T_OE_APP_SETTINGS> app_settings { get; set; }
        public T_OE_APP_SETTINGS edit_app_setting { get; set; }

        [DisplayName("Terms & Conditions")]
        [UIHint("forumeditor"), AllowHtml]
        [StringLength(10000)]
        public string TermsAndConditions { get; set; }

        [DisplayName("Announcements")]
        [UIHint("forumeditor"), AllowHtml]
        [StringLength(10000)]
        public string Announcements { get; set; }

        [DisplayName("Welcome Email")]
        [UIHint("forumeditor"), AllowHtml]
        [StringLength(10000)]
        public string WelcomeEmail { get; set; }

        [DisplayName("Reminder Email")]
        [UIHint("forumeditor"), AllowHtml]
        [StringLength(10000)]
        public string ReminderEmail { get; set; }
    }


    public class vmAdminUsers
    {
        public List<UserDisplayType> users { get; set; }

        [Required]
        [StringLength(40)]
        public string newUserFName { get; set; }

        [Required]
        [StringLength(40)]
        public string newUserLName { get; set; }

        [Required]
        public string newUserEmail { get; set; }
        public int userCount { get; set; }
        public int? currPage { get; set; }
        public string strSort { get; set; }

        public Guid? selAgency { get; set; }
        public string selAgencyName { get; set; }
        public IEnumerable<SelectListItem> ddl_Agencies { get; set; }

    }


    public class vmAdminAgencies
    {
        public List<T_OE_ORGANIZATION> agencies { get; set; }
        public string GovInd { get; set; }
    }


    public class vmAdminAgencyEdit
    {
        public T_OE_ORGANIZATION agency { get; set; }
        public IEnumerable<SelectListItem> ddl_States { get; set; }
        public IEnumerable<SelectListItem> ddl_Regions { get; set; }
        public IEnumerable<SelectListItem> ddl_OrgTypes { get; set; }

        public List<T_OE_ORGANIZATION_EMAIL_RULE> agency_emails { get; set; }
        public string new_email { get; set; }
        public string GovInd { get; set; }

        //initialize
        public vmAdminAgencyEdit()
        {
            ddl_States = ddlHelpers.get_ddl_states();
            ddl_Regions = ddlHelpers.get_ddl_regions();
            ddl_OrgTypes = ddlHelpers.get_ddl_orgtypes(false);

        }
    }

    public class vmAdminRefEntServices
    {
        public List<T_OE_REF_ENTERPRISE_PLATFORM> ent_services { get; set; }
        public T_OE_REF_ENTERPRISE_PLATFORM edit_ent_services { get; set; }
    }

    public class vmAdminRefTags
    {
        public List<T_OE_REF_TAGS> tags { get; set; }
        public IEnumerable<SelectListItem> ddl_tag_cats { get; set; }
        public string sel_tag_cat { get; set; }
        public string sel_tag_cat_desc { get; set; }
        public int? edit_tag_idx { get; set; }
        public string edit_tag { get; set; }
        public string edit_tag_desc { get; set; }
        public bool edit_promote_ind { get; set; }


        //INITIALIZE
        public vmAdminRefTags()
        {
            ddl_tag_cats = ddlHelpers.get_ddl_tag_cats();
        }
    }

    public class vmAdminRefTagsUser
    {
        public List<USER_DEFINED_TAGS> user_tags { get; set; }
        public IEnumerable<SelectListItem> ddl_tag_cats { get; set; }
        public IEnumerable<SelectListItem> ddl_tags { get; set; }
        public string sel_tag_cat { get; set; }


        public string edit_tag { get; set; }
        public string edit_tag_cat { get; set; }
        public string edit_new_tag { get; set; }



        //INITIALIZE
        public vmAdminRefTagsUser()
        {
            ddl_tag_cats = ddlHelpers.get_ddl_tag_cats();
            ddl_tags = ddlHelpers.get_ddl_tags_by_category_stringy("Tags");

        }
    }

    public class vmAdminRefBadges
    {
        public List<Badge> _badge { get; set; }
        public Badge edit_badge { get; set; }
    }


    public class vmAdminSysLog
    {
        public List<T_OE_SYS_LOG> T_OE_SYS_LOG { get; set; }
        public List<T_OE_SYS_EMAIL_LOG> T_OE_SYS_EMAIL_LOG { get; set; }
    }

    public class vmAdminEmail
    {
        public List<T_OE_REF_EMAIL_TEMPLATE> T_OE_REF_EMAIL_TEMPLATE { get; set; }
    }

    public class vmAdminSearch {
        public List<T_OE_REF_SYNONYMS> synonyms { get; set; }
        public int? edit_synonym_idx { get; set; }
        public string edit_synonym_text { get; set; }
        public string edit_synonym_bulk { get; set; }
    }

    public class vmAdminImportData {
        public string IMPORT_BLOCK { get; set; }  //raw text imported
        public List<ProjectImportType> projects { get; set; }   //in-memory storage of array of projects to import
    }

    public class vmAdminNewsletter
    {
        public List<string> results { get; set; }
    }

    public class vmAdminReminder
    {
        public List<T_OE_PROJECTS> projects { get; set; }
    }

}