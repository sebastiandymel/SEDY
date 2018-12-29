using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;

namespace MvcMusicStore.Controllers
{
//    [SessionState(SessionStateBehavior.Disabled)]
    public class StaticFilesController : Controller
    {
        // GET: /Home/
        [AllowAnonymous]
        public ActionResult Get(string albumId)
        {
            const string requestsKey = "requests";
            if (Session[requestsKey] != null)
                Session[requestsKey] = (int)Session[requestsKey] + 1;
            else
                Session[requestsKey] = 1;
            Console.WriteLine(TempData.Count);
            return new EmptyResult();//Content("~/Images/placeholder.png", "text/plain");
        }
    }
}