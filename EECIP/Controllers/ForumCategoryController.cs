using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EECIP.Models;
using EECIP.App_Logic.DataAccessLayer;

namespace EECIP.Controllers
{
    public class ForumCategoryController : Controller
    {
        // GET: ForumCategory
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult ListCategorySideMenu()
        {
            var model = new vmForumAdminCategories
            {
                Categories = db_Forum.GetCategory()
            };
            return PartialView(model);
        }
    }
}