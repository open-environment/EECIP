using EECIP.App_Logic.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EECIP.App_Logic.BusinessLogicLayer;
using Microsoft.Azure.Search.Models;
using System.Linq;
using System.ComponentModel;
using System.Web;

namespace EECIP.Models
{
    public class vmDashboardIndex {
        public List<UserBadgeDisplay> UserBadges { get; set; }
        public string UserName { get; set; }
        public int ProjectsNeedingReviewCount { get; set; }
        public List<UserMostPointsDisplay> UserPointLeaders { get; set; }
        public List<ProjectShortDisplayType> LatestProjects { get; set; }
        public List<TopicOverviewDisplay> LatestTopics { get; set; }
        public int ProjectCount { get; set; }
        public int DiscussionCount { get; set; }
        public int AgencyCount { get; set; }
        public int GovernanceCount { get; set; }
        public int UserBadgeEarnedCount { get; set; }
        public string Announcement { get; set; }
    }


    public class vmDashboardSearch
    {
        public DocumentSearchResult<EECIP_Index> searchResults { get; set; }
        public string q { get; set; }
        public string facetDataType { get; set; }
        public string facetMedia { get; set; }
        public string facetRecordSource { get; set; }
        public string facetAgency { get; set; }
        public string facetState { get; set; }
        public string facetTags { get; set; }
        public string facetPopDensity { get; set; }
        public string facetRegion { get; set; }
        public string facetStatus { get; set; }
        public string activeTab { get; set; }
        public int? currentPage { get; set; }
        public string sortType { get; set; }

        public List<T_OE_ORGANIZATION> active_agencies { get; set; }
        public List<T_OE_PROJECTS> active_projects { get; set; }
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

        //enterprise service portion
        public List<OrganizationEntServicesDisplayType> org_ent_services { get; set; }
        public OrganizationEntServicesDisplayType edit_ent_services { get; set; }
        public IEnumerable<SelectListItem> ddl_Status { get; set; }

        //agency filter for admins
        public IEnumerable<SelectListItem> ddl_Agencies { get; set; }

        //initialize
        public vmDashboardAgency()
        {
            ddl_States = ddlHelpers.get_ddl_states();
            ddl_Regions = ddlHelpers.get_ddl_regions();
            ddl_Cloud = ddlHelpers.get_ddl_tags_by_category_stringy("Cloud");
            ddl_API = ddlHelpers.get_ddl_tags_by_category_stringy("API");
            ddl_Status = db_Ref.GetT_OE_REF_TAGS_ByCategory_ProjStatus(false);
            ddl_Agencies = ddlHelpers.get_ddl_organizations(true, true);
        }

    }

    public class vmDashboardAgencyCard
    {
        public T_OE_ORGANIZATION agency { get; set; }
        public List<T_OE_USERS> users { get; set; }
        public List<string> SelectedDatabase { get; set; }
        public List<string> SelectedAppFramework { get; set; }
        public List<OrganizationEntServicesDisplayType> org_ent_services { get; set; }
        public List<T_OE_PROJECTS> projects { get; set; }
    }

    public class vmDashboardProjects
    {
        public List<T_OE_PROJECTS> projects { get; set; }
        public Guid? selAgency { get; set; }
        public string selAgencyName { get; set; }
        public IEnumerable<SelectListItem> ddl_Agencies { get; set; }
    }

    public class vmDashboardProjectReview {
        public List<T_OE_PROJECTS> ProjectsNeedingReview { get; set; }
    }


    public class vmDashboardProjectDetails {
        public string orgName { get; set; }
        public string orgType { get; set; }
        public T_OE_PROJECTS project { get; set; }
        public IEnumerable<T_OE_PROJECT_URLS> sProjectUrlList { get; set; }
        public IEnumerable<SelectListItem> ddl_Media { get; set; }
        public IEnumerable<SelectListItem> ddl_Status { get; set; }
        public IEnumerable<SelectListItem> ddl_UseAmount { get; set; }
        public IEnumerable<SelectListItem> ddl_COTS { get; set; }

        public IEnumerable<SelectListItem> ddl_AgencyUsers { get; set; }

        public IEnumerable<SelectListItem> AllProgramAreas { get; set; }
        public List<string> SelectedProgramAreas { get; set; }
        public IEnumerable<SelectListItem> AllFeatures { get; set; }
        public List<string> SelectedFeatures { get; set; }
        public bool NewProjInd { get; set; }
        [DisplayName("Upload File")]
        public HttpPostedFileBase[] files { get; set; }
        public List<T_OE_DOCUMENTS> files_existing { get; set; }
        public string ReturnURL { get; set; }
        public string FileDescription { get; set; }

        public vmDashboardProjectDetails()
        {
            ddl_Media = ddlHelpers.get_ddl_tags_by_category("Project Media");
            ddl_Status = db_Ref.GetT_OE_REF_TAGS_ByCategory_ProjStatus(true);
            ddl_UseAmount = ddlHelpers.get_ddl_tags_by_category("Use Amount");
            ddl_COTS = ddlHelpers.get_ddl_tags_by_category_stringy("COTS");
        }

    }

    public class vmDashboardProjectCard
    {
        public T_OE_PROJECTS project { get; set; }
        public IEnumerable<T_OE_PROJECT_URLS> sProjectUrlList { get; set; }
        public string OrgName { get; set; }
        public List<string> SelectedProgramAreas { get; set; }
        public List<string> SelectedFeatures { get; set; }
        public string LastUpdatedUser { get; set; }
        public int ProjectVotePoints { get; set; }
        public bool HasVoted { get; set; }
        public bool UserBelongsToProjectAgency { get; set; }
        public T_OE_USERS ProjectContact { get; set; }
        public List<T_OE_DOCUMENTS> files_existing { get; set; }

    }

    public class vmDashboardEntSvcOverview{
        public List<SP_ENT_SVC_COUNT_DISPLAY_Result> EntSvcOverviewDisplay { get; set; }
    }

    public class vmDashboardEntSvcCard
    {
        public OrganizationEntServicesDisplayType entsvc { get; set; }
        public string OrgName { get; set; }
        public string LastUpdatedUser { get; set; }
    }

    public class vmDashboardUserCard {
        public T_OE_USERS User { get; set; }
        public T_OE_ORGANIZATION UserOrg { get; set; }
        public List<string> SelectedExpertise { get; set; }
        public List<UserBadgeDisplay> UserBadges { get; set; }

    }


    public class vmDashboardGovernance {
        public bool UnlockedInd { get; set; }
        public List<T_OE_PROJECTS> projects { get; set; }
        public Guid? selAgency { get; set; }
        public string selAgencyName { get; set; }
        public IEnumerable<SelectListItem> ddl_Agencies { get; set; }
        public string edit_org_name { get; set; }
        public string edit_org_abbr { get; set; }

        //initialize
        public vmDashboardGovernance()
        {
            ddl_Agencies = ddlHelpers.get_ddl_organizations_all_governance();
        }

    }



}