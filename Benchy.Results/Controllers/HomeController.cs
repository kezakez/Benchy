using System.Web.Configuration;
using System.Web.Mvc;

namespace Benchy.Results.Controllers
{
    public class HomeController : Controller
    {
        private Models.Projects projects;
        public HomeController()
        {
            string dir = WebConfigurationManager.AppSettings["benchmarkDirectory"];
            projects = new Models.Projects(dir);
            projects.Load(); 
        }

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View(projects);
        }

        public ActionResult Details(string projId, string testId)
        {
            var test = projects.FindItem(projId, testId);
            return View(test);
        }
    }
}
