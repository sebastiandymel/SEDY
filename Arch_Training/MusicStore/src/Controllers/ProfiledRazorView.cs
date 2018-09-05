using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace MvcMusicStore.Controllers
{
    public class ProfiledRazorView : RazorView
    {
        public ProfiledRazorView(ControllerContext controllerContext, string viewPath, string layoutPath, bool runViewStartPages, IEnumerable<string> viewStartFileExtensions) : base(controllerContext, viewPath, layoutPath, runViewStartPages, viewStartFileExtensions)
        {
        }

        public ProfiledRazorView(ControllerContext controllerContext, string viewPath, string layoutPath, bool runViewStartPages, IEnumerable<string> viewStartFileExtensions, IViewPageActivator viewPageActivator) : base(controllerContext, viewPath, layoutPath, runViewStartPages, viewStartFileExtensions, viewPageActivator)
        {
        }

        public override void Render(ViewContext viewContext, TextWriter writer)
        {
            using (StackExchange.Profiling.MiniProfiler.StepStatic(this.ViewPath))
            {
                base.Render(viewContext, writer);
            }
        }
    }

    public class ProfiledRazorViewEngine : RazorViewEngine
    {
        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return new ProfiledRazorView(controllerContext, partialPath, null, false, FileExtensions,
                ViewPageActivator);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            var view = new ProfiledRazorView(controllerContext, viewPath, masterPath, true, FileExtensions,
                ViewPageActivator);
            return view;
        }
    }
}