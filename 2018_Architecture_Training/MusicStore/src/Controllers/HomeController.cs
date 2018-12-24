using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly MusicStoreEntities _storeContext = new MusicStoreEntities();

        // GET: /Home/
        public async Task<ActionResult> Index()
        {
            using (StackExchange.Profiling.MiniProfiler.StepStatic("ABC"))
            {
                using (StackExchange.Profiling.MiniProfiler.StepStatic("ABC2"))
                {
                    using (StackExchange.Profiling.MiniProfiler.StepStatic("ABC3"))
                    {
                        using (StackExchange.Profiling.MiniProfiler.StepStatic("ABC4"))
                        {

                        }
                    }
                }
            }
            return View(await _storeContext.Albums
                .OrderByDescending(a => a.OrderDetails.Count())
                .Take(6)
                .Include(a => a.Artist)
                .ToListAsync());

 
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _storeContext.Dispose();
            base.Dispose(disposing);
        }
    }
}