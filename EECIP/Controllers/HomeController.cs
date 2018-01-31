using System.Web.Mvc;
using EECIP.Models;
using EECIP.App_Logic.DataAccessLayer;

namespace EECIP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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
    }
}