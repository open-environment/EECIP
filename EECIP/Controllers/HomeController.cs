using System.Web.Mvc;
using EECIP.Models;
using EECIP.App_Logic.DataAccessLayer;
using EECIP.App_Logic.BusinessLogicLayer;
using System.Web;

namespace EECIP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult TermsAndConditions()
        {
            var model = new vmHomeTerms();
            T_OE_APP_SETTINGS_CUSTOM cust = db_Ref.GetT_OE_APP_SETTING_CUSTOM();
            model.TermsAndConditions = cust.TERMS_AND_CONDITIONS;

            return View(model);
        }

        public ActionResult Unsubscribe(int? ux, string key)
        {
            T_OE_USERS u = db_Accounts.GetT_OE_USERSByIDX(ux ?? -1);
            if (u != null)
            {
                //decrypt oauth string
                string oauthDecode = HttpUtility.UrlDecode(key);
                oauthDecode = oauthDecode.Replace(" ", "+");   //fix situations where spaces and plus get mixed up
                string decryptStr = new SimpleAES().Decrypt(oauthDecode);

                if (decryptStr == u.PWD_HASH)
                {
                    //unsubscribe from newsletter
                    db_Accounts.UpdateT_OE_USERS(u.USER_IDX, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                        null, null, null, null, false, null);

                    TempData["Success"] = "You have successfully unsubscribed.";
                }
                else
                    TempData["Error"] = "Unable to unsubscribe.";

            }
            else
                TempData["Error"] = "Unable to unsubscribe.";

            return View();

        }
    }
}