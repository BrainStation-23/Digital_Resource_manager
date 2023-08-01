using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager.DAL.DataContext;
using FileManager.Model;

using System.Runtime.InteropServices;
using System.Net;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Data.SqlClient;
using System.IO.Compression;

namespace FileManager.BusinessFacade
{
    public class ScheduleTask
    {
        Facade _facade = new Facade();
        #region CleanDownloadBusket
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
            var downloadCleanHours = DowloadCleanHours();
            if(downloadCleanHours !=0)
                return (DowloadCleanHours() * 60 * 60);
            return Int32.MaxValue;
            
        }
        private int MaxDownloadMinute()
        {
            var configStringHours = ConfigurationManager.AppSettings.Get("MaxDowloadMinute");
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
        #endregion

        #region BackupTask

        public bool IsEnableResourceBackup()
        {
            var enableDownloadCleanString = ConfigurationManager.AppSettings.Get("EnableResourceBackup");
            if (!string.IsNullOrEmpty(enableDownloadCleanString))
            {

                return enableDownloadCleanString.Equals("True", StringComparison.OrdinalIgnoreCase); ;
            }
            return false;

        }
        public bool IsEnableDbBackup()
        {
            var enableDownloadCleanString = ConfigurationManager.AppSettings.Get("EnableDbBackup");
            if (!string.IsNullOrEmpty(enableDownloadCleanString))
            {

                return enableDownloadCleanString.Equals("True", StringComparison.OrdinalIgnoreCase); ;
            }
            return false;
        }
        public bool IsEnableBackup()
        {
            var enableDownloadCleanString = ConfigurationManager.AppSettings.Get("EnableBackup");
            if (!string.IsNullOrEmpty(enableDownloadCleanString))
            {

                return enableDownloadCleanString.Equals("True", StringComparison.OrdinalIgnoreCase); ;
            }
            return false;
        }
        public int BackupIntervalSecond()
        {
            string backupIntervalHours = ConfigurationManager.AppSettings.Get("BackupIntervalHours");
            if (!string.IsNullOrEmpty(backupIntervalHours))
            {
                var backupInSecond = Int32.Parse(backupIntervalHours);
                backupInSecond = (backupInSecond * 60 * 60);
                return backupInSecond;
            }
            return Int32.MaxValue;
        }

        public void ResouceBackup(string dateTime)
        {
            string destinationServer = ConfigurationManager.AppSettings.Get("RemoteResourceBeckupPC");
            string destinationfolder = ConfigurationManager.AppSettings.Get("ResouceBeckupFolderName");
            string userName = ConfigurationManager.AppSettings.Get("RemoteResourceBeckupPCUserName");
            string password = ConfigurationManager.AppSettings.Get("RemoteResourceBeckupPCPassword");

            string destinationPath = destinationServer + destinationfolder;

            if (IsEnableResourceBackup() && !String.IsNullOrEmpty(destinationServer) && !string.IsNullOrEmpty(destinationfolder))
                BackupResource.CopyToRemoteLocation(@destinationServer, userName, password, @destinationPath,dateTime);
        }

        public void SqlDbBeckup(string dateTime)
        {
            string destinationServer = ConfigurationManager.AppSettings.Get("RemoteDbBeckupPC");
            string dbBackupFolder = ConfigurationManager.AppSettings.Get("DbBeckupFolderName");
            if(IsEnableDbBackup() && !String.IsNullOrEmpty(destinationServer) && !string.IsNullOrEmpty(dbBackupFolder))
                BeckupRestoreDb.TakeBackup(destinationServer, dbBackupFolder,dateTime);
        }
        #endregion

    }
    #region HelperClass
    class BackupResource
    {
        //used in calling WNetAddConnection2[StructLayout (LayoutKind.Sequential)]
        public struct NETRESOURCE
        {
            public int dwScope;
            public int dwType;
            public int dwDisplayType;
            public int dwUsage;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpLocalName;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpRemoteName;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpComment;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpProvider;
        }

