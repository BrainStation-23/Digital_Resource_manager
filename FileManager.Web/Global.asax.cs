using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FileManager.DAL;
using System.Web.SessionState;
using System.Web.Caching;
using FileManager.BusinessFacade;

namespace FileManager.Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			System.Data.Entity.Database.SetInitializer(new FileManager.DAL.DataContext.FileManagerContextInitializer());
            if(new ScheduleTask().IsEnableDownloadClean())
                AddTask("DeleteZipFile", new ScheduleTask().DowloadCleanInSecond());            
            

		}

        protected void Application_PostAuthorizeRequest()
        {
            //if (IsWebApiRequest())
            //{
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            //}
        }

        //private static bool IsWebApiRequest()
        //{
        //    return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(_WebApiExecutionPath);
        //}
        private void AddTask(string name, int seconds)
        {
            OnCacheRemove = new CacheItemRemovedCallback(CacheItemRemoved);
            HttpRuntime.Cache.Insert(name, seconds, null,
                DateTime.Now.AddSeconds(seconds), Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable, OnCacheRemove);
        }

        public void CacheItemRemoved(string k, object v, CacheItemRemovedReason r)
        {

            new ScheduleTask().CleanDowloadedFile();
            AddTask(k, Convert.ToInt32(v));
        }
        private static CacheItemRemovedCallback OnCacheRemove = null;
	}
}