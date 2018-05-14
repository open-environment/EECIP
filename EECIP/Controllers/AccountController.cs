using EECIP.App_Logic.BusinessLogicLayer;
using EECIP.App_Logic.DataAccessLayer;
using EECIP.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EECIP.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: Account/Login
        public ActionResult Login(string returnUrl)
        {
            //auto pass forward to dashboard if logged in
            T_OE_USERS u = db_Accounts.GetT_OE_USERSByID(User.Identity.Name);
            if (u != null)
            {
                if (u.ACT_IND == true)
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View("Login");
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(vmAccountLogin model, string returnUrl)
        {
            Session.Clear();

            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    T_OE_USERS u = db_Accounts.GetT_OE_USERSByID(model.UserName);
                    if (u.INITAL_PWD_FLAG)
                        return RedirectToAction("SetPermPassword");
                    else
                    {
                        //set last login time and reset failed login attempts
                        db_Accounts.UpdateT_OE_USERS(u.USER_IDX, null, null, null, null, null, null, null, null, System.DateTime.Now, null, null, null, 0, null, null, null, null,false);
                        return RedirectToAction("Index", "Dashboard");
                    }

                }
            }

            // If we got this far, something failed, redisplay form
            TempData["Error"] = "The user name or password provided is incorrect.";
            //ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }



        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // Invalidate the Cache on the Client Side
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            return RedirectToAction("Index", "Home");
        }


        // GET: /Account/Register
        public ActionResult Register()
        {
            T_OE_APP_SETTINGS_CUSTOM cust = db_Ref.GetT_OE_APP_SETTING_CUSTOM();
            var model = new vmAccountRegister();
            model.termsConditions = cust.TERMS_AND_CONDITIONS;
            return View(model);
        }


        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(vmAccountRegister model)
        {
            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    MembershipCreateStatus status;

                    try
                    {
                        // ******************** AGENCY VALIDATION ******************************
                        List<T_OE_ORGANIZATION> o = db_Ref.GetT_OE_ORGANIZATION_ByEmail(model.UserName);
                        if (o != null)
                        {
                            if (o.Count == 0)
                            {
                                // PREVENT REGISTRATION IF NON US/GOV EMAIL
                                if (model.UserName.Substring(model.UserName.Length - 4) != ".gov" && model.UserName.Substring(model.UserName.Length - 3) != ".us")
                                {
                                    TempData["Error"] = "We verify users based on email domain and it appears your domain is not in our system. Please email support@eecip.net and we will get your account set up. Apologies for the delay and thank you for your interest in the Inventory.";
                                    return View(model);
                                }
                                else
                                {
                                    if (model.suggestAgency == null)
                                    {
                                        TempData["Error"] = "No government agency is found matching that email. Please provide an agency name below.";
                                        ModelState.AddModelError("suggestAgency", "Enter your agency name");
                                        model.suggestAgencyInd = true;
                                        return View(model);
                                    }
                                }
                            }
                            else if (o.Count > 1 && model.intSelOrgIDX == null)  //more than one match and single hasn't been identified yet
                            {
                                model.ddl_Agencies = o.Select(x => new SelectListItem
                                {
                                    Value = x.ORG_IDX.ToString(),
                                    Text = x.ORG_NAME
                                });

                                TempData["Error"] = "Select the agency to which you belong.";
                                ModelState.AddModelError("intSelOrgIDX", "Select your agency");
                                return View(model);
                            }
                            else if (o.Count == 1)
                                model.intSelOrgIDX = o.FirstOrDefault().ORG_IDX;
                        }
                        // ****************** END AGENCY VALIDATION ******************************
                        

                        //create user and send out verification email
                        Membership.Provider.CreateUser(model.UserName, "", model.UserName, null, null, false, null, out status);

                        if (status == MembershipCreateStatus.Success)
                        {
                            int UserIDX = (int)Membership.GetUser(model.UserName).ProviderUserKey;

                            //create agency and email rule if new 
                            Guid? NewOrgIDX = null;
                            if (model.intSelOrgIDX == null)
                            {
                                //create the agency & email rule
                                NewOrgIDX = db_Ref.InsertUpdatetT_OE_ORGANIZATION(null, null, model.suggestAgency, null, null, "Tribal", null, null, true, UserIDX);
                                db_Ref.InsertT_OE_ORGANIZATION_EMAIL_RULE(NewOrgIDX.ConvertOrDefault<Guid>(), Regex.Match(model.UserName, "@(.*)").Groups[1].Value);

                                //notify Site Admins via email
                                List<T_OE_USERS> Admins = db_Accounts.GetT_OE_USERSInRole(1);
                                foreach (T_OE_USERS Admin in Admins)
                                    Utils.SendEmail(null, Admin.EMAIL, null, null, model.UserName + " has registered a new Agency", "The user " + model.UserName + " has registered the following new agency: " + model.suggestAgency, null, "", "The user " + model.UserName + " has registered the following new agency: " + model.suggestAgency);
                            }

                            //update first name, last name, and agency
                            db_Accounts.UpdateT_OE_USERS(UserIDX, null, null, model.FirstName, model.LastName, model.UserName, null, null, null, null, null, null, null, null, model.intSelOrgIDX ?? NewOrgIDX, null, null, null,false);

                            //redirect user to registration success view
                            return RedirectToAction("RegisterSuccess", "Account");
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
                    catch (MembershipCreateUserException e)
                    {
                        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                    }
                }
            }

            // Redisplay form showing error or success message
            return View(model);
        }


        // GET: /Account/RegisterSuccess
        public ActionResult RegisterSuccess()
        {
            //TODO lookup user name
            return View();
        }


        [Authorize]
        // GET: /Account/UserProfile/2
        public ActionResult UserProfile(int? id, string a)
        {
            if (id == null)
                id = db_Accounts.GetUserIDX();

            //security validation: only admins can (only allow site admin or user to edit their own profile)
            if (!User.IsInRole("Admins") && a=="a")
                return RedirectToAction("AccessDenied", "Home");

            //security validation (only allow site admin or user to edit their own profile)
            if ((!User.IsInRole("Admins")) && (id != db_Accounts.GetUserIDX()))
                return RedirectToAction("AccessDenied", "Home");
            

            var model = new vmAccountUserProfile();
            T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(id ?? 0);
            if (u != null)
            {
                model.UserIDX = u.USER_IDX;
                model.UserID = u.USER_ID;
                model.FName = u.FNAME;
                model.LName = u.LNAME;
                model.Email = u.EMAIL;
                model.Phone = u.PHONE;
                model.PhoneExt = u.PHONE_EXT;
                model.OrgIDX = u.ORG_IDX;
                model.JobTitle = u.JOB_TITLE;
                model.LinkedIn = u.LINKEDIN;
                model.NodeAdmin = u.NODE_ADMIN;
                model.HasAvatar = (u.USER_AVATAR != null);
                model.ImageUniqueStr = (u.MODIFY_DT ?? u.CREATE_DT).ConvertOrDefault<DateTime>().Ticks.ToString();
                model.ActInd = u.ACT_IND;
                model.uListInd = a;
                model.ExcludeBadges = u.EXCLUDE_POINTS_IND;


                //expertise
                model.SelectedExpertise = db_EECIP.GetT_OE_USER_EXPERTISE_ByUserIDX(id ?? 0);
                model.AllExpertise = db_EECIP.GetT_OE_USER_EXPERTISE_ByUserIDX_All(id ?? 0).Select(x => new SelectListItem { Value = x, Text = x });

                //org name
                T_OE_ORGANIZATION org = db_Ref.GetT_OE_ORGANIZATION_ByID(model.OrgIDX.ConvertOrDefault<Guid>());
                if (org != null)
                    model.OrgName = org.ORG_NAME;
            }

            return View(model);
        }


        // POST: /Account/UserProfile
        [Authorize]
        [HttpPost]
        public ActionResult UserProfile(vmAccountUserProfile model)
        {
            //security validation (only allow site admin or user to edit their own profile)
            if ((!User.IsInRole("Admins")) && (model.UserIDX != db_Accounts.GetUserIDX()))
                return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                if (model.UserIDX > 0)
                {
                    var strippedPhone = Regex.Replace(model.Phone ?? "", "[^0-9]", "");
                    if ((model.LinkedIn ?? "").Contains("www"))
                    {
                        try {
                            Uri uri = new Uri(model.LinkedIn);
                            model.LinkedIn = uri.Segments.Last();
                        } catch { }
                    }

                    int SuccID = db_Accounts.UpdateT_OE_USERS(model.UserIDX, null, null, model.FName, model.LName, model.Email, model.ActInd, null, null, null, strippedPhone, model.PhoneExt, null, null, model.OrgIDX, model.JobTitle, model.LinkedIn, model.NodeAdmin, model.ExcludeBadges);

                    //update user expertise
                    db_EECIP.DeleteT_OE_USER_EXPERTISE(model.UserIDX);
                    foreach (string expertise in model.SelectedExpertise ?? new List<string>())
                    {
                        db_EECIP.InsertT_OE_USER_EXPERTISE(model.UserIDX, expertise);
                    }

                    //award profile badge
                    if (db_Accounts.GetUserIDX() == model.UserIDX)
                        db_Forum.EarnBadgeController(model.UserIDX, "UserProfile");

                    ////avatar handling
                    if (model.imageBrowes != null)
                    {
                        // ******************** VALIDATION START ********************************
                        //File too big check
                        if (model.imageBrowes.ContentLength > 10485760)
                        {
                            TempData["Error"] = "File cannot exceed 10MB";
                            return RedirectToAction("UserProfile", new { a = model.uListInd });
                        }

                        //invalid file extension check
                        var fileExtension = Path.GetExtension(model.imageBrowes.FileName);
                        List<string> allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".bmp" };
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            TempData["Error"] = "Invalid file type";
                            return RedirectToAction("UserProfile", new { a = model.uListInd });
                        }
                        // ******************** VALIDATION END ********************************

                        // Convert to Png
                        var outputStream = model.imageBrowes.InputStream.ConvertImage(ImageFormat.Png);

                        //save to db
                        db_Accounts.UpdateT_OE_USERS_Avatar(model.UserIDX, Utils.ConvertGenericStreamToByteArray(outputStream));

                        //save to file system
                        string fileName1 = model.UserIDX.ToString() + ".png";
                        model.imageBrowes.SaveAs(Server.MapPath("/Content/Images/Users/" + fileName1));

                        //award badge
                        if (db_Accounts.GetUserIDX() == model.UserIDX)
                            db_Forum.EarnBadgeController(model.UserIDX, "Photogenic");

                    }

                    //update azure search
                    AzureSearch.PopulateSearchIndexUsers(model.UserIDX);

                    if (SuccID > 0)
                        TempData["Success"] = "Update successful.";
                    else
                        TempData["Error"] = "Error updating data.";
                }

            }

            return RedirectToAction("UserProfile", new { a = model.uListInd });
        }


        public ActionResult ResetPassword()
        {
            return View();
        }


        //POST: /Account/ResetPassword
        [HttpPost]
        public ActionResult ResetPassword(vmAccountLostPassword model)
        {
            System.Threading.Thread.Sleep(10000);

            if (ModelState.IsValid)
            {
                MembershipUser u = Membership.GetUser(model.Email, false);
                if (u != null)
                {
                    string SuccInd = u.ResetPassword();
                    TempData[SuccInd.Substring(0, 5) != "Error" ? "Success" : "Error"] = SuccInd;
                    return View();
                }
            }

            //if got this far, error
            TempData["Error"] = "Error resetting password.";
            return View();
        }


        [Authorize]
        public ActionResult SetPermPassword()
        {
            var model = new vmAccountChangePassword();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SetPermPassword(vmAccountChangePassword model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(db_Accounts.GetUserIDX());
                    if (u != null)
                    {
                        if (Membership.ValidateUser(u.USER_ID, model.OldPassword) == true)
                        {
                            if (Membership.Provider.ChangePassword(u.USER_ID, model.OldPassword, model.Password))
                            {
                                FormsAuthentication.SetAuthCookie(u.USER_ID, true);
                                return RedirectToAction("Index", "Dashboard");
                            }
                        }
                    }
                }
                catch { }

            }

            //if got this far, failed
            TempData["Error"] = "Change password failed.";
            return View(model);
        }



        // GET: /Account/Verify     USED TO SET PERMANENT PASSWORD
        public ActionResult Verify(string oauthcrd)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //decrypt oauth string
                    string oauthDecode = HttpUtility.UrlDecode(oauthcrd);
                    oauthDecode = oauthDecode.Replace(" ", "+");   //fix situations where spaces and plus get mixed up
                    string decryptStr = new SimpleAES().Decrypt(oauthDecode);

                    //split into password and username
                    string[] s = Regex.Split(decryptStr, "\\|\\|");

                    if (Membership.ValidateUser(s[1], s[0]) == false)
                        TempData["Error"] = "Verification failed.";
                }
                catch
                {
                    TempData["Error"] = "Verification failed.";
                }
            }

            var model = new vmAccountVerify
            {
                OAuth = oauthcrd
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult Verify(vmAccountVerify model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //decrypt oauth string
                    string oauthDecode = HttpUtility.UrlDecode(model.OAuth);
                    oauthDecode = oauthDecode.Replace(" ", "+");   //fix situations where spaces and plus get mixed up
                    string decryptStr = new SimpleAES().Decrypt(oauthDecode);

                    //split into password and username
                    string[] s = Regex.Split(decryptStr, "\\|\\|");

                    if (Membership.ValidateUser(s[1], s[0]) == true)
                    {
                        if (Membership.Provider.ChangePassword(s[1], s[0], model.Password))
                        {
                            FormsAuthentication.SetAuthCookie(s[1], true);
                            return RedirectToAction("Index", "Dashboard");
                        }
                    }
                }
                catch { }

                TempData["Error"] = "Change password failed.";

            }
            else
                TempData["Error"] = "Change password failed.";

            return View(model);
        }

        [Authorize]
        public ActionResult Notifications()
        {
            int UserIDX = db_Accounts.GetUserIDX();
            var model = new vmAccountNotifications();
            model.notifications = db_EECIP.GetT_OE_USER_NOTIFICATION_byUserIDX(UserIDX, false);
            return View(model);  
        }


        [Authorize]
        public ActionResult NotificationDelete2(Guid? id)
        {

            int UserIDX = db_Accounts.GetUserIDX();

            //CHECK PERMISSIONS
            T_OE_USER_NOTIFICATION n = db_EECIP.GetT_OE_USER_NOTIFICATION_byID(id);
            if (n != null)
            {
                if (User.IsInRole("Admins") || UserIDX == n.USER_IDX)
                {
                    int SuccID = db_EECIP.DeleteT_OE_NOTIFICATION(id.ConvertOrDefault<Guid>());
                    if (SuccID > 0)
                        TempData["Success"] = "Deleted";
                    else
                        TempData["Error"] = "Unable to delete notification";
                }
            }

            return RedirectToAction("Notifications", "Account");
        }


        // POST: /Forum/PostAnswer
        [Authorize]
        [HttpPost]
        public JsonResult NotificationRead(Guid? id)
        {
            int UserIDX = db_Accounts.GetUserIDX();

            //CHECK PERMISSIONS
            T_OE_USER_NOTIFICATION n = db_EECIP.GetT_OE_USER_NOTIFICATION_byID(id);
            if (n != null)
            {
                if (User.IsInRole("Admins") || UserIDX == n.USER_IDX)
                {
                    Guid? SuccID = db_EECIP.InsertUpdateT_OE_USER_NOTIFICATION(n.NOTIFICATION_IDX, n.USER_IDX, null, null, null, null, true, null, true, null, false);
                    if (SuccID != null)
                        return Json(new { msg = "Success" });
                }
            }

            //return ERROR
            return Json(new { msg = "Unable to mark read." });

        }

        
        #region Helpers
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

    }
}