using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EECIP.Models;
using EECIP.App_Logic.BusinessLogicLayer;
using EECIP.App_Logic.DataAccessLayer;

namespace EECIP.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        #region DASHBOARD    
        
        // GET: Dashboard
        public ActionResult Index()
        {
            int UserIDX = db_Accounts.GetUserIDX();

            var model = new vmDashboardIndex();
            model.UserBadges = db_Forum.GetBadgesForUser(UserIDX);
            model.ProjectsNeedingReview = db_EECIP.GetT_OE_PROJECTS_NeedingReview(UserIDX);
            T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(UserIDX);
            if (u != null)
            {
                model.UserName = u.FNAME + " " + u.LNAME;
            }
            return View(model);
        }

        // GET: Dashboard/Search
        public ActionResult Search(string q, string facetDataType, string facetMedia, string facetRecordSource, string facetAgency, string facetState, string facetTags, string activeTab, string currentPage, string sortType)
        {
            var model = new vmDashboardSearch();
            model.q = q;
            model.facetDataType = facetDataType;
            model.facetMedia = facetMedia;
            model.facetRecordSource = facetRecordSource;
            model.facetAgency = facetAgency;
            model.facetState = facetState;
            model.facetTags = facetTags;
            model.activeTab = activeTab ?? "1";
            model.currentPage = currentPage.ConvertOrDefault<int?>() ?? 1;
            model.sortType = sortType;

            model.searchResults = AzureSearch.QuerySearchIndex(model.q, model.facetDataType, model.facetMedia, model.facetRecordSource, model.facetAgency, model.facetState, model.facetTags, model.currentPage, model.sortType);
            return View(model);
        }

        // POST: Dashboard/Search
        //[HttpPost]
        //public ActionResult Search(vmDashboardSearch model)
        //{
        //    if (model.currentPage == null)
        //        model.currentPage = 1;

        //    model.searchResults = AzureSearch.QuerySearchIndex(model.q, model.facetDataType, model.facetMedia, model.facetRecordSource, model.facetAgency, model.facetState, model.facetTags, model.currentPage);
        //    return View(model);
        //}



        // GET: Agency

        #endregion


        #region AGENCY

        // GET: /Dashboard/Agency
        public ActionResult Agency(Guid? selAgency)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            if (selAgency == null || selAgency == Guid.Empty)
            {
                // get agency for which the logged in user is associated
                T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(UserIDX);
                if (u != null && u.ORG_IDX != null)
                    selAgency = u.ORG_IDX.ConvertOrDefault<Guid>();
            }

            //if still no agency
            if (selAgency == null || selAgency == Guid.Empty)
            {
                TempData["Error"] = "You are not associated with an agency.";
                return RedirectToAction("AccessDenied", "Home");
            }

            if (!User.IsInRole("Admins") && !db_Accounts.UserCanEditOrgIDX(UserIDX, selAgency.ConvertOrDefault<Guid>()))
            {
                TempData["Error"] = "You cannot edit this agency.";
                return RedirectToAction("AccessDenied", "Home");
            }


            var model = new vmDashboardAgency();
            model.agency = db_Ref.GetT_OE_ORGANIZATION_ByID(selAgency.ConvertOrDefault<Guid>());
            model.users = db_Accounts.GetT_OE_USERSByAgency(selAgency.ConvertOrDefault<Guid>());
            //database
            model.SelectedDatabase = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByOrgAttribute(selAgency.ConvertOrDefault<Guid>(), "Database");
            model.AllDatabase = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByAttributeAll(selAgency.ConvertOrDefault<Guid>(), "Database").Select(x => new SelectListItem { Value = x, Text = x });
            //app framework
            model.SelectedAppFramework = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByOrgAttribute(selAgency.ConvertOrDefault<Guid>(), "App Framework");
            model.AllAppFramework = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByAttributeAll(selAgency.ConvertOrDefault<Guid>(), "App Framework").Select(x => new SelectListItem { Value = x, Text = x });
            //ent services
            model.org_ent_services = db_EECIP.GetT_OE_ORGANIZATION_ENTERPRISE_PLATFORM(selAgency.ConvertOrDefault<Guid>());

            return View(model);

        }

        // POST: /Dashboard/AgencyEdit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AgencyEdit(vmDashboardAgency model)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            //update general agency data
            Guid? SuccID = db_Ref.InsertUpdatetT_OE_ORGANIZATION(model.agency.ORG_IDX, null, null, null, null, null, model.agency.CLOUD, model.agency.API, true, UserIDX);


            //update database tags
            db_Ref.DeleteT_OE_ORGANIZATION_TAGS(model.agency.ORG_IDX, "Database");
            foreach (string expertise in model.SelectedDatabase ?? new List<string>())
                db_Ref.InsertT_OE_ORGANIZATION_TAGS(model.agency.ORG_IDX, "Database", expertise);


            //update app framework tags
            db_Ref.DeleteT_OE_ORGANIZATION_TAGS(model.agency.ORG_IDX, "App Framework");
            foreach (string expertise in model.SelectedAppFramework ?? new List<string>())
                db_Ref.InsertT_OE_ORGANIZATION_TAGS(model.agency.ORG_IDX, "App Framework", expertise);

            //now update the Azure search
            AzureSearch.PopulateSearchIndexOrganization(SuccID);


            if (SuccID != null)
                TempData["Success"] = "Update successful.";
            else
                TempData["Error"] = "Error updating data.";

            return RedirectToAction("Agency", new { selAgency = model.agency.ORG_IDX });
        }

        // POST: /Dashboard/AgencyEntServiceEdit
        [HttpPost]
        public ActionResult AgencyEntServiceEdit(vmDashboardAgency model)
        {
            if (ModelState.IsValid)
            {
                var z = model.edit_ent_services;
                int SuccID = db_EECIP.InsertUpdatetT_OE_ORGANIZATION_ENT_SVCS(z.ENT_PLATFORM_IDX, model.agency.ORG_IDX, z.ENT_PLATFORM_IDX, z.PROJECT_NAME, z.VENDOR, z.IMPLEMENT_STATUS,
                    z.COMMENTS, z.PROJECT_CONTACT, z.ACTIVE_INTEREST_IND, false, db_Accounts.GetUserIDX());
                if (SuccID > 0)
                {
                    //sync to search service
                    AzureSearch.PopulateSearchIndexEntServices(SuccID);
                    TempData["Success"] = "Data Saved.";
                }
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("Agency", "Dashboard", new { selAgency = model.agency.ORG_IDX });
        }
        
        // POST: /Dashboard/AgencyEntServicesDelete
        [HttpPost]
        public JsonResult AgencyEntServiceDelete(int id)
        {
            int SuccID = db_EECIP.DeleteT_OE_ORGANIZATION_ENT_SVCS(id);
            if (SuccID == 0)
                return Json("Unable to delete record.");
            else
            {
                //now delete from Azure
                AzureSearch.DeleteSearchIndexEntService(id);
                return Json("Success");
            }
        }
        
        // GET: /Dashboard/AgencyCard
        public ActionResult AgencyCard(string strid)
        {
            Guid id;
            if (Guid.TryParse(strid, out id))
            {
                var model = new vmDashboardAgencyCard();
                model.agency = db_Ref.GetT_OE_ORGANIZATION_ByID(id);
                model.org_ent_services = db_EECIP.GetT_OE_ORGANIZATION_ENT_SVCS_NoLeftJoin(id);
                model.users = db_Accounts.GetT_OE_USERSByAgency(id);
                model.SelectedDatabase = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByOrgAttribute(id, "Database");
                model.SelectedAppFramework = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByOrgAttribute(id, "App Framework");
                model.projects = db_EECIP.GetT_OE_PROJECTS_ByOrgIDX(id);

                if (model.agency != null)
                    return View(model);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        #endregion


        #region PROJECT
        
        // GET: /Dashboard/Projects
        public ActionResult Projects(Guid? selAgency)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            if (selAgency == null || selAgency == Guid.Empty)
            {
                // get agency for which the logged in user is associated
                T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(UserIDX);
                if (u != null && u.ORG_IDX != null)
                    selAgency = u.ORG_IDX.ConvertOrDefault<Guid>();                    
            }

            //if still no agency
            if (selAgency == null || selAgency == Guid.Empty)
            {
                TempData["Error"] = "You are not associated with an agency.";
                return RedirectToAction("AccessDenied", "Home");
            }

            if (!User.IsInRole("Admins") && !db_Accounts.UserCanEditOrgIDX(UserIDX, selAgency.ConvertOrDefault<Guid>()))
            {
                TempData["Error"] = "You are not associated with an agency.";
                return RedirectToAction("AccessDenied", "Home");
            }


            var model = new vmDashboardProjects();
            T_OE_ORGANIZATION o = db_Ref.GetT_OE_ORGANIZATION_ByID(selAgency.ConvertOrDefault<Guid>());
                if (o != null)
                    model.selAgencyName = o.ORG_NAME;

            model.projects = db_EECIP.GetT_OE_PROJECTS_ByOrgIDX(selAgency.ConvertOrDefault<Guid>());
            model.selAgency = selAgency;
            return View(model);
        }

        // GET: /Dashboard/ProjectDetails/1
        public ActionResult ProjectDetails(Guid? id, Guid? orgIDX)
        {
            if (User.IsInRole("Admins") || db_Accounts.UserCanEditOrgIDX(db_Accounts.GetUserIDX(), orgIDX.ConvertOrDefault<Guid>()))
            {
                var model = new vmDashboardProjectDetails();
                model.project = db_EECIP.GetT_OE_PROJECTS_ByIDX(id);
                if (model.project == null)
                {
                    //case: new project
                    model.project = new T_OE_PROJECTS
                    {
                        ORG_IDX = orgIDX,
                        PROJECT_IDX = Guid.NewGuid()
                    };
                }
                else
                {
                    model.SelectedProgramAreas = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeSelected(model.project.PROJECT_IDX, "Program Area");
                    model.SelectedFeatures = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeSelected(model.project.PROJECT_IDX, "Project Feature");
                }
                model.AllProgramAreas = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeAll(model.project.PROJECT_IDX, "Program Area").Select(x => new SelectListItem { Value = x, Text = x });
                model.AllFeatures = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeAll(model.project.PROJECT_IDX, "Project Feature").Select(x => new SelectListItem { Value = x, Text = x });
                return View(model);
            }
            else
            {
                TempData["Error"] = "You cannot edit projects for this agency.";
                return RedirectToAction("AccessDenied", "Home");
            }
        }

        // POST: /Dashboard/ProjectEdit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ProjectEdit(vmDashboardProjectDetails model)
        {
            //CHECK PERMISSIONS
            if (User.IsInRole("Admins") || db_Accounts.UserCanEditOrgIDX(db_Accounts.GetUserIDX(), model.project.ORG_IDX.ConvertOrDefault<Guid>()))
            {
                //update project data
                Guid? SuccID = db_EECIP.InsertUpdatetT_OE_PROJECTS(model.project.PROJECT_IDX, model.project.ORG_IDX, model.project.PROJ_NAME,
                    model.project.PROJ_DESC, model.project.MEDIA_TAG, model.project.START_YEAR, model.project.PROJ_STATUS,
                    model.project.DATE_LAST_UPDATE, model.project.RECORD_SOURCE, model.project.PROJECT_URL, model.project.MOBILE_IND,
                    model.project.MOBILE_DESC, model.project.ADV_MON_IND, model.project.ADV_MON_DESC, model.project.BP_MODERN_IND,
                    model.project.BP_MODERN_DESC, model.project.COTS, model.project.VENDOR, model.project.PROJECT_CONTACT, true, false, db_Accounts.GetUserIDX());

                if (SuccID != null)
                {
                    //update program area tags
                    db_EECIP.DeleteT_OE_PROJECT_TAGS(model.project.PROJECT_IDX, "Program Area");
                    foreach (string expertise in model.SelectedProgramAreas ?? new List<string>())
                        db_EECIP.InsertT_OE_PROJECT_TAGS(model.project.PROJECT_IDX, "Program Area", expertise);


                    //update feature tags
                    db_EECIP.DeleteT_OE_PROJECT_TAGS(model.project.PROJECT_IDX, "Project Feature");
                    foreach (string feature in model.SelectedFeatures ?? new List<string>())
                        db_EECIP.InsertT_OE_PROJECT_TAGS(model.project.PROJECT_IDX, "Project Feature", feature);

                    //now update the Azure search
                    AzureSearch.PopulateSearchIndexProject(SuccID);

                    TempData["Success"] = "Update successful.";
                }
                else
                    TempData["Error"] = "Error updating data.";
            }
            else
                TempData["Error"] = "Error updating data.";

            return RedirectToAction("Projects", new { selAgency = model.project.ORG_IDX } );
        }

        // POST: /Dashboard/ProjectsDelete
        [HttpPost]
        public JsonResult ProjectsDelete(Guid id)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            //CHECK PERMISSIONS
            T_OE_PROJECTS p = db_EECIP.GetT_OE_PROJECTS_ByIDX(id);
            if (p != null)
            {
                if (User.IsInRole("Admins") || db_Accounts.UserCanEditOrgIDX(db_Accounts.GetUserIDX(), p.ORG_IDX.ConvertOrDefault<Guid>()))
                {
                    int SuccID = db_EECIP.DeleteT_OE_PROJECTS(id);
                    if (SuccID > 0)
                    {
                        //SUCCESS - now delete from Azure
                        AzureSearch.DeleteSearchIndexProject(id);
                        return Json("Success");
                    }
                }
            }

            //if got this far, general error
            return Json("Unable to delete project.");
        }

        // GET: /Dashboard/ProjectCard/1
        public ActionResult ProjectCard(string strid)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            Guid id;
            if (Guid.TryParse(strid, out id))
            {
                var model = new vmDashboardProjectCard();
                model.project = db_EECIP.GetT_OE_PROJECTS_ByIDX(id);
                if (model.project != null)
                {
                    T_OE_ORGANIZATION _org = db_Ref.GetT_OE_ORGANIZATION_ByID(model.project.ORG_IDX.ConvertOrDefault<Guid>());
                    if (_org != null)
                        model.OrgName = _org.ORG_NAME;

                    model.SelectedProgramAreas = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeSelected(model.project.PROJECT_IDX, "Program Area");
                    model.SelectedFeatures = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeSelected(model.project.PROJECT_IDX, "Project Feature");
                    T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(model.project.MODIFY_USERIDX ?? model.project.CREATE_USERIDX ?? -1);
                    if (u != null)
                        model.LastUpdatedUser = u.FNAME + " " + u.LNAME;
                    model.ProjectVotePoints = db_EECIP.GetT_OE_PROJECT_VOTES_TotalByProject(model.project.PROJECT_IDX);
                    model.HasVoted = db_EECIP.GetT_OE_PROJECT_VOTES_HasVoted(model.project.PROJECT_IDX, UserIDX);
                    model.UserBelongsToProjectAgency = db_Accounts.UserCanEditOrgIDX(db_Accounts.GetUserIDX(), model.project.ORG_IDX.ConvertOrDefault<Guid>());
                }

                if (model.project != null)
                    return View(model);
            }

            TempData["Error"] = "No project found";
            return RedirectToAction("Index", "Dashboard");
        }

        // POST: /Dashboard/ProjectVote
        [HttpPost]
        public JsonResult ProjectVote(Guid? id, string typ)
        {
            int UserIDX = db_Accounts.GetUserIDX();
            bool SuccInd = false;

            if (typ == "up")
            {
                Guid? VoteID = db_EECIP.InsertT_OE_PROJECT_VOTES(id.ConvertOrDefault<Guid>(), UserIDX, 1);
                SuccInd = (VoteID != null);
            }
            else if (typ == "removeup") {
                int SuccID = db_EECIP.DeleteT_OE_PROJECT_VOTE(id.ConvertOrDefault<Guid>(), UserIDX);
                SuccInd = (SuccID == 1);
            }
            else
                return Json(new { msg = "Unable to record vote." });

            if (SuccInd)
            {
                string votes = db_EECIP.GetT_OE_PROJECT_VOTES_TotalByProject(id.ConvertOrDefault<Guid>()).ToString();
                return Json(new { msg = "Success", val = votes });
            }
            else
                return Json(new { msg = "Unable to record vote." });

        }

        #endregion


        public ActionResult EnterpriseSvcCard(string strid)
        {
            int UserIDX = strid.ConvertOrDefault<int>();
            int id = strid.ConvertOrDefault<int>();
            if (id > 100000)
                id = id - 100000;

            var model = new vmDashboardEntSvcCard();
            model.entsvc = db_EECIP.GetT_OE_ORGANIZATION_ENT_SVCS_ByID(id);
            if (model.entsvc != null)
            {
                T_OE_ORGANIZATION _org = db_Ref.GetT_OE_ORGANIZATION_ByID(model.entsvc.ORG_IDX.ConvertOrDefault<Guid>());
                if (_org != null)
                    model.OrgName = _org.ORG_NAME;

                T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(model.entsvc.MODIFY_USERIDX ?? model.entsvc.CREATE_USERIDX ?? -1);
                if (u != null)
                    model.LastUpdatedUser = u.FNAME + " " + u.LNAME;
            }
            //model.User = db_Accounts.GetT_OE_USERSByIDX(UserIDX);
            //model.UserOrg = db_Ref.GetT_OE_ORGANIZATION_ByID(model.User.ORG_IDX.ConvertOrDefault<Guid>());

            //            if (model.User != null)
            return View(model);

            //return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult UserCard(string strid)
        {
            int UserIDX = strid.ConvertOrDefault<int>();

            var model = new vmDashboardUserCard();
            model.User = db_Accounts.GetT_OE_USERSByIDX(UserIDX);
            model.UserOrg = db_Ref.GetT_OE_ORGANIZATION_ByID(model.User.ORG_IDX.ConvertOrDefault<Guid>());
            model.SelectedExpertise = db_EECIP.GetT_OE_USER_EXPERTISE_ByUserIDX(UserIDX);
            model.UserBadges = db_Forum.GetBadgesForUserNoLeftJoin(UserIDX);

            if (model.User != null && model.User.USER_IDX>0)
                return View(model);

            return RedirectToAction("Index", "Dashboard");
        }

    }
}