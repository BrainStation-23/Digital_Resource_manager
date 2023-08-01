using System;
using System.Collections.Generic;
using System.Linq;
using FileManager.DAL;
using FileManager.DAL.DataContext;
using Microsoft.AspNetCore.Mvc;


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