        //WIN32API - WNetAddConnection2
        [DllImport("mpr.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int WNetAddConnection2A(
            [MarshalAs(UnmanagedType.LPArray)]NETRESOURCE[] lpNetResource,
            [MarshalAs(UnmanagedType.LPStr)]string lpPassword,
            [MarshalAs(UnmanagedType.LPStr)]string lpUserName,
            int dwFlags);

        //WIN32API - WNetCancelConnection2
        [DllImport("mpr.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]

        private static extern int WNetCancelConnection2A(
            [MarshalAs(UnmanagedType.LPStr)]string lpName, int dwFlags, int fForce);
        public static void CopyToRemoteLocation(string shareServer, string username, string password, string dirTo, string dateTime)
        {
            NETRESOURCE[] nr = new NETRESOURCE[1];
            nr[0].lpRemoteName = shareServer;
            nr[0].lpLocalName = "";

            //mLocalName;nr[0].dwType = 1;
            //disknr[0].dwDisplayType = 0;

            nr[0].dwScope = 0; nr[0].dwUsage = 0;
            nr[0].lpComment = "";
            nr[0].lpProvider = "";
            try
            {
                WNetAddConnection2A(nr, password, username, 0);
                var folderToCopy = Path.Combine(HttpRuntime.AppDomainAppPath, "Resources");
                CopyFolder(folderToCopy, dirTo + "\\Resource" + dateTime);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                WNetCancelConnection2A(shareServer, 0, -1);
            }
        }
        public static void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }
    }
    class BeckupRestoreDb
    {
        public static void TakeBackup(string destinationServer, string dbBackupFolder, string dateTime)
        {
            try
            {
                string connectionstring = ConfigurationManager.ConnectionStrings["FileManagerDbContext"].ConnectionString;
                ServerConnection conn = new ServerConnection();
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionstring);
                conn.LoginSecure = false;
                conn.DatabaseName = builder.InitialCatalog;
                conn.ServerInstance = builder.DataSource;
                conn.Login = builder.UserID;
                conn.Password = builder.Password;

                Server svr = new Server(conn);
                
                Database BuildDB = svr.Databases[builder.InitialCatalog];
                var beckupfolder = Path.Combine(dbBackupFolder, builder.InitialCatalog + dateTime + ".bak");
                    
                string dbbackupfile = @destinationServer +  beckupfolder;


                Backup backup = new Backup();
                backup.Database = builder.InitialCatalog;
                backup.MediaName = "FileSystem";
                BackupDeviceItem bkpDeviceItem = new BackupDeviceItem();
                bkpDeviceItem.DeviceType = DeviceType.File;
                bkpDeviceItem.Name = dbbackupfile;
                backup.Devices.Add(bkpDeviceItem);
                backup.Initialize = true;
                backup.SqlBackup(svr);
            }
            catch (Exception ex)
            {


            }
        }
        /*public void RestoreDb(string backUpFileName,string backupFileFolder)
        {           
 
            string connectionstring = ConfigurationManager.ConnectionStrings["FileManagerDbContext"].ConnectionString;
            ServerConnection conn = new ServerConnection();
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionstring);
            conn.LoginSecure = false;
            conn.DatabaseName = builder.InitialCatalog;
            conn.ServerInstance = builder.DataSource;
            conn.Login = builder.UserID;
            conn.Password = builder.Password;            

            Server svr = new Server(conn);
            Database db = svr.Databases["Master"];

            string dbbackupfile = backupFileFolder + @"\" + backUpFileName + ".bak";
           
            try
            {
                // Restore Database
                Restore restore = new Restore();
                restore.Database = builder.InitialCatalog;
                restore.RestrictedUser = true; 
                restore.Action = RestoreActionType.Database;
                restore.ReplaceDatabase = true;
                restore.Devices.AddDevice(dbbackupfile, DeviceType.File);
                svr.KillAllProcesses(builder.InitialCatalog);
                restore.Wait();  
                restore.SqlRestore(svr);
            }
            catch (Exception ex)
            {
               // "Database restore failed", ex.InnerException.Message
            }
            
        
        }*/
    }
    #endregion
}
