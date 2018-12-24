using System.Linq;
using System.Web.Mvc;
using MvcMusicStore.Models;
using NLog;

namespace MvcMusicStore.Filters
{
    public class SecurityWarningAttribute : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            using (var storeDb = new MusicStoreEntities())
            {
                var adminRole = storeDb.Roles.FirstOrDefault(a => a.Name == "Administrator");
                var u = storeDb.Users
                        .Where(a => a.EmailConfirmed && a.PhoneNumberConfirmed && a.TwoFactorEnabled)
                        .Where(a => a.AccessFailedCount > 10)
                        .Where(a=>a.Roles.Any(r=>r.RoleId== adminRole.Id))
                        .Where(a => a.UserName == filterContext.RequestContext.HttpContext.User.Identity.Name)
                        .FirstOrDefault()
                    ;
                if (u != null)
                    LogManager.GetCurrentClassLogger().Warn("Security warning for user:'" + u.UserName + "'");
                OnActionExecuting(filterContext);
            }
        }
    }
}