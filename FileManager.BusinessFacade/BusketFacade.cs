using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager.DAL.DataContext;
using FileManager.Model;
namespace FileManager.BusinessFacade
{
    public partial class Facade
    {
        private FileManagerDbContext _db = new FileManagerDbContext();
        private FileManagerAuthorizationFacade _fmAuthorization = new FileManagerAuthorizationFacade();
        public IList<Basket> GetBuskets()
        {
            return _db.Baskets.ToList();
        }
        public bool AddBusket(Basket basket)
        {
            if (basket != null)
            {
                _db.Baskets.Add(basket);
                _db.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }
        }
        
        public IEnumerable<Basket> GetBasketResourceByResourceId(long resourceId)
        {
            return _db.Baskets.Where(x => x.ResourceId == resourceId).ToList();
        }

        public bool RemoveBasketResourceByResourceId(long resourceId)
        {
            _db.Baskets.Where(x => x.ResourceId == resourceId).ToList().ForEach(d => {
            _db.Baskets.Remove(d);
            });
            try
            {
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
        public bool RemoveBasketResourceByBasket(Basket basket)
        {
            try
            {
                _db.Baskets.Remove(basket);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public IEnumerable<Basket> GetBasketsByUserId(Guid userId)
        {
            return _db.Baskets.Where(x=> x.UserId == userId).AsEnumerable();
        }
        public Basket GetBasketByResourceIdAndUserId(long resourceId, Guid userId)
        {
            return _db.Baskets.Where(x => x.ResourceId == resourceId && x.UserId == userId).FirstOrDefault();
        }
        public string DowloadBasketAllFiles()
        {
            Guid userId = _fmAuthorization.GetCurrentUserId();

            List<Basket> basketList = this.GetBasketsByUserId(userId).ToList();

            CopyAndZipBasketFiles(basketList, userId);
            string downloadURL = "";
            if (basketList.Count > 0)
            {
                foreach (Basket item in basketList)
                {

                    this.RemoveBasketResourceByBasket(item);
                    ResourceInfo resourceInfo = this.GetResourceById(item.ResourceId);
                    if (resourceInfo != null)
                    {
                        this.AddDownloadHistory(new Downloadhistory() { ResourceId = resourceInfo.Id, UserId = userId, DownloadDateTime = DateTime.Now });
                        resourceInfo.DownloadCount++;

                        this.UpdateResource(resourceInfo);
                    }
                }

                downloadURL = Path.Combine("/Downloadbasket/" + userId.ToString() + "/", userId.ToString() + ".zip");
            }
            return downloadURL;
        }

        private void CopyAndZipBasketFiles(List<Basket> basketList, Guid userId)
        {
            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Downloadbasket/"), userId.ToString());
            DirectoryInfo d = new DirectoryInfo(path);
            if (!d.Exists)
            {
                d.Create();
            }
            else
            {
                d.Delete(true);
                d.Create();
            }

            ResourceInfo resourceInfo = null;

            foreach (Basket basket in basketList)
            {
                resourceInfo = this.GetResourceById(basket.ResourceId);
                if (resourceInfo != null)
                {
                    string originalFilePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Resources/"), resourceInfo.Id.ToString());

                    string[] fileparts = resourceInfo.ResourceName.ToString().Split('.');
                    string filename = "";
                    if (fileparts.Length == 2)
                    {
                        filename = fileparts[0] + ".zip";
                    }

                    string fileInfoSource = Path.Combine(originalFilePath, filename);

                    DirectoryInfo fileIdDir = new DirectoryInfo(Path.Combine(d.ToString(), resourceInfo.Id.ToString()));
                    if (!fileIdDir.Exists)
                    {
                        fileIdDir.Create();
                    }

                    string newFilePath = Path.Combine(fileIdDir.ToString(), filename);
                    File.Copy(fileInfoSource, newFilePath.ToString());
                }
            }

            string zipFolder = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Downloadbasket/"), userId.ToString() + ".zip");
            CreateZipForBasketFiles(path, zipFolder, userId);
        }
        private void CreateZipForBasketFiles(string path, string zipFolder, Guid userID)
        {
            string folderToZip = path;
            if (!File.Exists(zipFolder))
            {
                ZipFile.CreateFromDirectory(folderToZip, zipFolder);
                File.Move(zipFolder, Path.Combine(path, userID.ToString() + ".zip"));
            }
        }
    }
}
