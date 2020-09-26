using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EECIP.App_Logic.BusinessLogicLayer;
using EECIP.App_Logic.DataAccessLayer;
using EECIP.Models;
using System.Web.Security;
using System.IO;
using System.Data;
using ClosedXML.Excel;

namespace EECIP.Controllers
{
    [Authorize(Roles = "Admins")]
    public class AdminController : Controller
    {
        //************************************* USERS ************************************************************
        // GET: /Admin/Users
        public ActionResult Users(string sort, int? currPage, Guid? selAgency)
        {
            var model = new vmAdminUsers
            {
                users = db_Accounts.GetT_OE_USERS_Search(sort, currPage ?? 1, selAgency),
                ddl_Agencies = ddlHelpers.get_ddl_organizations(true, true),
                selAgency = selAgency,
                userCount = (selAgency == null ? db_Accounts.GetT_OE_USERS_Count() : 1),
                currPage = currPage,
                strSort = sort
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
                MembershipUser _u = Membership.Provider.CreateUser(model.newUserEmail, "", model.newUserEmail, null, null, false, null, out status);
                if (status == MembershipCreateStatus.Success)
                {
                    int UserIDX = _u.ProviderUserKey.ConvertOrDefault<int>();
                    //update first name and last name
                    db_Accounts.UpdateT_OE_USERS(UserIDX, null, null, model.newUserFName, model.newUserLName, null, null, null, null, null, null, null, null, null, null, null, null, null, false, true, true, true);
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

        // POST: /Dashboard/UserDelete
        [HttpPost]
        public JsonResult UserDelete(int id)
        {

            int SuccID = db_Accounts.DeleteT_OE_USERS(id);
            AzureSearch.DeleteSearchIndexUsers(id);
            if (SuccID>0)
            {
                //SUCCESS - now delete user from Azure search
                return Json("Success");
            }
            else
                return Json("User has been made inactive instead of being deleted due to data in the database.");
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


        // POST: /Admin/RoleEdit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UserRoleEdit(vmAccountUserProfile model, string submitButton)
        {
            int SuccID = 0;
            int UserIDX = model.UserIDX;
            int AdmIDX = db_Accounts.GetUserIDX();

            // ADDING USER TO ROLE
            if (submitButton == "Add")
            {
                foreach (string u in model.Roles_Not_In_User_Selected)
                    SuccID = db_Accounts.CreateT_OE_USER_ROLE(u.ConvertOrDefault<int>(), UserIDX, AdmIDX);

                if (SuccID > 0)
                    TempData["Success"] = "Update successful.";

                //return View(model);
                return RedirectToAction("UserProfile", "Account", new { id = UserIDX, a = "a" });

            }
            // REMOVE USER FROM ROLE
            else if (submitButton == "Remove")
            {
                foreach (string u in model.Roles_In_User_Selected)
                    SuccID = db_Accounts.DeleteT_OE_USER_ROLE(UserIDX, u.ConvertOrDefault<int>());

                if (SuccID > 0)
                    TempData["Success"] = "Update successful.";

                return RedirectToAction("UserProfile", "Account", new { id = UserIDX, a = "a" });
            }
            else
                return View(model);
        }



        //************************************* SETTINGS ************************************************************
        //GET: /Admin/Settings
        public ActionResult Settings()
        {
            T_OE_APP_SETTINGS_CUSTOM custSettings = db_Ref.GetT_OE_APP_SETTING_CUSTOM();

            var model = new vmAdminSettings
            {
                app_settings = db_Ref.GetT_OE_APP_SETTING_List(),
                TermsAndConditions = custSettings.TERMS_AND_CONDITIONS,
                Announcements = custSettings.ANNOUNCEMENTS,
                WelcomeEmail = custSettings.WELCOME_EMAIL
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

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CustomSettings(vmAdminSettings model)
        {
            if (ModelState.IsValid)
            {
                int SuccID = db_Ref.InsertUpdateT_OE_APP_SETTING_CUSTOM(model.TermsAndConditions, null, null);
                if (SuccID > 0)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("Settings");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CustomSettingsAnnounce(vmAdminSettings model)
        {
            if (ModelState.IsValid)
            {
                int SuccID = db_Ref.InsertUpdateT_OE_APP_SETTING_CUSTOM(null, model.Announcements ?? "", null);
                if (SuccID > 0)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("Settings");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CustomSettingsWelcomeEmail(vmAdminSettings model)
        {
            if (ModelState.IsValid)
            {
                int SuccID = db_Ref.InsertUpdateT_OE_APP_SETTING_CUSTOM(null, null, model.WelcomeEmail ?? "");
                if (SuccID > 0)
                    TempData["Success"] = "Data Saved.";
                else
                    TempData["Error"] = "Data Not Saved.";
            }

            return RedirectToAction("Settings");
        }



        //*************************************** AGENCIES **********************************************************
        // GET: /Admin/Agencies
        public ActionResult RefAgencies(string typ) {
            var model = new vmAdminAgencies
            {
                agencies = db_Ref.GetT_OE_ORGANIZATION(false, typ != "Governance", typ),
                GovInd = typ
            };
            return View(model);
        }

        // GET: /Admin/RefAgencyEdit
        public ActionResult RefAgencyEdit(Guid? id, string typ)
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
                if (typ != null)
                    model.agency.ORG_TYPE = typ;
                model.agency.ACT_IND = true;
                model.agency_emails = new List<T_OE_ORGANIZATION_EMAIL_RULE>();
            }

            model.GovInd = typ;
            return View(model);
        }

        // POST: /Admin/RefAgencyEdit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RefAgencyEdit(T_OE_ORGANIZATION agency)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            Guid? SuccID = db_Ref.InsertUpdatetT_OE_ORGANIZATION(agency.ORG_IDX, agency.ORG_ABBR, agency.ORG_NAME, agency.STATE_CD, agency.EPA_REGION, agency.ORG_TYPE, null, null, true, UserIDX);

            if (SuccID != null)
            {
                AzureSearch.PopulateSearchIndexOrganization(SuccID);
                TempData["Success"] = "Update successful.";
            }
            else
                TempData["Error"] = "Error updating data.";

            return RedirectToAction("RefAgencyEdit", new { id = SuccID });
        }

        // POST: /Admin/RefAgencyDelete
        [HttpPost]
        public JsonResult RefAgencyDelete(string id)
        {
            string response = "";

            if (id != null)
            {
                Guid org = new Guid(id);
                int SuccID = db_Ref.DeleteT_OE_ORGANIZATION(org);

                //now delete from Azure
                AzureSearch.DeleteSearchIndexAgency(id);

                if (SuccID == -1)
                    response = "Cannot delete agency because agency users exist.";
                else if (SuccID == -2)
                    response = "Cannot delete agency because agency projects exist.";
                else if (SuccID == -3)
                    response = "Cannot delete agency because agency enterprise services exist.";
                else
                    response = "Success";
            }
            else
                response = "Unable to delete agency";

            return Json(response);
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
        public JsonResult RefAgencyEditEmailDelete(string id, string id2)
        {
            Guid org = new Guid(id);
            int SuccID = db_Ref.DeleteT_OE_ORGANIZATION_EMAIL_RULE(org, id2);
            if (SuccID == 0)
                return Json("Unable to delete record.");
            else
                return Json("Success");
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
        public JsonResult RefEntServicesDelete(int id)
        {
            int SuccID = db_Ref.DeleteT_OE_REF_ENTERPRISE_PLATFORM(id);
            if (SuccID == 0)
                return Json("Unable to delete record.");
            else
                return Json("Success");
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
            {
                model.sel_tag_cat = selTag;
                model.sel_tag_cat_desc = db_Ref.GetT_OE_REF_TAG_CATEGORIES_ByName(selTag).TAG_CAT_DESCRIPTION;
            }

            return View(model);
        }


        // POST: /Admin/RefTagEdit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RefTagEdit(vmAdminRefTags model)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            int SuccID = db_Ref.InsertUpdatetT_OE_REF_TAGS(model.edit_tag_idx, model.edit_tag, model.edit_tag_desc, model.sel_tag_cat, model.edit_promote_ind, UserIDX);

            if (SuccID > 0)
                TempData["Success"] = "Update successful.";
            else
                TempData["Error"] = "Error updating data.";

            //return View(model);
            return RedirectToAction("RefTags", new { selTag = model.sel_tag_cat });

        }



        // POST: /Admin/RefTagsDelete
        [HttpPost]
        public JsonResult RefTagsDelete(int id)
        {
            int SuccID = db_Ref.DeleteT_OE_REF_TAGS(id);
            if (SuccID == 0)
                return Json("Unable to delete record.");
            else
                return Json("Success");
        }



        //*************************************** REF BADGES **********************************************************
        // GET: /Admin/RefBadge
        public ActionResult RefBadge()
        {
            var model = new vmAdminRefBadges
            {
                _badge = db_Forum.GetBadges()
            };

            return View(model);
        }


        // POST: /Admin/RefBadge
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RefBadge(vmAdminRefBadges model)
        {
            Guid? SuccID = db_Forum.InsertUpdateBadge(model.edit_badge.Id, model.edit_badge.Type, model.edit_badge.Name, model.edit_badge.DisplayName, model.edit_badge.Description, model.edit_badge.Image, 
                model.edit_badge.AwardsPoints);

            if (SuccID != null)
                TempData["Success"] = "Update successful.";
            else
                TempData["Error"] = "Error updating data.";

            //return View(model);
            return RedirectToAction("RefBadge");

        }



        //*************************************** SYS LOG **********************************************************
        // GET: /Admin/SysLog
        public ActionResult SysLog()
        {
            var model = new vmAdminSysLog
            {
                T_OE_SYS_LOG = db_Ref.GetT_OE_SYS_LOG(1, 100),
                T_OE_SYS_EMAIL_LOG = db_Ref.GetT_OE_SYS_EMAIL_LOG(1, 100)
            };

            return View(model);
        }


        //*************************************** EMAIL TEMPLATES **********************************************************
        // GET: /Admin/AdmEmail
        public ActionResult AdmEmail()
        {
            var model = new vmAdminEmail
            {
                T_OE_REF_EMAIL_TEMPLATE = db_Ref.GetT_OE_REF_EMAIL_TEMPLATE()
            };

            return View(model);
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
            try
            {
                AzureSearch.DeleteSearchIndex();
                TempData["Success"] = "Search index deleted.";
            }
            catch (Exception ex) {
                TempData["Error"] = ex.ToString().SubStringPlus(0, 100);
            }

            return RedirectToAction("SearchAdmin", "Admin");
        }


        [HttpPost]
        public ActionResult SearchAdminCreateIndex()
        {
            try
            {
                AzureSearch.CreateSearchIndex();
                TempData["Success"] = "Search index created.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.ToString().SubStringPlus(0,100);
            }

            return RedirectToAction("SearchAdmin", "Admin");
        }


        [HttpPost]
        public ActionResult SearchAdminPopulateIndex()
        {
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


                //***********FORUM TOPICS  **************************
                db_Forum.UpdatePost_SetAllUnsynced();

                //then send all unsynced agencies to Azure
                AzureSearch.PopulateSearchIndexForumPost(null);



                TempData["Success"] = "Search index populated.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.ToString().SubStringPlus(0, 100);
            }

            return RedirectToAction("SearchAdmin", "Admin");
        }


        [HttpPost]
        public ActionResult SearchAdminUploadSynonyms()
        {
            try
            {
                AzureSearch.UploadSynonyms();

                AzureSearch.EnableSynonyms();

                TempData["Success"] = "Synonyms uploaded and enabled.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.ToString().SubStringPlus(0, 100);
            }

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
        public JsonResult SearchAdminSynonymDelete(int id)
        {
            int SuccID = db_Ref.DeleteT_OE_REF_SYNONYMS(id);
            if (SuccID == 0)
                return Json("Unable to delete record.");
            else
            {
                AzureSearch.UploadSynonyms();
                AzureSearch.EnableSynonyms();
                return Json("Success");
            }
        }


        // POST: /Admin/SearchAdminSynonymEdit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SearchAdminSynonymEdit(vmAdminSearch model)
        {
            int UserIDX = db_Accounts.GetUserIDX();
            int SuccID = 0;
            if (model.edit_synonym_text != null)
            {
                SuccID = db_Ref.InsertUpdatetT_OE_REF_SYNONYMS(model.edit_synonym_idx, model.edit_synonym_text, UserIDX);
            }
            else if (model.edit_synonym_bulk != null)
            {
                foreach (string row in model.edit_synonym_bulk.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    SuccID = db_Ref.InsertUpdatetT_OE_REF_SYNONYMS(null, row, UserIDX);
                }
            }

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



        //*************************************** NEWSLETTER TESTING **********************************************************
        public ActionResult Newsletter()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Newsletter(string s, string key)
        {
            var model = new vmAdminNewsletter();

            try
            {
                //authenticate
                int UserIDX = db_Accounts.GetUserIDX();
                T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(UserIDX);
                if (u != null && u.PWD_HASH == key)
                {
                    List<string> _results = App_Logic.NewsletterClass.generateNewsletter(String.IsNullOrEmpty(s) ? null : s);

                    //update the next run time if no override email set
                    if (String.IsNullOrEmpty(s))
                    {
                        string _newnext = System.DateTime.Now.AddMonths(1).ToString("MM/dd/yyyy HH:mm");
                        db_Ref.InsertUpdateT_OE_APP_SETTING(null, "NEWSLETTER_NEXT_RUN", _newnext, false, null);
                    }

                    model.results = _results;
                    TempData["Success"] = "Done.";
                    return View(model);
                }
                else {
                    TempData["Error"] = "Invalid key";
                    return RedirectToAction("Newsletter", "Admin");
                }

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.ToString().SubStringPlus(0, 100);
                return RedirectToAction("Newsletter", "Admin");
            }

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
                    if (ps.DEL_IND == false)
                    {

                        //import projects
                        T_OE_PROJECTS x = ps.T_OE_PROJECT;
                        Guid? ProjectIDX = db_EECIP.InsertUpdatetT_OE_PROJECTS(x.PROJECT_IDX, null, x.PROJ_NAME, x.PROJ_DESC, x.MEDIA_TAG, x.START_YEAR, x.PROJ_STATUS,
                            x.DATE_LAST_UPDATE, x.RECORD_SOURCE, x.PROJECT_URL, x.MOBILE_IND, x.MOBILE_DESC, x.ADV_MON_IND, x.ADV_MON_DESC, x.BP_MODERN_IND,
                            x.BP_MODERN_DESC, x.COTS, x.VENDOR, x.PROJECT_CONTACT, -1, true, false, UserIDX, x.IMPORT_ID, true);

                        //import orject orgs
                        if (x.ORG_IDX != null)
                        {
                            Guid? ProjectOrgIDX = db_EECIP.InsertUpdateT_OE_PROJECT_ORGS(ProjectIDX.ConvertOrDefault<Guid>(), x.ORG_IDX.ConvertOrDefault<Guid>(), UserIDX);
                        }

                        //import project url
                        if (!string.IsNullOrEmpty(x.PROJECT_URL))
                        {
                            db_EECIP.DeleteT_OE_PROJECTS_URL(ProjectIDX.ConvertOrDefault<Guid>());
                            db_EECIP.InsertT_OE_PROJECTS_URL(ProjectIDX.ConvertOrDefault<Guid>(), x.PROJECT_URL, null);
                        }

                        //import features
                        if (ps.FEATURES != null)
                        {
                            foreach (string f in ps.FEATURES.Split('|'))
                                if (f.Trim().Length > 0)
                                    db_EECIP.InsertT_OE_PROJECT_TAGS(ProjectIDX.ConvertOrDefault<Guid>(), "Tags", f, UserIDX);
                        }

                        //import program areas
                        if (ps.PROGRAM_AREAS != null)
                        {
                            foreach (string f in ps.PROGRAM_AREAS.Split('|'))
                                if (f.Trim().Length > 0)
                                    db_EECIP.InsertT_OE_PROJECT_TAGS(ProjectIDX.ConvertOrDefault<Guid>(), "Program Area", f, UserIDX);
                        }
                    }
                    else if (ps.DEL_IND == true && ps.T_OE_PROJECT.PROJECT_IDX != null)
                    {
                        //delete project
                        int SuccID = db_EECIP.DeleteT_OE_PROJECTS(ps.T_OE_PROJECT.PROJECT_IDX);
                        if (SuccID > 0)
                        {
                            //SUCCESS - now delete from Azure
                            List<T_OE_ORGANIZATION> orgs = db_EECIP.GetT_OE_PROJECT_ORGS_ByProject(ps.T_OE_PROJECT.PROJECT_IDX);
                            if (orgs != null)
                            {
                                foreach (T_OE_ORGANIZATION org in orgs) {
                                    AzureSearch.DeleteSearchIndexKey(ps.T_OE_PROJECT.PROJECT_IDX.ToString() + "_" + org.ORG_IDX.ToString());
                                }
                            }
                        }
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

        //*************************************** EXPORT DATA **********************************************************
        public ActionResult ExportData()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ExportData2(string[] exportdata)
        {
            //****validation **************
            if (exportdata == null)
            {
                TempData["Error"] = "You must select at least one option.";
                return RedirectToAction("ExportData", "Admin");
            }
            //***end validation ***********

            DataTable dtUsers = new DataTable("Users");
            DataTable dtAgency = new DataTable("Agencies");
            DataTable dtProject = new DataTable("Project");
            DataTable dtServices = new DataTable("Service");
            DataTable dtSearch = new DataTable("Search");

            List<T_OE_ORGANIZATION> oOrgList = db_Ref.GetT_OE_ORGANIZATION(true, false, null);



            //******************************USERS ******************************
            if (exportdata.Contains("Users"))
            {
                dtUsers.Columns.AddRange(new DataColumn[10] {
                                            new DataColumn("User IDX"),
                                            new DataColumn("Agency"),
                                            new DataColumn("Email"),
                                            new DataColumn("First"),
                                            new DataColumn("Last"),
                                            new DataColumn("Last Login"),
                                            new DataColumn("Phone"),
                                            new DataColumn("Title"),
                                            new DataColumn("Create Date"),
                                            new DataColumn("Last Modify Date")
                                           });

                List<UserDisplayType> _users = db_Accounts.GetT_OE_USERS();
                foreach (var item in _users)
                    dtUsers.Rows.Add(item.users.USER_IDX, item.ORG_NAME, item.users.EMAIL, item.users.FNAME, item.users.LNAME, item.users.LASTLOGIN_DT, item.users.PHONE,
                        item.users.JOB_TITLE, item.users.CREATE_DT, item.users.MODIFY_DT);
            }



            //******************************AGENCIES ******************************
            if (exportdata.Contains("Agencies")) {
                dtAgency.Columns.AddRange(new DataColumn[9] {
                                            new DataColumn("Agency ID"),
                                            new DataColumn("Agency Name"),
                                            new DataColumn("State Code"),
                                            new DataColumn("EPA Region"),
                                            new DataColumn("Cloud"),
                                            new DataColumn("API"),
                                            new DataColumn("Org Type"),
                                            new DataColumn("Create Date"),
                                            new DataColumn("Last Modify Date")
                                           });


                foreach (var item in oOrgList)
                    dtAgency.Rows.Add(item.ORG_IDX, item.ORG_NAME, item.STATE_CD, item.EPA_REGION, item.CLOUD, item.API, item.ORG_TYPE, item.CREATE_DT, item.MODIFY_DT);
            }


            //******************************PROJECTS ******************************
            if (exportdata.Contains("Projects"))
            {
                dtProject.Columns.AddRange(new DataColumn[24] {
                                            new DataColumn("Agency Name"),
                                            new DataColumn("Record Source"),
                                            new DataColumn("Project Name"),
                                            new DataColumn("Project Description"),
                                            new DataColumn("Media"),
                                            new DataColumn("Start Year"),
                                            new DataColumn("Project Status"),
                                            new DataColumn("Year Last Updated"),
                                            new DataColumn("Project URL"),
                                            new DataColumn("Mobile Ind"),
                                            new DataColumn("Mobile Description"),
                                            new DataColumn("Adv Mon Ind"),
                                            new DataColumn("Adv Mon Description"),
                                            new DataColumn("BP Improvement Ind"),
                                            new DataColumn("BP Improvement Desc"),
                                            new DataColumn("COTS"),
                                            new DataColumn("Vendor"),
                                            new DataColumn("Project Contact"),
                                            new DataColumn("EECIP Project ID"),
                                            new DataColumn("Import ID"),
                                            new DataColumn("Program Areas"),
                                            new DataColumn("Features"),
                                            new DataColumn("Delete Ind"),
                                            new DataColumn("External Source ID")
                                           });


                foreach (var oOneOrgName in oOrgList)
                {
                    List<T_OE_PROJECTS> oProject = db_EECIP.GetT_OE_PROJECTS_ByOrgIDX(oOneOrgName.ORG_IDX.ConvertOrDefault<Guid>());
                    foreach (var oOneProject in oProject)
                    {
                        string AgencyName = oOneOrgName.ORG_NAME;
                        string RecordSource = oOneProject.RECORD_SOURCE;
                        string ProjectName = oOneProject.PROJ_NAME;
                        string ProjectDescription = oOneProject.PROJ_DESC;
                        string Media = "";
                        if (oOneProject.T_OE_REF_TAGS2 != null)
                        {
                            Media = oOneProject.T_OE_REF_TAGS2.TAG_NAME;
                        }
                        int? StartYear = oOneProject.START_YEAR;
                        string ProjectStatus = oOneProject.PROJ_STATUS;
                        int? YearLastUpdated = oOneProject.DATE_LAST_UPDATE;
                        string ProjectURL = "";

                        List<T_OE_PROJECT_URLS> oURL = db_EECIP.GetT_OE_PROJECTS_URL_ByProjIDX(oOneProject.PROJECT_IDX);
                        foreach (var suburl in oURL)
                        {
                            if (suburl.PROJECT_URL != "")
                            {
                                if (ProjectURL == "")
                                {
                                    ProjectURL = suburl.PROJECT_URL;
                                }
                                else
                                {
                                    ProjectURL = ProjectURL + "|" + suburl.PROJECT_URL;
                                }
                            }
                        }

                        int? MobileInd = oOneProject.MOBILE_IND;
                        string MobileTagName = "";
                        if (MobileInd != null)
                        {
                            MobileTagName = db_Ref.GetT_OE_REF_TAGS_ByID(MobileInd);
                        }
                        string MobileDescription = oOneProject.MOBILE_DESC;

                        int? AdvMonInd = oOneProject.ADV_MON_IND;
                        string AdvMonTagName = "";
                        if (AdvMonInd != null)
                        {
                            AdvMonTagName = db_Ref.GetT_OE_REF_TAGS_ByID(AdvMonInd);
                        }
                        string AdvMonDescription = oOneProject.ADV_MON_DESC;

                        int? BPImprovementInd = oOneProject.BP_MODERN_IND;
                        string BPImprovementTagName = "";
                        if (BPImprovementInd != null)
                        {
                            BPImprovementTagName = db_Ref.GetT_OE_REF_TAGS_ByID(BPImprovementInd);
                        }
                        string BPImprovementDesc = oOneProject.BP_MODERN_DESC;
                        string COTS = oOneProject.COTS;
                        string Vendor = oOneProject.VENDOR;
                        string ProjectContact = oOneProject.PROJECT_CONTACT;
                        Guid EECIPProjectID = oOneProject.PROJECT_IDX;
                        string ImportID = oOneProject.IMPORT_ID;
                        string ProgramAreas = "";
                        foreach (var subitem in oOneProject.T_OE_PROJECT_TAGS)
                        {
                            if (subitem.PROJECT_ATTRIBUTE == "Program Area")
                            {
                                if (ProgramAreas == "")
                                {
                                    ProgramAreas = subitem.PROJECT_TAG_NAME;
                                }
                                else
                                {
                                    ProgramAreas = ProgramAreas + "|" + subitem.PROJECT_TAG_NAME;
                                }
                            }
                        }
                        string ProjectFeature = "";
                        foreach (var subfeature in oOneProject.T_OE_PROJECT_TAGS)
                        {
                            if (subfeature.PROJECT_ATTRIBUTE == "Tags")
                            {
                                if (ProjectFeature == "")
                                {
                                    ProjectFeature = subfeature.PROJECT_TAG_NAME;
                                }
                                else
                                {
                                    ProjectFeature = ProjectFeature + "|" + subfeature.PROJECT_TAG_NAME;
                                }
                            }
                        }

                        dtProject.Rows.Add(AgencyName, RecordSource, ProjectName, ProjectDescription, Media, StartYear, ProjectStatus, YearLastUpdated, ProjectURL, MobileTagName,
                             MobileDescription, AdvMonTagName, AdvMonDescription, BPImprovementTagName, BPImprovementDesc, COTS, Vendor, ProjectContact, EECIPProjectID, ImportID,
                             ProgramAreas, ProjectFeature, "", "");

                    }
                }
            }

            //******************************SERVICES******************************
            if (exportdata.Contains("Services"))
            {
                dtServices.Columns.AddRange(new DataColumn[15] {
                                            new DataColumn("Agency Name"),
                                            new DataColumn("Record Source"),
                                            new DataColumn("Enterprise Service Name"),
                                            new DataColumn("Active Interest IND"),
                                            new DataColumn("Project Name"),
                                            new DataColumn("Vendor"),
                                            new DataColumn("Status"),
                                            new DataColumn("Project Contact"),
                                            new DataColumn("Comments"),
                                            new DataColumn("EECIP Ent Service ID"),
                                            new DataColumn("Import ID"),
                                            new DataColumn("Created By"),
                                            new DataColumn("Created Date"),
                                            new DataColumn("Last Modified By"),
                                            new DataColumn("Last Modified Date")
                                           });
                foreach (var oOneOrgName in oOrgList)
                {
                    List<OrganizationEntServicesDisplayType> oServicesList = db_EECIP.GetT_OE_ORGANIZATION_ENT_SVCS_NoLeftJoin(oOneOrgName.ORG_IDX.ConvertOrDefault<Guid>());

                    foreach (var oOneService in oServicesList)
                    {
                        string AgencyName = oOneOrgName.ORG_NAME;
                        string RecordSource = oOneService.RECORD_SOURCE;
                        string EnterpriseServiceName = oOneService.ENT_PLATFORM_NAME;
                        bool ActiveInterestIND = oOneService.ACTIVE_INTEREST_IND;
                        string ProjectName = oOneService.PROJECT_NAME;
                        string Vendor = oOneService.VENDOR;
                        string ProjectStatus = oOneService.IMPLEMENT_STATUS;
                        string ProjectContact = oOneService.PROJECT_CONTACT;
                        string Comments = oOneService.COMMENTS;
                        int? EECIPEntServiceID = oOneService.ORG_ENT_SVCS_IDX;
                        string ImportID = "";
                        int? CreatedBy = oOneService.CREATE_USERIDX;
                        DateTime? CreatedDate = oOneService.CREATE_DT;
                        int? LastModifiedBy = oOneService.MODIFY_USERIDX;
                        DateTime? LastModifiedDate = oOneService.MODIFY_DT;

                        dtServices.Rows.Add(AgencyName, RecordSource, EnterpriseServiceName, ActiveInterestIND, ProjectName, Vendor, ProjectStatus, ProjectContact,
                            Comments, EECIPEntServiceID, ImportID, CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate);

                    }
                }
            }

            //******************************USERS ******************************
            if (exportdata.Contains("Search"))
            {
                dtSearch.Columns.AddRange(new DataColumn[2] {
                                            new DataColumn("Date"),
                                            new DataColumn("Search Term")
                                           });

                List<T_OE_SYS_SEARCH_LOG> _logs = db_Ref.GetT_OE_SYS_SEARCH();
                foreach (var item in _logs)
                    dtSearch.Rows.Add(item.LOG_DT, item.LOG_TERM);
            }


            DataSet dsExport = new DataSet();

            if (dtUsers.Rows.Count > 0)
                dsExport.Tables.Add(dtUsers);
            if (dtAgency.Rows.Count > 0)
                dsExport.Tables.Add(dtAgency);
            if (dtProject.Rows.Count > 0)
                dsExport.Tables.Add(dtProject);
            if (dtServices.Rows.Count > 0)
                dsExport.Tables.Add(dtServices);
            if (dtSearch.Rows.Count > 0)
                dsExport.Tables.Add(dtSearch);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dsExport);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= EECIP_DataExport.xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);

                    Response.Flush();
                    Response.End();
                }
            }
           
            return View();
        }

       
    }
}