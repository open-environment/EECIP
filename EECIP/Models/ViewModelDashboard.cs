using EECIP.App_Logic.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EECIP.App_Logic.BusinessLogicLayer;
using Microsoft.Azure.Search.Models;

namespace EECIP.Models
{
    public class vmDashboardSearch
    {
        public DocumentSearchResult<EECIP_Index> searchResults { get; set; }
        public string searchStr { get; set; }
    }


    public class vmDashboardAgency
    {
        public T_OE_ORGANIZATION agency { get; set; }
        public List<T_OE_USERS> users { get; set; }
        public IEnumerable<SelectListItem> ddl_States { get; set; }
        public IEnumerable<SelectListItem> ddl_Regions { get; set; }
        public IEnumerable<SelectListItem> ddl_Cloud { get; set; }
        public IEnumerable<SelectListItem> ddl_API { get; set; }
        public IEnumerable<SelectListItem> AllDatabase { get; set; }
        public List<string> SelectedDatabase { get; set; }
        public IEnumerable<SelectListItem> AllAppFramework { get; set; }
        public List<string> SelectedAppFramework { get; set; }



        public List<OrganizationEntServicesDisplayType> org_ent_services { get; set; }
        public OrganizationEntServicesDisplayType edit_ent_services { get; set; }

        //initialize
        public vmDashboardAgency()
        {
            ddl_States = ddlHelpers.get_ddl_states();
            ddl_Regions = ddlHelpers.get_ddl_regions();
            ddl_Cloud = ddlHelpers.get_ddl_tags_by_category_stringy("Cloud");
            ddl_API = ddlHelpers.get_ddl_tags_by_category_stringy("API");
        }

    }


    public class vmDashboardProjects
    {
        public List<T_OE_PROJECTS> projects { get; set; }
    }

    public class vmDashboardProjectDetails {
        public T_OE_PROJECTS project { get; set; }
        public IEnumerable<SelectListItem> ddl_Media { get; set; }
        public IEnumerable<SelectListItem> ddl_Status { get; set; }
        public IEnumerable<SelectListItem> ddl_UseAmount { get; set; }
        public IEnumerable<SelectListItem> ddl_COTS { get; set; }

        public IEnumerable<SelectListItem> AllProgramAreas { get; set; }
        public List<string> SelectedProgramAreas { get; set; }
        public IEnumerable<SelectListItem> AllFeatures { get; set; }
        public List<string> SelectedFeatures { get; set; }
        public vmDashboardProjectDetails()
        {
            ddl_Media = ddlHelpers.get_ddl_tags_by_category("Project Media");
            ddl_Status = ddlHelpers.get_ddl_tags_by_category_stringy("Project Status");
            ddl_UseAmount = ddlHelpers.get_ddl_tags_by_category("Use Amount");
            ddl_COTS = ddlHelpers.get_ddl_tags_by_category_stringy("COTS");
        }

    }


    public class vmDashboardProjectCard
    {
        public T_OE_PROJECTS project { get; set; }
        public List<string> SelectedProgramAreas { get; set; }
        public List<string> SelectedFeatures { get; set; }

    }
}