using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EECIP.App_Logic.BusinessLogicLayer;
using EECIP.App_Logic.DataAccessLayer;
using EECIP.Models;
using System.Web.Security;

namespace EECIP.Controllers
{
    [Authorize(Roles = "Admins")]
    public class AdminController : Controller
    {
        //************************************* USERS ************************************************************
        // GET: /Admin/Users
        public ActionResult Users()
        {
            var model = new vmAdminUsers
            {
                users = db_Accounts.GetT_OE_USERS()
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddAdminUser(vmAdminUsers model)
        {
            if (ModelState.IsValid)
            {
                MembershipCreateStatus status;

                //create user and send out verification email
                Membership.Provider.CreateUser(model.newUserEmail, "", model.newUserEmail, null, null, false, null, out status);
                if (status == MembershipCreateStatus.Success)
                {
                    int UserIDX = (int)Membership.GetUser(model.newUserEmail).ProviderUserKey;
                    //update first name and last name
                    db_Accounts.UpdateT_OE_USERS(UserIDX, null, null, model.newUserFName, model.newUserLName, null, null, null, null, null, null, null, null, null, null, null);
                    TempData["Success"] = "User created and verification email sent to user.";
                }
                else
                {
                    if (status.ToString() == "DuplicateUserName")
                        TempData["Error"] = "An account has already been created with that email address. Please recover lost password.";
                    else if (status.ToString() == "InvalidEmail")
                        TempData["Error"] = "Unable to send verification email. Please try again later.";
                    else
                        TempData["Error"] = status;
                }
            }
            else
                TempData["Error"] = "Unable to create user.";

            return RedirectToAction("Users");

        }


        //************************************* ROLES ************************************************************
        // GET: /Admin/Roles
        public ActionResult Roles()
        {
            return View(db_Accounts.GetT_OE_ROLES());
        }
        
        // GET: /Admin/RoleEdit/1
        public ActionResult RoleEdit(int id)
        {
            var model = new vmAdminRoleEdit
            {
                T_OE_ROLES = db_Accounts.GetT_OE_ROLEByIDX(id),
                Users_In_Role = db_Accounts.GetT_OE_USERSInRole(id).Select(x => new SelectListItem
                {
                    Value = x.USER_IDX.ToString(),
                    Text = x.USER_ID
                }),
                Users_Not_In_Role = db_Accounts.GetT_OE_USERSNotInRole(id).Select(x => new SelectListItem
                {
                    Value = x.USER_IDX.ToString(),
                    Text = x.USER_ID
                })
            };
            return View(model);
        }

        // POST: /Admin/RoleEdit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RoleEdit(vmAdminRoleEdit model, string submitButton)
        {
            int SuccID = 0;
            int UserIDX = db_Accounts.GetUserIDX();


            // ADDING USER TO ROLE
            if (submitButton == "Add")
            {
                foreach (string u in model.Users_Not_In_Role_Selected)
                    SuccID = db_Accounts.CreateT_OE_USER_ROLE(model.T_OE_ROLES.ROLE_IDX, u.ConvertOrDefault<int>(), UserIDX);

                if (SuccID > 0)
                    TempData["Success"] = "Update successful.";

                //return View(model);
                return RedirectToAction("RoleEdit", new { id = model.T_OE_ROLES.ROLE_IDX });

            }
            // REMOVE USER FROM ROLE
            else if (submitButton == "Remove")
            {
                foreach (string u in model.Users_In_Role_Selected)
                    SuccID = db_Accounts.DeleteT_OE_USER_ROLE(u.ConvertOrDefault<int>(), model.T_OE_ROLES.ROLE_IDX);

                if (SuccID > 0)
                    TempData["Success"] = "Update successful.";

                return RedirectToAction("RoleEdit", new { id = model.T_OE_ROLES.ROLE_IDX });
            }
            else
                return View(model);
        }


        //************************************* SETTINGS ************************************************************
        //GET: /Admin/Settings
        public ActionResult Settings()
        {
            var model = new vmAdminSettings
            {
                app_settings = db_Ref.GetT_OE_APP_SETTING_List()
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Settings(vmAdminSettings model)
        {
            if (ModelState.IsValid)
            {
                int SuccID = db_Ref.InsertUpdateT_OE_APP_SETTING(model.edit_app_setting.SETTING_IDX, model.edit_app_setting.SETTING_NAME, model.edit_app_setting.SETTING_VALUE, false, null, db_Accounts.GetUserIDX());
                if (SuccID > 0)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("Settings");
        }


        //*************************************** AGENCIES **********************************************************
        // GET: /Admin/Agencies
        public ActionResult RefAgencies() {
            var model = new vmAdminAgencies
            {
                agencies = db_Ref.GetT_OE_ORGANIZATION(false)
            };
            return View(model);
        }

        // GET: /Admin/RefAgencyEdit
        public ActionResult RefAgencyEdit(Guid? id)
        {
            var model = new vmAdminAgencyEdit();
            if (id != null)
            {
                model.agency = db_Ref.GetT_OE_ORGANIZATION_ByID(id.ConvertOrDefault<Guid>());
                model.agency_emails = db_Ref.GetT_OE_ORGANIZATION_EMAIL_RULES_ByID(id.ConvertOrDefault<Guid>());
            }
            else //add new case
            {
                model.agency = new T_OE_ORGANIZATION();
                model.agency.ORG_IDX = Guid.NewGuid();
                model.agency.ACT_IND = true;
                model.agency_emails = new List<T_OE_ORGANIZATION_EMAIL_RULE>();
            }

            return View(model);
        }

        // POST: /Admin/RefAgencyEdit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RefAgencyEdit(T_OE_ORGANIZATION agency)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            Guid? SuccID = db_Ref.InsertUpdatetT_OE_ORGANIZATION(agency.ORG_IDX, agency.ORG_ABBR, agency.ORG_NAME, agency.STATE_CD, agency.EPA_REGION, null, null, true, UserIDX);

            if (SuccID != null)
                TempData["Success"] = "Update successful.";
            else
                TempData["Error"] = "Error updating data.";

            //return View(model);
            return RedirectToAction("RefAgencyEdit", new { id = agency.ORG_IDX });

        }

        // POST: /Admin/RefAgencyEditEmail
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RefAgencyEditEmail(vmAdminAgencyEdit model)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            int SuccID = db_Ref.InsertT_OE_ORGANIZATION_EMAIL_RULE(model.agency.ORG_IDX, model.new_email, UserIDX);

            if (SuccID == 1)
                TempData["Success"] = "Update successful.";
            else
                TempData["Error"] = "Error updating data.";

            return RedirectToAction("RefAgencyEdit", new { id = model.agency.ORG_IDX });
        }

        // POST: /Admin/RefAgencyEditEmailDelete
        [HttpPost]
        public ActionResult RefAgencyEditEmailDelete(string id, string id2)
        {
            Guid org = new Guid(id);
            int SuccID = db_Ref.DeleteT_OE_ORGANIZATION_EMAIL_RULE(org, id2);
            if (SuccID == 0)
                TempData["Error"] = "Unable to delete record.";

            return RedirectToAction("RefAgencyEdit", "Admin", new { id = id });
        }



        //*************************************** REF ENTPERISE SERVICES **********************************************************
        // GET: /Admin/RefEntServices
        public ActionResult RefEntServices()
        {
            var model = new vmAdminRefEntServices
            {
                ent_services = db_Ref.GetT_OE_REF_ENTERPRISE_PLATFORM(true)
            };
            return View(model);
        }

        // POST: /Admin/RefEntServices
        [HttpPost]
        public ActionResult RefEntServices(vmAdminRefEntServices model)
        {
            if (ModelState.IsValid)
            {
                var z = model.edit_ent_services;
                int SuccID = db_Ref.InsertUpdatetT_OE_REF_ENTERPRISE_PLATFORM(z.ENT_PLATFORM_IDX, z.ENT_PLATFORM_NAME, z.ENT_PLATFORM_DESC, z.ENT_PLATFORM_EXAMPLE, z.SEQ_NO, true, db_Accounts.GetUserIDX());
                if (SuccID > 0)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("RefEntServices", "Admin");
        }

        // POST: /Admin/RefEntServicesDelete
        [HttpPost]
        public ActionResult RefEntServicesDelete(int id)
        {
            int SuccID = db_Ref.DeleteT_OE_REF_ENTERPRISE_PLATFORM(id);
            if (SuccID == 0)
                TempData["Error"] = "Unable to delete record.";

            return RedirectToAction("RefEntServices", "Admin");
        }


        //*************************************** REF TAGS **********************************************************
        // GET: /Admin/RefTags
        public ActionResult RefTags(string selTag)
        {
            var model = new vmAdminRefTags
            {
                tags = db_Ref.GetT_OE_REF_TAGS_ByCategory(selTag),
            };
            if (!string.IsNullOrEmpty(selTag))
                model.sel_tag_cat = selTag;

            return View(model);
        }


        // POST: /Admin/RefTagEdit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RefTagEdit(vmAdminRefTags model)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            int SuccID = db_Ref.InsertUpdatetT_OE_REF_TAGS(model.edit_tag_idx, model.edit_tag, model.sel_tag_cat, UserIDX);

            if (SuccID > 0)
                TempData["Success"] = "Update successful.";
            else
                TempData["Error"] = "Error updating data.";

            //return View(model);
            return RedirectToAction("RefTags", new { selTag = model.sel_tag_cat });

        }



        // POST: /Admin/RefTagsDelete
        [HttpPost]
        public ActionResult RefTagsDelete(int id)
        {
            int SuccID = db_Ref.DeleteT_OE_REF_TAGS(id);
            if (SuccID == 0)
                TempData["Error"] = "Unable to delete record.";

            return RedirectToAction("RefTags", "Admin");
        }




        //*************************************** SEARCH ADMIN **********************************************************
        // GET: /Admin/SearchAdmin
        public ActionResult SearchAdmin()
        {
            var model = new vmAdminSearch
            {
                synonyms = db_Ref.GetT_OE_REF_SYNONYMS()
            };


            return View(model);
        }

        [HttpPost]
        public ActionResult SearchAdminDeleteIndex()
        {
            string err = "";
            try
            {
                AzureSearch.DeleteSearchIndex();
                TempData["Success"] = "Search index deleted.";
            }
            catch (Exception ex) {
                err = ex.ToString();
            }

            if (err.Length > 0)
                TempData["Error"] = err;

            return RedirectToAction("SearchAdmin", "Admin");
        }


        [HttpPost]
        public ActionResult SearchAdminCreateIndex()
        {
            string err = "";
            try
            {
                AzureSearch.CreateSearchIndex();
                TempData["Success"] = "Search index created.";
            }
            catch (Exception ex)
            {
                err = ex.ToString();
            }

            if (err.Length > 0)
                TempData["Error"] = err;

            return RedirectToAction("SearchAdmin", "Admin");
        }


        [HttpPost]
        public ActionResult SearchAdminPopulateIndex()
        {
            string err = "";
            try
            {
                //***********PROJECTS **************************
                //first reset all projects to unsynced
                db_EECIP.ResetT_OE_PROJECTS_Unsynced();

                //then send all unsynced projects to Azure
                AzureSearch.PopulateSearchIndexProject(null);


                //***********AGENCIES  **************************
                //reset all agencies to unsynced
                db_Ref.ResetT_OE_ORGANIZATION_Unsynced();

                //then send all unsynced agencies to Azure
                AzureSearch.PopulateSearchIndexOrganization(null);


                //***********ENT SERVICES  **************************
                db_EECIP.ResetT_OE_ORGANIZATION_ENT_SVCS_Unsynced();

                //then send all unsynced agencies to Azure
                AzureSearch.PopulateSearchIndexEntServices(null);


                //***********PEOPLE  **************************
                db_Accounts.ResetT_OE_USERS_Unsynced();

                //then send all unsynced agencies to Azure
                AzureSearch.PopulateSearchIndexUsers(null);


                TempData["Success"] = "Search index populated.";
            }
            catch (Exception ex)
            {
                err = ex.ToString();
            }

            if (err.Length > 0)
                TempData["Error"] = err;
            return RedirectToAction("SearchAdmin", "Admin");
        }


        [HttpPost]
        public ActionResult SearchAdminUploadSynonyms()
        {
            string err = "";
            try
            {
                AzureSearch.UploadSynonyms();

                AzureSearch.EnableSynonyms();

                TempData["Success"] = "Synonyms uploaded and enabled.";
            }
            catch (Exception ex)
            {
                err = ex.ToString();
            }

            TempData["Error"] = err;
            return RedirectToAction("SearchAdmin", "Admin");
        }


        [HttpGet]
        public ActionResult SearchAdminSuggest(string term, bool fuzzy = true)
        {
            // Call suggest query and return results
            var response = AzureSearch.Suggest(term, fuzzy);
            List<string> suggestions = new List<string>();
            foreach (var result in response.Results)
            {
                suggestions.Add(result.Text);
            }

            // Get unique items
            List<string> uniqueItems = suggestions.Distinct().ToList();

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = uniqueItems
            };

        }


        // POST: /Admin/SearchAdminSynonymDelete
        [HttpPost]
        public ActionResult SearchAdminSynonymDelete(int id)
        {
            int SuccID = db_Ref.DeleteT_OE_REF_SYNONYMS(id);
            if (SuccID == 0)
                TempData["Error"] = "Unable to delete record.";
            else
            {
                AzureSearch.UploadSynonyms();
                AzureSearch.EnableSynonyms();
            }
            return RedirectToAction("SearchAdmin", "Admin");
        }


        // POST: /Admin/SearchAdminSynonymEdit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SearchAdminSynonymEdit(vmAdminSearch model)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            int SuccID = db_Ref.InsertUpdatetT_OE_REF_SYNONYMS(model.edit_synonym_idx, model.edit_synonym_text, UserIDX);

            if (SuccID > 0)
            {
                AzureSearch.UploadSynonyms();
                AzureSearch.EnableSynonyms();
                TempData["Success"] = "Update successful.";

            }
            else
                TempData["Error"] = "Error updating data.";

            //return View(model);
            return RedirectToAction("SearchAdmin");

        }



        // ***************************************** IMPORT DATA *************************************************************
        public ActionResult ImportData()
        {
            var model = new vmAdminImportData
            {
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ImportData(vmAdminImportData model)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            //set dictionaries used to store stuff in memory
            Dictionary<string, int> colMapping = new Dictionary<string, int>();  //identifies the column number for each field to be imported

            //initialize variables
            bool headInd = true;
            bool anyError = false;
            //loop through each row
            foreach (string row in model.IMPORT_BLOCK.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                //split row's columns into string array
                string[] cols = row.Split(new char[] { '\t' }, StringSplitOptions.None);

                if (cols.Length > 0) //skip blank rows
                {
                    if (headInd)
                    {                   
                        //**********************************************************
                        //HEADER ROW - LOGIC TO DETERMINE WHAT IS IN EACH COLUMN
                        //**********************************************************
                        colMapping = Utils.GetColumnMapping("P", cols);

                        headInd = false;

                        model.projects = new List<ProjectImportType>();
                    }
                    else
                    {
                        //**********************************************************
                        //NOT HEADER ROW - READING IN VALUES
                        //**********************************************************
                        var colList = cols.Select((value, index) => new { value, index });
                        var colDataIndexed = (from f in colMapping
                                              join c in colList on f.Value equals c.index
                                              select new
                                              {
                                                  _Name = f.Key,
                                                  _Val = c.value
                                              }).ToList();

                        Dictionary<string, string> fieldValuesDict = new Dictionary<string, string>();  //identifies the column number for each field to be imported

                        //loop through all values and insert to list
                        foreach (var c in colDataIndexed)
                            fieldValuesDict.Add(c._Name, c._Val);

                        //VALIDATE ROW AND INSERT TO LOCAL OBJECT
                        ProjectImportType p = db_EECIP.InsertOrUpdate_T_OE_PROJECT_local(UserIDX, fieldValuesDict);
                        if (p.VALIDATE_CD == false)
                            anyError = true;

                        model.projects.Add(p);
                    }
                }
            } //end each row

            //if no errors, just import. otherwise 
            if (!anyError)
            {
                foreach (ProjectImportType ps in model.projects)
                {
                    //import projects
                    T_OE_PROJECTS x = ps.T_OE_PROJECT;
                    Guid? ProjectIDX = db_EECIP.InsertUpdatetT_OE_PROJECTS(x.PROJECT_IDX, x.ORG_IDX, x.PROJ_NAME, x.PROJ_DESC, x.MEDIA_TAG, x.START_YEAR, x.PROJ_STATUS, 
                        x.DATE_LAST_UPDATE, x.RECORD_SOURCE, x.PROJECT_URL, x.MOBILE_IND, x.MOBILE_DESC, x.ADV_MON_IND, x.ADV_MON_DESC, x.BP_MODERN_IND,
                        x.BP_MODERN_DESC, x.COTS, x.VENDOR, true, false, UserIDX, null, x.IMPORT_ID);

                    //import features
                    if (ps.FEATURES != null)
                    {
                        foreach (string f in ps.FEATURES.Split('|'))
                            if (f.Length > 0)
                                db_EECIP.InsertT_OE_PROJECT_TAGS(ProjectIDX.ConvertOrDefault<Guid>(), "Project Feature", f, UserIDX);
                    }

                    //import program areas
                    if (ps.PROGRAM_AREAS != null)
                    {
                        foreach (string f in ps.PROGRAM_AREAS.Split('|'))
                            if (f.Length > 0)
                                db_EECIP.InsertT_OE_PROJECT_TAGS(ProjectIDX.ConvertOrDefault<Guid>(), "Program Area", f, UserIDX);
                    }
                }

                //update azure search
                AzureSearch.PopulateSearchIndexProject(null);

                //clear form
                model.IMPORT_BLOCK = "";
                model.projects = null;
                TempData["Success"] = "Data imported successfully";
            }
            return View(model);
        }




    }
}