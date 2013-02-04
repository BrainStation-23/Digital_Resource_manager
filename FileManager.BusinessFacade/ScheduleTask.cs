using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager.DAL.DataContext;
using FileManager.Model;

namespace FileManager.BusinessFacade
{
    public class ScheduleTask
    {
        Facade _facade = new Facade();

        public void CleanDowloadedFile()
        {
            if (IsEnableDownloadClean())
            {
                var timetoExecute = DowloadCleanHours();
                var dowloadLimitTime = DateTime.Now.AddMinutes(-MaxDownloadMinute());
                DateTime dateTimeToExucute = DateTime.Now.AddHours(-timetoExecute);
                var distictdowloadedItem = _facade.GetDistinctDowloadHistoryByUser();
                if (distictdowloadedItem.Count > 0)
                {
                    foreach (var history in distictdowloadedItem)
                    {
                        if (history.DownloadDateTime < dowloadLimitTime)
                        {
                            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Downloadbasket/"), history.UserId.ToString());

                            DirectoryInfo d = new DirectoryInfo(path);

                            if (d.Exists)
                            {
                                d.Delete(true);
                            }
                        }
                    }
                }
            }
        }
        private int DowloadCleanHours()
        {
            var configStringHours = ConfigurationManager.AppSettings.Get("DownloadCleanIntervalHours");
            if (!string.IsNullOrEmpty(configStringHours))
            {
                var cleanHours = Convert.ToInt32(configStringHours);
                return cleanHours;
            }
            return 0;
        }
        public int DowloadCleanInSecond()
        {
            var cleanInSecond = (DowloadCleanHours() * 60 * 60);
            return cleanInSecond;
        }
        private int MaxDownloadMinute()
        {
            var configStringHours = ConfigurationManager.AppSettings.Get("MaxDowloadMin");
            if (!string.IsNullOrEmpty(configStringHours))
            {
                var maxMinute = Convert.ToInt32(configStringHours);
                return maxMinute;
            }
            return 0;
            
        }
        public bool IsEnableDownloadClean()
        {
            var enableDownloadCleanString = ConfigurationManager.AppSettings.Get("EnableDownloadClean");
            if (!string.IsNullOrEmpty(enableDownloadCleanString))
            {

                return enableDownloadCleanString.Equals("True", StringComparison.OrdinalIgnoreCase); ;
            }
            return false;
            
        }
        
    }
}
