using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using WsdlExtractor;

namespace WsdlExtractor.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Inspect(string url)
        {
            return View("Index", Core.GetDataTypes(url));
        }
    }
}