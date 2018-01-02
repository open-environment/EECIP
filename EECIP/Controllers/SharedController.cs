using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EECIP.Models;
using EECIP.App_Logic.DataAccessLayer;

namespace EECIP.Controllers
{
    public class SharedController : Controller
    {
        public ActionResult _PartialHeadNotification()
        {
            var model = new vmSharedNotificationHeader();

            int UserIDX = db_Accounts.GetUserIDX();

            if (UserIDX > 0)
            {
                model.Notifications = db_EECIP.GetT_OE_USER_NOTIFICATION_byUserIDX(UserIDX, true);
                model.NotifyCount = model.Notifications == null ? 0 : model.Notifications.Count;
            }

            return PartialView(model);
        }
    }
}