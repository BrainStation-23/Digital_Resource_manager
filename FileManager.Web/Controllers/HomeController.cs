using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FileManager.DAL;
using FileManager.DAL.DataContext;


namespace FileManager.Web.Controllers
{
	public class HomeController : Controller
	{

		public ActionResult Index()
		{
            return View();
		}
	}
}
