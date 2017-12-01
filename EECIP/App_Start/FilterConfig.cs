using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EECIP.App_Logic.DataAccessLayer;
using EECIP.App_Logic.BusinessLogicLayer;

namespace EECIP
{
    public class FilterConfig
    {        
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }

}
