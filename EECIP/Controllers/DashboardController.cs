using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EECIP.Models;
using EECIP.App_Logic.BusinessLogicLayer;
using EECIP.App_Logic.DataAccessLayer;
using Microsoft.Azure.Search.Models;

namespace EECIP.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            var model = new vmDashboardSearch();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(vmDashboardSearch model)
        {
            //int UserIDX = db_Accounts.GetUserIDX();

            model.searchResults = AzureSearch.QuerySearchIndex(model.searchStr);

            return View(model);
        }



        // GET: Agency
        public ActionResult Agency()
        {
            // get the agency for which the logged in user is associated
            T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(db_Accounts.GetUserIDX());
            if (u != null && u.ORG_IDX != null)
            {
                var model = new vmDashboardAgency();
                model.agency = db_Ref.GetT_OE_ORGANIZATION_ByID(u.ORG_IDX.ConvertOrDefault<Guid>());
                model.users = db_Accounts.GetT_OE_USERSByAgency(u.ORG_IDX.ConvertOrDefault<Guid>());
                //database
                model.SelectedDatabase = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByOrgAttribute(u.ORG_IDX.ConvertOrDefault<Guid>(), "Database");
                model.AllDatabase = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByAttributeAll(u.ORG_IDX.ConvertOrDefault<Guid>(), "Database").Select(x => new SelectListItem { Value = x, Text = x });
                //app framework
                model.SelectedAppFramework = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByOrgAttribute(u.ORG_IDX.ConvertOrDefault<Guid>(), "App Framework");
                model.AllAppFramework = db_Ref.GetT_OE_ORGANIZATION_TAGS_ByAttributeAll(u.ORG_IDX.ConvertOrDefault<Guid>(), "App Framework").Select(x => new SelectListItem { Value = x, Text = x });
                //ent services
                model.org_ent_services = db_EECIP.GetT_OE_ORGANIZATION_ENTERPRISE_PLATFORM(u.ORG_IDX.ConvertOrDefault<Guid>());

                return View(model);
            }

            TempData["Error"] = "You are not associated with an agency.";
            return RedirectToAction("AccessDenied","Home");
        }

        // POST: /Dashboard/AgencyEdit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AgencyEdit(vmDashboardAgency model)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            //update general agency data
            Guid? SuccID = db_Ref.InsertUpdatetT_OE_ORGANIZATION(model.agency.ORG_IDX, null, null, null, null, model.agency.CLOUD, model.agency.API, true, UserIDX);


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

            return RedirectToAction("Agency");
        }

        [HttpPost]
        public ActionResult AgencyEntServiceEdit(vmDashboardAgency model)
        {
            if (ModelState.IsValid)
            {
                var z = model.edit_ent_services;
                int SuccID = db_EECIP.InsertUpdatetT_OE_ORGANIZATION_ENT_SVCS(z.ENT_PLATFORM_IDX, model.agency.ORG_IDX, z.ENT_PLATFORM_IDX, z.PROJECT_NAME, z.VENDOR, z.IMPLEMENT_STATUS,
                    z.COMMENTS, db_Accounts.GetUserIDX());
                if (SuccID > 0)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("Agency", "Dashboard");
        }



        // GET: Projects
        public ActionResult Projects()
        {
            // get the agency for which the logged in user is associated
            T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(db_Accounts.GetUserIDX());
            if (u != null && u.ORG_IDX != null)
            {
                var model = new vmDashboardProjects();
                model.projects = db_EECIP.GetT_OE_PROJECTS_ByOrgIDX(u.ORG_IDX.ConvertOrDefault<Guid>());

                return View(model);
            }

            TempData["Error"] = "You are not associated with an agency.";
            return RedirectToAction("AccessDenied", "Home");

        }


        // GET: ProjectDetails/1
        public ActionResult ProjectDetails(Guid? id)
        {
            // get the agency for which the logged in user is associated
            T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(db_Accounts.GetUserIDX());
            if (u != null)
            {
                if (u.ORG_IDX == null)
                {
                    TempData["Error"] = "You are not associated with an agency.";
                    return RedirectToAction("AccessDenied", "Home");
                }

                var model = new vmDashboardProjectDetails();
                model.project = db_EECIP.GetT_OE_PROJECTS_ByIDX(id);
                if (model.project == null)
                {
                    model.project = new T_OE_PROJECTS
                    {
                        ORG_IDX = u.ORG_IDX,
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

            TempData["Error"] = "You are unauthorized to edit projects.";
            return RedirectToAction("AccessDenied", "Home");
        }

        // POST: /Dashboard/ProjectEdit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ProjectEdit(vmDashboardProjectDetails model)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            //update project data
            Guid? SuccID = db_EECIP.InsertUpdatetT_OE_PROJECTS(model.project.PROJECT_IDX, model.project.ORG_IDX, model.project.PROJ_NAME, 
                model.project.PROJ_DESC, model.project.MEDIA_TAG, model.project.START_YEAR, model.project.PROJ_STATUS, 
                model.project.DATE_LAST_UPDATE, model.project.RECORD_SOURCE, model.project.PROJECT_URL, model.project.MOBILE_IND,
                model.project.MOBILE_DESC, model.project.ADV_MON_IND, model.project.ADV_MON_DESC, model.project.BP_MODERN_IND,
                model.project.BP_MODERN_DESC, model.project.COTS, model.project.VENDOR, true, false, UserIDX);

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

            return RedirectToAction("Projects");
        }


        [HttpPost]
        public ActionResult ProjectsDelete(Guid id)
        {
            int SuccID = db_EECIP.DeleteT_OE_PROJECTS(id);
            if (SuccID == 0)
                TempData["Error"] = "Unable to delete record.";

            return RedirectToAction("Projects", "Dashboard");
        }


        // GET: ProjectDetails/1
        public ActionResult ProjectCard(string strid)
        {
            Guid id = Guid.Parse(strid);
            var model = new vmDashboardProjectCard();
            model.project = db_EECIP.GetT_OE_PROJECTS_ByIDX(id);
            if (model.project != null)
            {
                model.SelectedProgramAreas = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeSelected(model.project.PROJECT_IDX, "Program Area");
                model.SelectedFeatures = db_EECIP.GetT_OE_PROJECT_TAGS_ByAttributeSelected(model.project.PROJECT_IDX, "Project Feature");
            }
            return View(model);


        }

    }
}