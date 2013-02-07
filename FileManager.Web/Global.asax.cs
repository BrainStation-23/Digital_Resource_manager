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
using System.Threading;

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

            AddTask("DeleteZipFile", new ScheduleTask().DowloadCleanInSecond());
            BackupTask("BackupResourceAndDb", 20);//new ScheduleTask().BackupIntervalSecond());
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
            if (new ScheduleTask().IsEnableDownloadClean())
            {
                Thread t = new Thread(new ThreadStart(new ScheduleTask().CleanDowloadedFile));
                t.Start();
            }
            AddTask(k, Convert.ToInt32(v));
        }
        private static CacheItemRemovedCallback OnCacheRemove = null;

        private void BackupTask(string name, int seconds)
        {
            OnBackupCacheRemove = new CacheItemRemovedCallback(BackupCacheItemRemoved);
            HttpRuntime.Cache.Insert(name, seconds, null,
                DateTime.Now.AddSeconds(seconds), Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable, OnBackupCacheRemove);
        }

        public void BackupCacheItemRemoved(string k, object v, CacheItemRemovedReason r)
        {
            if (new ScheduleTask().IsEnableBackup())
            {
                string dateTime = DateTime.Now.ToString("-yyyy-MM-dd-HH-mm-ss");
                if (new ScheduleTask().IsEnableResourceBackup())
                {
                    Thread t = new Thread(() => new ScheduleTask().ResouceBackup(dateTime));
                    t.Start();
                }
                if (new ScheduleTask().IsEnableDbBackup())
                {
                    Thread t = new Thread(() => new ScheduleTask().SqlDbBeckup(dateTime));
                    t.Start();
                }
            }
            BackupTask(k, Convert.ToInt32(v));
        }
        private static CacheItemRemovedCallback OnBackupCacheRemove = null;
        
	}
}