using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EECIP.Models;
using EECIP.App_Logic.BusinessLogicLayer;
using EECIP.App_Logic.DataAccessLayer;
using System.Web;
using System.IO;

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
            model.UserBadges = db_Forum.GetBadgesForUser(UserIDX);  //badge progress
            model.ProjectsNeedingReviewCount = db_EECIP.GetT_OE_PROJECTS_NeedingReviewCount(UserIDX);  //projects needing review
            model.UserPointLeaders = db_Forum.GetMembershipUserPoints_MostPoints(6);  //user point leaders
            model.LatestProjects = db_EECIP.GetT_OE_PROJECTS_RecentlyUpdatedMatchingInterest(UserIDX);  //latest projects
            model.LatestTopics = db_Forum.GetLatestTopicPostsMatchingInterest(UserIDX); //latest topics matching interest
            model.ProjectCount = db_EECIP.GetT_OE_PROJECTS_CountNonGovernance();
            model.GovernanceCount = db_EECIP.GetT_OE_PROJECTS_CountGovernance();
            model.DiscussionCount = db_Forum.GetTopicCount();
            model.AgencyCount = db_Ref.GetT_OE_ORGANIZATION_Count();
            model.UserBadgeEarnedCount = db_Forum.GetBadgesForUserCount(UserIDX);
            model.Announcement = db_Ref.GetT_OE_APP_SETTING_CUSTOM().ANNOUNCEMENTS;
            T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(UserIDX);
            if (u != null)
            {
                model.UserName = u.FNAME + " " + u.LNAME;
            }
            return View(model);
        }

        // GET: Dashboard/Search
        public ActionResult Search(string q, string facetDataType, string facetMedia, string facetRecordSource, string facetAgency, string facetState, string facetTags, string facetPopDensity, string facetRegion, string facetStatus, string activeTab, string currentPage, string sortType)
        {
            var model = new vmDashboardSearch();
            model.q = q;
            model.facetDataType = facetDataType;
            model.facetMedia = facetMedia;
            model.facetRecordSource = facetRecordSource;
            model.facetAgency = facetAgency;
            model.facetState = facetState;
            model.facetTags = facetTags;
            model.facetPopDensity = facetPopDensity;
            model.facetRegion = facetRegion;
            model.facetStatus = facetStatus;
            model.activeTab = activeTab ?? "1";
            model.currentPage = currentPage.ConvertOrDefault<int?>() ?? 1;
            model.sortType = sortType;

            model.searchResults = AzureSearch.QuerySearchIndex(model.q, model.facetDataType, model.facetMedia, model.facetRecordSource, model.facetAgency, model.facetState, model.facetTags, model.facetPopDensity, model.facetRegion, model.facetStatus, model.currentPage, model.sortType);
            return View(model);
        }

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

            //CHECK PERMISSIONS
            if (User.IsInRole("Admins") || db_Accounts.UserCanEditOrgIDX(UserIDX, model.agency.ORG_IDX.ConvertOrDefault<Guid>()))
            {
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


                //award agency profile badge
                if (db_Accounts.UserCanEditOrgIDX(UserIDX, model.agency.ORG_IDX.ConvertOrDefault<Guid>()))
                    db_Forum.EarnBadgeController(UserIDX, "AgencyProfile");


                //now update the Azure search
                AzureSearch.PopulateSearchIndexOrganization(SuccID);


                if (SuccID != null)
                    TempData["Success"] = "Update successful.";
                else
                    TempData["Error"] = "Error updating data.";
            }


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
                    z.COMMENTS, z.PROJECT_CONTACT, z.ACTIVE_INTEREST_IND, false, db_Accounts.GetUserIDX(), true);
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

            //if still no agency (and not an Admin, then return error)
            if (!User.IsInRole("Admins") && (selAgency == null || selAgency == Guid.Empty || !db_Accounts.UserCanEditOrgIDX(UserIDX, selAgency.ConvertOrDefault<Guid>())))
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

            if (User.IsInRole("Admins"))
                model.ddl_Agencies = ddlHelpers.get_ddl_organizations(true, true);

            return View(model);
        }

        // GET: /Dashboard/ProjectReview
        public ActionResult ProjectReview()
        {
            int UserIDX = db_Accounts.GetUserIDX();
            var model = new vmDashboardProjectReview();
            model.ProjectsNeedingReview = db_EECIP.GetT_OE_PROJECTS_NeedingReview(UserIDX);  //projects needing review
            return View(model);
        }



        // GET: /Dashboard/ProjectDetails/1
        /// <param name="id">Only supply for existing case</param>
        /// <param name="orgIDX">Only supply for new case</param>
        public ActionResult ProjectDetails(Guid? id, Guid? orgIDX, string returnURL)
        {
            int UserIDX = db_Accounts.GetUserIDX();
            var model = new vmDashboardProjectDetails();

            //new case
            if (id == null)
            {
                //if new case and either no org supplied or the org supplied the user doesn't have rights to edit, then fail
                if ((orgIDX == null) || (User.IsInRole("Admins") == false && db_Accounts.UserCanEditOrgIDX(UserIDX, orgIDX.ConvertOrDefault<Guid>()) == false ))
                {
                    TempData["Error"] = "You cannot edit projects for this agency.";
                    return RedirectToAction("AccessDenied", "Home");
                }

                //if got this far then initialize valid new project
                model.project = new T_OE_PROJECTS
                {
                    ORG_IDX = orgIDX,
                    PROJECT_IDX = Guid.NewGuid(),
                };
                model.NewProjInd = true;
            }
            else
            {
                model.project = db_EECIP.GetT_OE_PROJECTS_ByIDX(id);
                if (model.project == null)
                {
                    TempData["Error"] = "Project not found.";
                    return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    //should be existing case
                    if (User.IsInRole("Admins") == false && db_Accounts.UserCanEditOrgIDX(UserIDX, model.project.ORG_IDX.ConvertOrDefault<Guid>()) == false)
                    {
                        TempData["Error"] = "You cannot edit projects for this agency.";
                        return RedirectToAction("AccessDenied", "Home");
                    }

                    //existing case, no failure
                    model.SelectedProgramAreas = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeSelected(model.project.PROJECT_IDX, "Program Area");
                    model.SelectedFeatures = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeSelected(model.project.PROJECT_IDX, "Project Feature");
                    model.files_existing = db_EECIP.GetT_OE_DOCUMENTS_ByProjectID(model.project.PROJECT_IDX);
                }
            }


            //if got this far (regardless of new or existing) then add more to the model
            T_OE_ORGANIZATION _org = db_Ref.GetT_OE_ORGANIZATION_ByID(model.project.ORG_IDX.ConvertOrDefault<Guid>());
            if (_org != null)
            {
                model.orgName = _org.ORG_NAME;
                model.orgType = _org.ORG_TYPE;
            }
            model.sProjectUrlList = db_EECIP.GetT_OE_PROJECTS_URL_ByProjIDX(model.project.PROJECT_IDX);
            model.ddl_AgencyUsers = ddlHelpers.get_ddl_users_by_organization(model.project.ORG_IDX.ConvertOrDefault<Guid>());
            model.AllProgramAreas = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeAll(model.project.PROJECT_IDX, "Program Area").Select(x => new SelectListItem { Value = x, Text = x });
            model.AllFeatures = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeAll(model.project.PROJECT_IDX, "Project Feature").Select(x => new SelectListItem { Value = x, Text = x });
            model.ReturnURL = returnURL ?? "Projects";
            return View(model);

        }

        // POST: /Dashboard/ProjectEdit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ProjectEdit(vmDashboardProjectDetails model)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            //CHECK PERMISSIONS
            if (User.IsInRole("Admins") || db_Accounts.UserCanEditOrgIDX(UserIDX, model.project.ORG_IDX.ConvertOrDefault<Guid>()))
            {
                //update project data
                Guid? newProjID = db_EECIP.InsertUpdatetT_OE_PROJECTS(model.project.PROJECT_IDX, model.project.ORG_IDX, model.project.PROJ_NAME ?? "",
                    model.project.PROJ_DESC ?? "", model.project.MEDIA_TAG, model.project.START_YEAR ?? -1, model.project.PROJ_STATUS ?? "",
                    model.project.DATE_LAST_UPDATE ?? -1, model.project.RECORD_SOURCE ?? "", model.project.PROJECT_URL ?? "", model.project.MOBILE_IND,
                    model.project.MOBILE_DESC ?? "", model.project.ADV_MON_IND, model.project.ADV_MON_DESC ?? "", model.project.BP_MODERN_IND,
                    model.project.BP_MODERN_DESC ?? "", model.project.COTS ?? "", model.project.VENDOR ?? "", model.project.PROJECT_CONTACT ?? "", model.project.PROJECT_CONTACT_IDX ?? -1, true, false, UserIDX, null, true);

                if (newProjID != null)
                {
                    Guid newProjID2 = newProjID.ConvertOrDefault<Guid>();
                    //update program area tags
                    db_EECIP.DeleteT_OE_PROJECT_TAGS(newProjID2, "Program Area");
                    foreach (string expertise in model.SelectedProgramAreas ?? new List<string>())
                        db_EECIP.InsertT_OE_PROJECT_TAGS(newProjID2, "Program Area", expertise);


                    //update project url
                    db_EECIP.DeleteT_OE_PROJECTS_URL(newProjID2);
                    foreach (T_OE_PROJECT_URLS urls in model.sProjectUrlList?? new List<T_OE_PROJECT_URLS>())
                        db_EECIP.InsertT_OE_PROJECTS_URL(newProjID2,urls.PROJECT_URL,urls.PROJ_URL_DESC);


                    //update feature tags
                    db_EECIP.DeleteT_OE_PROJECT_TAGS(newProjID2, "Project Feature");
                    foreach (string feature in model.SelectedFeatures ?? new List<string>())
                        db_EECIP.InsertT_OE_PROJECT_TAGS(newProjID2, "Project Feature", feature);

                    //update files
                    if (model.files != null)
                    {
                        foreach (HttpPostedFileBase file in model.files)
                        {
                            byte[] fileBytes = null;

                            if (file != null)
                            {
                                using (Stream inputStream = file.InputStream)
                                {
                                    MemoryStream memoryStream = inputStream as MemoryStream;
                                    if (memoryStream == null)
                                    {
                                        memoryStream = new MemoryStream();
                                        inputStream.CopyTo(memoryStream);
                                    }
                                    fileBytes = memoryStream.ToArray();

                                    //insert to database
                                    db_EECIP.InsertUpdateT_OE_DOCUMENTS(null, fileBytes, file.FileName, "project", file.ContentType, fileBytes.Length, model.FileDescription, null, newProjID2, null, UserIDX);
                                }


                                //award profile badge
                                if (db_Accounts.UserCanEditOrgIDX(UserIDX, model.project.ORG_IDX.ConvertOrDefault<Guid>()))
                                    db_Forum.EarnBadgeController(UserIDX, "ProjectDocument");

                            }
                        }



                    }

                    //award badges for new project
                    if (newProjID2 != model.project.PROJECT_IDX)//new case
                        db_Forum.EarnBadgeAddProject(UserIDX);

                    //now update the Azure search
                    AzureSearch.PopulateSearchIndexProject(newProjID2);

                    TempData["Success"] = "Update successful.";
                    return RedirectToAction("ProjectDetails", "Dashboard", new { id = newProjID2, returnURL = model.ReturnURL });
                }
                else
                    TempData["Error"] = "Error updating data.";
            }
            else
                TempData["Error"] = "You don't have permissions to edit this project.";

            return RedirectToAction(model.ReturnURL ?? "Projects", new { selAgency = model.project.ORG_IDX } );
        }

        // POST: /Dashboard/ProjectsDelete
        //[HttpPost]
        //public JsonResult ProjectsDelete(Guid id, string Type)
        //{
        //    int UserIDX = db_Accounts.GetUserIDX();

        //    //CHECK PERMISSIONS
        //    T_OE_PROJECTS p = db_EECIP.GetT_OE_PROJECTS_ByIDX(id);
        //    if (p != null)
        //    {
        //        if (User.IsInRole("Admins") || db_Accounts.UserCanEditOrgIDX(db_Accounts.GetUserIDX(), p.ORG_IDX.ConvertOrDefault<Guid>()))
        //        {
        //            int SuccID = db_EECIP.DeleteT_OE_PROJECTS(id);
        //            if (SuccID > 0)
        //            {
        //                //SUCCESS - now delete from Azure
        //                AzureSearch.DeleteSearchIndexProject(id);
        //                return Json("Success");
        //            }
        //        }
        //    }

        //    //if got this far, general error
        //    return Json("Unable to delete project.");
        //}

        // POST: /Dashboard/ProjectsDelete
        [HttpPost]
        public JsonResult ProjectsDelete(IEnumerable<Guid> RecordDeletebyId)
        {
            int UserIDX = db_Accounts.GetUserIDX();
            if (RecordDeletebyId == null)
            {
                return Json("No record selected to delete");
            }
            else
            {
                foreach (var id in RecordDeletebyId)
                {
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
                }

                //if got this far, general error
                return Json("Unable to delete project.");
            }
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
                    model.sProjectUrlList = db_EECIP.GetT_OE_PROJECTS_URL_ByProjIDX(model.project.PROJECT_IDX);
                    T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(model.project.MODIFY_USERIDX ?? model.project.CREATE_USERIDX ?? -1);
                    if (u != null)
                        model.LastUpdatedUser = u.FNAME + " " + u.LNAME;
                    model.ProjectVotePoints = db_EECIP.GetT_OE_PROJECT_VOTES_TotalByProject(model.project.PROJECT_IDX);
                    model.HasVoted = db_EECIP.GetT_OE_PROJECT_VOTES_HasVoted(model.project.PROJECT_IDX, UserIDX);
                    model.UserBelongsToProjectAgency = db_Accounts.UserCanEditOrgIDX(db_Accounts.GetUserIDX(), model.project.ORG_IDX.ConvertOrDefault<Guid>());
                    //project contact
                    if (model.project.PROJECT_CONTACT_IDX != null)
                    {
                        model.ProjectContact = db_Accounts.GetT_OE_USERSByIDX(model.project.PROJECT_CONTACT_IDX??-1);
                    }
                    model.files_existing = db_EECIP.GetT_OE_DOCUMENTS_ByProjectID(model.project.PROJECT_IDX);

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
                //now award badge
                db_Forum.EarnBadgeUpVoteProject(UserIDX);

                string votes = db_EECIP.GetT_OE_PROJECT_VOTES_TotalByProject(id.ConvertOrDefault<Guid>()).ToString();
                return Json(new { msg = "Success", val = votes });
            }
            else
                return Json(new { msg = "Unable to record vote." });

        }


        [HttpPost]
        public ActionResult ProjectFileUpload(vmForumAttachFilesToPost attachFileToPostViewModel)
        {
            try
            {
                int UserIDX = db_Accounts.GetUserIDX();

                // First this to do is get the post
                var project = db_EECIP.GetT_OE_PROJECTS_ByIDX(attachFileToPostViewModel.UploadPostId);
                if (project != null)
                {
                    // Check we get a valid post back and have some file
                    if (attachFileToPostViewModel.Files != null && attachFileToPostViewModel.Files.Any())
                    {
                        UploadProjectFiles(attachFileToPostViewModel.Files, UserIDX, project.PROJECT_IDX);
                        TempData["Success"] = "Upload success";
                    }
                    else
                        TempData["Error"] = "Please select a file";

                    return RedirectToAction("ProjectDetails", "Dashboard", new { id = project.PROJECT_IDX });
                }
                else
                    TempData["Error"] = "Invalid post";

                return RedirectToAction("ShowTopic", "Forum", new { id = project.PROJECT_IDX });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Invalid post";
                return RedirectToAction("Index", "Forum");
            }

        }


        private static void UploadProjectFiles(HttpPostedFileBase[] files, int UserIDX, Guid keyID)
        {
            // Loop through each file and get the file info and save to the users folder and Db
            foreach (var file in files)
            {
                byte[] fileBytes = null;

                if (file != null)
                {
                    using (Stream inputStream = file.InputStream)
                    {
                        MemoryStream memoryStream = inputStream as MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        fileBytes = memoryStream.ToArray();

                        //insert to database
                        db_EECIP.InsertUpdateT_OE_DOCUMENTS(null, fileBytes, file.FileName, "project", file.ContentType, fileBytes.Length, null, null, keyID, null, UserIDX);
                    }
                }
            }
        }



        public ActionResult ProjectFileDownload(Guid? id)
        {
            try
            {
                T_OE_DOCUMENTS doc = db_EECIP.GetT_OE_DOCUMENTS_ByID(id.ConvertOrDefault<Guid>());
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = doc.DOC_NAME,
                    Inline = false
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());
                if (doc.DOC_CONTENT != null)
                    return File(doc.DOC_CONTENT, doc.DOC_FILE_TYPE ?? "application/octet-stream");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            TempData["Error"] = "Unable to download document.";
            return RedirectToAction("Index", "Forum");
        }


        public ActionResult ProjectFileDelete(Guid? id)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            //get project, then org to check permissions
            T_OE_DOCUMENTS doc = db_EECIP.GetT_OE_DOCUMENTS_ByID(id.ConvertOrDefault<Guid>());
            if (doc != null)
            {

                T_OE_PROJECTS _project = db_EECIP.GetT_OE_PROJECTS_ByIDX(doc.PROJECT_IDX);
                if (_project != null)
                {
                    //actual permissions check
                    if (db_Accounts.UserCanEditOrgIDX(UserIDX, _project.ORG_IDX.ConvertOrDefault<Guid>()) || User.IsInRole("Admins"))
                    {
                        int SuccID = db_EECIP.DeleteT_OE_DOCUMENTS(id.ConvertOrDefault<Guid>());
                        if (SuccID > 0)
                        {
                            TempData["Success"] = "File removed.";
                            return RedirectToAction("ProjectDetails", "Dashboard", new { id = _project.PROJECT_IDX });
                        }
                    }
                    TempData["Error"] = "Unable to delete document.";
                    return RedirectToAction("ProjectDetails", "Dashboard", new { id = _project.PROJECT_IDX });
                }
            }

            TempData["Error"] = "Unable to delete document.";
            return RedirectToAction("ProjectDetails", "Dashboard");
        }


        #endregion

        public ActionResult EnterpriseSvcOverview() {
            var model = new vmDashboardEntSvcOverview();
            model.EntSvcOverviewDisplay = db_EECIP.GetT_OE_ORGANIZATION_ENT_SVCS_Overview();
            return View(model);
        }


        public ActionResult EnterpriseSvc(int id)
        {
            var model = new vmDashboardEntSvcOverview();
            model.EntSvcOverviewDisplay = db_EECIP.GetT_OE_ORGANIZATION_ENT_SVCS_Overview();
            return View(model);
        }



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


        public ActionResult Governance(Guid? selAgency)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            var model = new vmDashboardGovernance();

            T_OE_USERS user = db_Accounts.GetT_OE_USERSByIDX(UserIDX);
            if (user != null)
            {
                model.UnlockedInd = user.ALLOW_GOVERNANCE;

                //if user has unlocked governance, display more
                if (model.UnlockedInd == true)
                {
                    if (selAgency != null)
                    {
                        model.selAgency = selAgency;
                        model.projects = db_EECIP.GetT_OE_PROJECTS_ByOrgIDX(selAgency.ConvertOrDefault<Guid>());

                        T_OE_ORGANIZATION o = db_Ref.GetT_OE_ORGANIZATION_ByID(selAgency.ConvertOrDefault<Guid>());
                        if (o != null)
                            model.selAgencyName = o.ORG_NAME;
                    }
                }
            }

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GovernanceOrgAdd(vmDashboardGovernance model)
        {
            int UserIDX = db_Accounts.GetUserIDX();
            Guid? SuccID = db_Ref.InsertUpdatetT_OE_ORGANIZATION(null, model.edit_org_abbr, model.edit_org_name, null, null, "Governance", null, null, true, UserIDX);

            if (SuccID != null)
                TempData["Success"] = "Governance Group Added";
            else
                TempData["Error"] = "Unable to add group";

            return RedirectToAction("Governance");
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GovernanceUnlock()
        {
            int UserIDX = db_Accounts.GetUserIDX();
            db_Accounts.UpdateT_OE_USERS_UnlockGovernance(UserIDX);
            return RedirectToAction("Governance");
        }

        
    }
}