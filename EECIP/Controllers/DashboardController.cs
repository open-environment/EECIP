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
        public ActionResult Index(string selSub)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            var model = new vmDashboardIndex
            {
                UserBadges = db_Forum.GetBadgesForUser(UserIDX),  //badge progress
                ProjectsNeedingReviewCount = db_EECIP.GetT_OE_PROJECTS_NeedingReviewCount(UserIDX),  //projects needing review
                UserPointLeaders = db_Forum.GetMembershipUserPoints_MostPoints(6),  //user point leaders
                UserPointLeadersMonth = db_Forum.GetMembershipUserPoints_MostPoints(6, System.DateTime.Today.AddDays(-30), System.DateTime.Now.AddDays(1)),
                LatestProjects = db_EECIP.GetT_OE_PROJECTS_RecentlyUpdatedMatchingInterest(UserIDX, 900, true, 6, (selSub == "Default View" ? null : selSub)),  //latest projects matching interest
                LatestTopics = db_Forum.GetLatestTopicPostsMatchingInterestNew(UserIDX, 900, 6, (selSub == "Default View" ? null : selSub)), //latest topics matching interest
                ProjectCount = db_EECIP.GetT_OE_PROJECTS_CountNonGovernance(),
                GovernanceCount = db_EECIP.GetT_OE_PROJECTS_CountGovernance(),
                DiscussionCount = db_Forum.GetTopicCount(null),
                AgencyCount = db_Ref.GetT_OE_ORGANIZATION_Count(),
                UserBadgeEarnedCount = db_Forum.GetBadgesForUserCount(UserIDX),
                Announcement = db_Ref.GetT_OE_APP_SETTING_CUSTOM().ANNOUNCEMENTS,
                ddl_Subscriptions = db_EECIP.GetT_OE_USER_EXPERTISE_ByUserIDX_withDefault(UserIDX).Select(x => new SelectListItem { Value = x, Text = x }),
                selSub = (selSub ?? "Default View")
            };

            //fallback on topics
            model.TopicMatchInd = (model.LatestTopics != null && model.LatestTopics.Count > 0);
            if (model.TopicMatchInd == false)
                model.LatestTopics = db_Forum.GetLatestTopicPostsFallback(900, 6);

            T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(UserIDX);
            if (u != null)
                model.UserName = u.FNAME + " " + u.LNAME;

            return View(model);
        }

        // GET: Dashboard/Search
        public ActionResult Search(string q, string facetDataType, string facetMedia, string facetRecordSource, string facetAgency, string facetState, string facetOrgType, string facetTags, string facetPopDensity, string facetRegion, string facetStatus, string activeTab, string currentPage, string sortType, string sortDir)
        {
            var model = new vmDashboardSearch
            {
                q = q,
                facetDataType = facetDataType,
                facetMedia = facetMedia,
                facetRecordSource = facetRecordSource,
                facetAgency = facetAgency,
                facetState = facetState,
                facetOrgType = facetOrgType,
                facetTags = facetTags,
                facetPopDensity = facetPopDensity,
                facetRegion = facetRegion,
                facetStatus = facetStatus,
                activeTab = activeTab ?? "1",
                currentPage = currentPage.ConvertOrDefault<int?>() ?? 1,
                sortType = sortType,
                sortDir = sortDir,
                searchResults = AzureSearch.QuerySearchIndex(q, facetDataType, facetMedia, facetRecordSource, facetAgency, facetState, facetOrgType, facetTags, facetPopDensity, facetRegion, facetStatus, currentPage.ConvertOrDefault<int?>() ?? 1, sortType, sortDir)
            };

            //log search
            if (!string.IsNullOrEmpty(q))
                db_Ref.InsertT_OE_SYS_SEARCH_LOG(q.ToUpper().Trim());


            return View(model);
        }

        public ActionResult Leaderboard(DateTime? startDt, DateTime? endDt)
        {
            var model = new vmDashboardLeaderboard
            {
                UserPointLeaders = db_Forum.GetMembershipUserPoints_MostPoints(100, startDt, endDt)
            };

            return View(model);
        }

        public ActionResult LeaderboardDtl(int? id)
        {
            var model = new vmDashboardLeaderboardDtl
            {
                UserPointDetails = db_Forum.GetMembershipUserPoints_ByUserID(id ?? 0)
            };

            T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(id ??0);
            if (u != null)
                model.UserName = u.FNAME + " " + u.LNAME;


            return View(model);
        }

        #endregion


        #region AGENCY

        // GET: /Dashboard/Agency
        public ActionResult Agency(Guid? selAgency)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            // if no agency passed, get agency for which the logged-in user is associated
            if (selAgency == null || selAgency == Guid.Empty)
            {
                T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(UserIDX);
                if (u != null && u.ORG_IDX != null)
                    selAgency = u.ORG_IDX.ConvertOrDefault<Guid>();
            }

            //if still no agency (and not an Admin, then return error), or if user cannot edit agency
            if (!User.IsInRole("Admins") &&  (selAgency == null || selAgency == Guid.Empty || !db_Accounts.UserCanEditOrgIDX(UserIDX, selAgency.ConvertOrDefault<Guid>())))
            {
                TempData["Error"] = "You are not associated with an agency.";
                return RedirectToAction("AccessDenied", "Home");
            }


            Guid agency = selAgency.ConvertOrDefault<Guid>();

            var model = new vmDashboardAgency
            {
                agency = db_Ref.GetT_OE_ORGANIZATION_ByID(agency),
                //users = db_Accounts.GetT_OE_USERSByAgency(agency),
                //database
                SelectedDatabase = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByOrgAttribute(agency, "Database"),
                AllDatabase = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByAttributeAll(agency, "Database").Select(x => new SelectListItem { Value = x, Text = x }),
                //app framework
                SelectedAppFramework = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByOrgAttribute(agency, "App Framework"),
                AllAppFramework = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByAttributeAll(agency, "App Framework").Select(x => new SelectListItem { Value = x, Text = x }),
                //ent services
                org_ent_services = db_EECIP.GetT_OE_ORGANIZATION_ENTERPRISE_PLATFORM(agency)
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult AgencyUserListData()
        {
            var id = Request.Form.GetValues("agencyid")?.FirstOrDefault();
            if (id != null)
            {
                Guid idg = new Guid(id);
                var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
                var data = db_Accounts.GetT_OE_USERSByAgencyLight(idg);
                return Json(new { draw, recordsFiltered = data.Count(), recordsTotal = data.Count(), data = data });
            }
            else
                return null;
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
                int SuccID = db_EECIP.InsertUpdatetT_OE_ORGANIZATION_ENT_SVCS(z.ORG_ENT_SVCS_IDX, model.agency.ORG_IDX, z.ENT_PLATFORM_IDX, z.PROJECT_NAME, z.VENDOR, z.IMPLEMENT_STATUS,
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
            if (Guid.TryParse(strid, out Guid id))
            {
                var model = new vmDashboardAgencyCard
                {
                    agency = db_Ref.GetT_OE_ORGANIZATION_ByID(id),
                    org_ent_services = db_EECIP.GetT_OE_ORGANIZATION_ENT_SVCS_NoLeftJoin(id),
                    users = db_Accounts.GetT_OE_USERSByAgency(id),
                    SelectedDatabase = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByOrgAttribute(id, "Database"),
                    SelectedAppFramework = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByOrgAttribute(id, "App Framework"),
                    projects = db_EECIP.GetT_OE_PROJECTS_ByOrgIDX(id)
                };

                if (model.agency != null)
                    return View(model);
            }

            return RedirectToAction("Index", "Dashboard");
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AgencyUserFlagRemoval(vmDashboardAgency model)
        {
            if (model.FlagUserIDX != null && model.agency.ORG_IDX != null)
            {

                //get flagging user
                int UserIDX = db_Accounts.GetUserIDX();
                T_OE_USERS flaggingUser = db_Accounts.GetT_OE_USERSByIDX(UserIDX);

                //get flagged user
                T_OE_USERS flaggedUser = db_Accounts.GetT_OE_USERSByIDX(model.FlagUserIDX.GetValueOrDefault());


                //notify Site Admins via email
                List<T_OE_USERS> Admins = db_Accounts.GetT_OE_USERSInRole(1);
                foreach (T_OE_USERS Admin in Admins)
                    Utils.SendEmail(null, Admin.EMAIL, null, null, "EECIP: " + flaggedUser.FNAME + ' ' + flaggedUser.LNAME + " flagged for removal", "The user " + flaggedUser.FNAME + ' ' + flaggedUser.LNAME + " (" + flaggedUser.EMAIL + ") has been flagged for removal by the EECIP user " + flaggedUser.FNAME + ' ' + flaggedUser.LNAME + ". Please log into EECIP and consider removing or inactivating the user account", null, null, null);

                TempData["Success"] = "Your request has been submitted.";
            }
            else
                TempData["Error"] = "Unable to make request at this time.";


            return RedirectToAction("Agency", "Dashboard");
        }

        #endregion


        #region PROJECT

        // GET: /Dashboard/Projects
        public ActionResult Projects(Guid? selAgency)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            // if no agency passed, get agency for which the logged-in user is associated
            if (selAgency == null || selAgency == Guid.Empty)
            {
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
            var model = new vmDashboardProjectReview
            {
                ProjectsNeedingReview = db_EECIP.GetT_OE_PROJECTS_NeedingReview(UserIDX)  //projects needing review
            };
            return View(model);
        }


        public ActionResult ProjectReview2(Guid id)
        {
            int UserIDX = db_Accounts.GetUserIDX();
            List<T_OE_ORGANIZATION> _ProjectOrgs = db_EECIP.GetT_OE_PROJECT_ORGS_ByProject(id);


            //CHECK PERMISSIONS
            if (User.IsInRole("Admins") || db_Accounts.UserCanEditOrgList(UserIDX, _ProjectOrgs))
                db_EECIP.InsertUpdatetT_OE_PROJECTS(id, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 
                    null, null, null, true, null, UserIDX, null, true);

                return RedirectToAction("ProjectReview", "Dashboard");
        }
        
        // GET: /Dashboard/ProjectDetails/1
        /// <param name="id">Only supply for existing case</param>
        /// <param name="orgIDX">Only supply for new case</param>
        public ActionResult ProjectDetails(Guid? id, Guid? orgIDX, string returnURL)
        {
            int UserIDX = db_Accounts.GetUserIDX();
            var model = new vmDashboardProjectDetails();

            //new case********************************************
            if (id == null)
            {
                //if new case and either no org supplied or the org supplied the user doesn't have rights to edit, then fail
                if ((orgIDX == null) || (User.IsInRole("Admins") == false && db_Accounts.UserCanEditOrgIDX(UserIDX, orgIDX.ConvertOrDefault<Guid>()) == false ))
                {
                    TempData["Error"] = "You cannot edit projects for this agency or no agency supplied.";
                    return RedirectToAction("AccessDenied", "Home");
                }

                //if got this far then initialize valid new project
                model.project = new T_OE_PROJECTS
                {
                    PROJECT_IDX = Guid.NewGuid(),
                };

                //populate org for new project
                T_OE_ORGANIZATION o = db_Ref.GetT_OE_ORGANIZATION_ByID(orgIDX.ConvertOrDefault<Guid>()); 
                model.ProjectOrgs = new List<T_OE_ORGANIZATION>();
                model.ProjectOrgs.Add(o);

                model.NewProjInd = true;
                model.NewProjOrgIDX = orgIDX;
            }
            else  //edit case********************************************
            {
                model.project = db_EECIP.GetT_OE_PROJECTS_ByIDX(id);
                if (model.project == null)
                {
                    TempData["Error"] = "Project not found.";
                    return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    //rich text display
                    model.ProjectRichDesc = model.project.PROJ_DESC_HTML;

                    //project orgs
                    model.ProjectOrgs = db_EECIP.GetT_OE_PROJECT_ORGS_ByProject(id.ConvertOrDefault<Guid>());

                    //should be existing case
                    if (User.IsInRole("Admins") == false && db_Accounts.UserCanEditOrgList(UserIDX, model.ProjectOrgs) == false)
                    {
                        TempData["Error"] = "You cannot edit projects for this agency.";
                        return RedirectToAction("AccessDenied", "Home");
                    }

                    //existing case, no failure
                    model.SelectedProgramAreas = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeSelected(model.project.PROJECT_IDX, "Program Area");
                    model.SelectedFeatures = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeSelected(model.project.PROJECT_IDX, "Tags");
                    model.files_existing = db_EECIP.GetT_OE_DOCUMENTS_ByProjectID(model.project.PROJECT_IDX);
                }
            }


            //if got this far (regardless of new or existing) then add more to the model
            T_OE_ORGANIZATION _org = db_Ref.GetT_OE_ORGANIZATION_ByID(model.ProjectOrgs[0].ORG_IDX.ConvertOrDefault<Guid>());
            if (_org != null && _org.ORG_TYPE == "Governance")
                model.governanceInd = true;

            model.sProjectUrlList = db_EECIP.GetT_OE_PROJECTS_URL_ByProjIDX(model.project.PROJECT_IDX);
            model.ddl_AgencyUsers = ddlHelpers.get_ddl_users_by_organizationList(model.ProjectOrgs);
            model.AllProgramAreas = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeAll(model.project.PROJECT_IDX, "Program Area").Select(x => new SelectListItem { Value = x, Text = x });
            model.AllFeatures = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeAll(model.project.PROJECT_IDX, "Tags").Select(x => new SelectListItem { Value = x, Text = x });
            //model.ddl_Agencies = ddlHelpers.get_ddl_organizations(true, false);
            model.ReturnURL = returnURL ?? "Projects";
            return View(model);

        }


        // POST: /Dashboard/ProjectEdit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ProjectEdit(vmDashboardProjectDetails model)
        {
            int UserIDX = db_Accounts.GetUserIDX();


            model.ProjectOrgs = db_EECIP.GetT_OE_PROJECT_ORGS_ByProject(model.project.PROJECT_IDX);

            //for insert case, project orgs will not have org yet
            if (model.ProjectOrgs.Count == 0 && model.NewProjOrgIDX != null)
            {
                model.ProjectOrgs.Add(db_Ref.GetT_OE_ORGANIZATION_ByID(model.NewProjOrgIDX.ConvertOrDefault<Guid>()));
            }
        
            //CHECK PERMISSIONS
            if (User.IsInRole("Admins") || db_Accounts.UserCanEditOrgList(UserIDX, model.ProjectOrgs))
            {
                //plain text version of project description
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(model.ProjectRichDesc);
                string projDescPlain = "";
                foreach (HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//text()"))
                    projDescPlain += node.InnerText;

                //determine record source 
                if (db_Accounts.UserCanEditOrgList(UserIDX, model.ProjectOrgs))
                    model.project.RECORD_SOURCE = "Agency supplied";

                //update project data
                Guid? newProjID = db_EECIP.InsertUpdatetT_OE_PROJECTS(model.project.PROJECT_IDX, null, model.project.PROJ_NAME ?? "",
                    projDescPlain, model.ProjectRichDesc ?? "", model.project.MEDIA_TAG, model.project.START_YEAR ?? -1, model.project.PROJ_STATUS ?? "",
                    model.project.DATE_LAST_UPDATE ?? -1, model.project.RECORD_SOURCE ?? "", model.project.PROJECT_URL ?? "", model.project.MOBILE_IND,
                    model.project.MOBILE_DESC ?? "", model.project.ADV_MON_IND, model.project.ADV_MON_DESC ?? "", model.project.BP_MODERN_IND,
                    model.project.BP_MODERN_DESC ?? "", model.project.COTS ?? "", model.project.VENDOR ?? "", model.project.PROJECT_CONTACT ?? "", model.project.PROJECT_CONTACT_IDX ?? -1, true, false, UserIDX, null, true);

                if (newProjID != null)
                {
                    Guid newProjID2 = newProjID.ConvertOrDefault<Guid>();

                    //update project org
                    if (model.NewProjOrgIDX != null)
                        db_EECIP.InsertUpdateT_OE_PROJECT_ORGS(newProjID2, model.NewProjOrgIDX.ConvertOrDefault<Guid>(), UserIDX);


                    //update program area tags
                    db_EECIP.DeleteT_OE_PROJECT_TAGS(newProjID2, "Program Area");
                    foreach (string expertise in model.SelectedProgramAreas ?? new List<string>())
                        db_EECIP.InsertT_OE_PROJECT_TAGS(newProjID2, "Program Area", expertise);


                    //update project url
                    db_EECIP.DeleteT_OE_PROJECTS_URL(newProjID2);
                    foreach (T_OE_PROJECT_URLS urls in model.sProjectUrlList?? new List<T_OE_PROJECT_URLS>())
                        db_EECIP.InsertT_OE_PROJECTS_URL(newProjID2,urls.PROJECT_URL,urls.PROJ_URL_DESC);


                    //update feature tags
                    db_EECIP.DeleteT_OE_PROJECT_TAGS(newProjID2, "Tags");
                    foreach (string feature in model.SelectedFeatures ?? new List<string>())
                        db_EECIP.InsertT_OE_PROJECT_TAGS(newProjID2, "Tags", feature);

                    foreach (T_OE_DOCUMENTS docs in model.files_existing ?? new List<T_OE_DOCUMENTS>())
                    {                        
                        db_EECIP.InsertUpdateT_OE_DOCUMENTS(docs.DOC_IDX, null, null, "project", null, null, docs.DOC_COMMENT, null, newProjID2, null, UserIDX);
                    }

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

            return RedirectToAction(model.ReturnURL ?? "Projects", new { selAgency = model.NewProjOrgIDX } );
        }


        // POST: /Dashboard/ProjectsDelete
        [HttpPost]
        public JsonResult ProjectsDelete(IEnumerable<Guid> id)
        {
            int UserIDX = db_Accounts.GetUserIDX();
            if (id == null || id.Count() == 0)
                return Json("No record selected to delete");
            else
            {
                foreach (var id1 in id)
                {
                    //CHECK PERMISSIONS
                    T_OE_PROJECTS p = db_EECIP.GetT_OE_PROJECTS_ByIDX(id1);
                    if (p != null)
                    {
                        List<T_OE_ORGANIZATION> orgs = db_EECIP.GetT_OE_PROJECT_ORGS_ByProject(p.PROJECT_IDX);
                        if (orgs != null)
                        {
                            if (User.IsInRole("Admins") || db_Accounts.UserCanEditOrgList(db_Accounts.GetUserIDX(), orgs))
                            {
                                int SuccID = db_EECIP.DeleteT_OE_PROJECTS(id1);
                                if (SuccID > 0)
                                {
                                    //SUCCESS - now delete from Azure (need to concat proj and all proj orgs
                                    foreach (T_OE_ORGANIZATION _org in orgs)
                                        AzureSearch.DeleteSearchIndexKey(id1.ToString() + "_" + _org.ORG_IDX.ToString());
                                }
                                else
                                    return Json("Unable to delete project.");
                            }
                        }
                    }
                }

                //if got this far, success
                return Json("Success");
            }
        }


        // GET: /Dashboard/ProjectCard/1
        public ActionResult ProjectCard(string strid)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            if (Guid.TryParse(strid, out Guid id))
            {
                var model = new vmDashboardProjectCard
                {
                    project = db_EECIP.GetT_OE_PROJECTS_ByIDX(id)
                };

                if (model.project != null)
                {
                    model.ProjectOrgs = db_EECIP.GetT_OE_PROJECT_ORGS_ByProject(id.ConvertOrDefault<Guid>());
                    model.SelectedProgramAreas = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeSelected(model.project.PROJECT_IDX, "Program Area");
                    model.SelectedFeatures = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeSelected(model.project.PROJECT_IDX, "Tags");
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
                        model.ProjectContact = db_Accounts.GetT_OE_USERSByIDX(model.project.PROJECT_CONTACT_IDX ?? -1);
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
            catch
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
                    List<T_OE_ORGANIZATION> _projOrgs = db_EECIP.GetT_OE_PROJECT_ORGS_ByProject(_project.PROJECT_IDX);

                    //actual permissions check
                    if (db_Accounts.UserCanEditOrgList(UserIDX, _projOrgs) || User.IsInRole("Admins"))
                    {
                        int SuccID = db_EECIP.DeleteT_OE_DOCUMENTS(id.ConvertOrDefault<Guid>());
                        if (SuccID > 0)
                        {
                            TempData["Success"] = "File removed.";
                            return RedirectToAction("ProjectDetails", "Dashboard", new { id = _project.PROJECT_IDX });
                        }
                    }
                    else
                    {
                        TempData["Error"] = "No permission to delete document.";
                        return RedirectToAction("ProjectDetails", "Dashboard", new { id = _project.PROJECT_IDX });
                    }
                }
            }

            TempData["Error"] = "Unable to delete document.";
            return RedirectToAction("ProjectDetails", "Dashboard");
        }


        public ActionResult AddProjectOrg(Guid? id)
        {
            var model = new vmDashboardAddProjectOrg();
            model.project = db_EECIP.GetT_OE_PROJECTS_ByIDX(id);

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddProjectOrg(vmDashboardAddProjectOrg model)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            if (model.selAgency != null)
            {
                List<T_OE_ORGANIZATION> projectOrgs = db_EECIP.GetT_OE_PROJECT_ORGS_ByProject(model.project.PROJECT_IDX);
                if (projectOrgs != null && projectOrgs.Count > 0)
                {
                    //CHECK PERMISSIONS
                    if (User.IsInRole("Admins") || db_Accounts.UserCanEditOrgList(UserIDX, projectOrgs))
                    {
                        db_EECIP.InsertUpdateT_OE_PROJECT_ORGS(model.project.PROJECT_IDX, model.selAgency.ConvertOrDefault<Guid>(), UserIDX);

                        //now update the Azure search
                        db_EECIP.InsertUpdatetT_OE_PROJECTS(model.project.PROJECT_IDX, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true, false);
                        AzureSearch.PopulateSearchIndexProject(model.project.PROJECT_IDX);

                        TempData["Success"] = "Update successful.";
                        return RedirectToAction("ProjectDetails", "Dashboard", new { id = model.project.PROJECT_IDX });
                    }
                    else
                        TempData["Error"] = "You do not have rights to edit this project";
                }
                else
                    TempData["Error"] = "No project found";
            }
            else
                TempData["Error"] = "Please select an agency";

            return RedirectToAction("AddProjectOrg", "Dashboard", new { id = model.project.PROJECT_IDX });
        }


        [HttpPost]
        public JsonResult ProjectOrgDelete(string id, string id2)
        {
            if (id == null)
                return Json("No record selected to delete");
            else
            {
                Guid orgIDX = Guid.Parse(id);
                Guid projIDX = Guid.Parse(id2);

                List<T_OE_ORGANIZATION> allPO = db_EECIP.GetT_OE_PROJECT_ORGS_ByProject(projIDX);
                if (allPO != null)
                {
                    //CHECK PERMISSIONS IF USER CAN EDIT PROJECT BASED ON ALL ORGS ASSOCIATED WITH PROJECT
                    int UserIDX = db_Accounts.GetUserIDX();
                    if (User.IsInRole("Admins") || db_Accounts.UserCanEditOrgList(UserIDX, allPO))
                    {
                        //FIND EXACT RECORD TO REMOVE
                        T_OE_PROJECT_ORGS po = db_EECIP.GetT_OE_PROJECT_ORGS_ByProj_Org(orgIDX, projIDX);
                        if (po != null)
                        {
                            int SuccID = db_EECIP.DeleteT_OE_PROJECT_ORGS(po.PROJECT_ORG_IDX);
                            if (SuccID > 0)
                            {
                                //SUCCESS - now delete from Azure
                                //now update the Azure search
                                db_EECIP.InsertUpdatetT_OE_PROJECTS(projIDX, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true, false);
                                AzureSearch.DeleteSearchIndexKey(id2 + "_" + id);
                                return Json("Success");
                            }
                        }
                    }
                    else
                        return Json("You don't have rights to remove .");
                }
            }

            //if got this far, generic error
            return Json("Unable to delete.");

        }
        #endregion


        public ActionResult EnterpriseSvcOverview() {
            var model = new vmDashboardEntSvcOverview
            {
                EntSvcOverviewDisplay = db_EECIP.GetT_OE_ORGANIZATION_ENT_SVCS_Overview()
            };
            return View(model);
        }


        public ActionResult EnterpriseSvcAgencies(int id)
        {
            var model = new vmDashboardEntSvcAgencies {
                ENT_PLATFORM_IDX = id,
                Organizations = db_EECIP.GetT_OE_ORGANIZATION_ENT_SVCS_ByEnt_Platform_ID(id)
            };

            return View(model);
        }

        public ActionResult EnterpriseSvcCard(string strid)
        {
            int UserIDX = strid.ConvertOrDefault<int>();
            int id = strid.ConvertOrDefault<int>();
            if (id > 100000)
                id = id - 100000;

            var model = new vmDashboardEntSvcCard
            {
                entsvc = db_EECIP.GetT_OE_ORGANIZATION_ENT_SVCS_ByID(id)
            };
            if (model.entsvc != null)
            {
                T_OE_ORGANIZATION _org = db_Ref.GetT_OE_ORGANIZATION_ByID(model.entsvc.ORG_IDX.ConvertOrDefault<Guid>());
                if (_org != null)
                    model.OrgName = _org.ORG_NAME;

                T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(model.entsvc.MODIFY_USERIDX ?? model.entsvc.CREATE_USERIDX ?? -1);
                if (u != null)
                    model.LastUpdatedUser = u.FNAME + " " + u.LNAME;
            }

            return View(model);
        }


        public ActionResult UserCard(string strid)
        {
            int UserIDX = strid.ConvertOrDefault<int>();

            var model = new vmDashboardUserCard
            {
                User = db_Accounts.GetT_OE_USERSByIDX(UserIDX),
                SelectedExpertise = db_EECIP.GetT_OE_USER_EXPERTISE_ByUserIDX(UserIDX),
                UserBadges = db_Forum.GetBadgesForUserNoLeftJoin(UserIDX)
            };
            model.UserOrg = db_Ref.GetT_OE_ORGANIZATION_ByID(model.User.ORG_IDX.ConvertOrDefault<Guid>());

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


        public ActionResult Metrics()
        {
            int UserIDX = db_Accounts.GetUserIDX();

            var model = new vmDashboardMetrics
            {
                PopSearchTerms = db_Ref.GetT_OE_SYS_SEARCH_LOG_Popular(30)
            };

            return View(model);
        }


        public JsonResult MetricChartUsers()
        {
            List<SP_NEW_CONTENT_USER_AGE_Result> recs = db_EECIP.GetSP_NEW_CONTENT_USER_AGE_Result();

            List<object> iData = new List<object>();
            foreach (SP_NEW_CONTENT_USER_AGE_Result rec in recs)
            {
                iData.Add(rec.CNT);
            }

            //Source data returned as JSON  
            return Json(iData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult MetricChartProjects()
        {
            List<SP_PROJECT_CREATE_COUNT_Result> recs = db_EECIP.GetSP_PROJECT_CREATE_COUNT_Result();
            List<SP_DISCUSSION_CREATE_COUNT_Result> recs2 = db_EECIP.GetSP_DISCUSSION_CREATE_COUNT_Result();

            return Json(new
            {
                Projects = recs,
                Discussions = recs2
            }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult MetricChartFreshness()
        {
            //first refresh the current month data
            db_EECIP.GetSP_RPT_FRESHNESS_RECORD_Result();

            //retrieve data
            List<T_OE_RPT_FRESHNESS> iData1 = db_EECIP.GetT_OE_RPT_FRESHNESS_ByCat(1);
            List<T_OE_RPT_FRESHNESS> iData2 = db_EECIP.GetT_OE_RPT_FRESHNESS_ByCat(2);
            List<T_OE_RPT_FRESHNESS> iData3 = db_EECIP.GetT_OE_RPT_FRESHNESS_ByCat(3);
            List<T_OE_RPT_FRESHNESS> iData4 = db_EECIP.GetT_OE_RPT_FRESHNESS_ByCat(4);

            return Json(new
            {
                Cat1 = iData1,
                Cat2 = iData2,
                Cat3 = iData3,
                Cat4 = iData4
            }, JsonRequestBehavior.AllowGet);
        }

    }
}