using System.Collections.Generic;
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
            return View(await _storeContext.Albums
                .OrderByDescending(a => a.OrderDetails.Count())
                .Take(6)
                .Include(x=> x.Artist)
                .ToListAsync());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _storeContext.Dispose();
            base.Dispose(disposing);
        }
    }

    public interface IMostPopularAlbumsPrimaryPort
    {
        IList<ApplicationAlbum> GetMostPopular(int count);
    }

    public class MostPopularAlbumsPrimaryPort
    {
        private MostPopularAlbumsApplicationService appService;
        private readonly IOrmSecondaryPort secondaryPort;
        private readonly ILoggerPort logger;

        public MostPopularAlbumsPrimaryPort(
            IOrmSecondaryPort secondaryPort, 
            ILoggerPort logger)
        {
            this.secondaryPort = secondaryPort;
            this.logger = logger;
            this.appService = new MostPopularAlbumsApplicationService( new AlbumsSearchService(secondaryPort), logger);
        }

        public IList<ApplicationAlbum> GetMostPopular(int count)
        {
            return this.appService.GetMostPopular(count);
        }
    }

    internal class MostPopularAlbumsApplicationService
    {
        private AlbumDomainService domainService;
        public MostPopularAlbumsApplicationService(
            AlbumsSearchService searchService,
            ILoggerPort logger)
        {
            this.domainService = new AlbumDomainService(searchService);
        }

        public IList<ApplicationAlbum> GetMostPopular(int count)
        {
            var ret = this.domainService.GetMostPopularAlbums(count);

            return MapToApplicationCore(ret);
        }

        private IList<ApplicationAlbum> MapToApplicationCore(List<DomainAlbum> ret)
        {
            return new List<ApplicationAlbum>();
        }
    }

    internal class AlbumsSearchService
    {
        private readonly IOrmSecondaryPort orm;

        public AlbumsSearchService(IOrmSecondaryPort orm)
        {
            this.orm = orm;
        }

        public IList<ApplicationAlbum> GetMostPopularAlbums()
        {
            var ret = this.orm.GetAlbums();

            return Map(ret);
        }

        private IList<ApplicationAlbum> Map(IList<Album> ret)
        {
            return new List<ApplicationAlbum>();
        }
    }

    internal class AlbumDomainService
    {
        private readonly AlbumsSearchService searchService;

        public AlbumDomainService(AlbumsSearchService searchService)
        {
            this.searchService = searchService;
        }

        public List<DomainAlbum> GetMostPopularAlbums(int count)
        {
            var ret = this.searchService.GetMostPopularAlbums();
            return MapApp(ret);
        }

        private List<DomainAlbum> MapApp(IList<ApplicationAlbum> ret)
        {
            return new List<DomainAlbum>();
        }
    }

    internal class DomainAlbum
    {
    }

    public interface ILoggerPort
    {
    }

    public interface IOrmSecondaryPort
    {
        IList<Album> GetAlbums();
    }

    public class ApplicationAlbum
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ArtistName { get; set; }
    }
}