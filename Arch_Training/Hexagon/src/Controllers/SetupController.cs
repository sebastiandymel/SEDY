using System.Data.Entity;
using System.Web.Mvc;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    public class SetupController : Controller
    {
        private readonly MusicStoreEntities db = new MusicStoreEntities();

        [HttpGet]
        public ActionResult DatabaseInit()
        {
            Database.SetInitializer<MusicStoreEntities>(null);
            return Content("OK");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}