using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Benchy.Results.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var projects = new Benchy.Results.Models.Projects(@"C:\Users\keza\Dropbox\benchy\benchmarks");
            projects.Load();
            return View(projects);
        }

    }
}
