using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager.Model;
using System.IO.Compression;
using System.Net.Http;

namespace FileManager.BusinessFacade
{
    public partial class Facade
    {
        public IEnumerable<ResourceInfo> GetResources()
        {
            return _db.ResourceInfos.ToList();
        }
        public ResourceInfo GetResourceById(long id)
        {
            return _db.ResourceInfos.Where(r => r.Id == id).FirstOrDefault();
        }
        public List<ResourceInfo> GetSearchResults(string searchText)
        {

            if (string.IsNullOrEmpty(searchText))
                return this.GetResources().ToList();

            var searchResults = (from res in _db.ResourceInfos
                                 from tag in res.Tags
                                 where (res.Title.Contains(searchText) || res.Description.Contains(searchText) || tag.Name.Contains(searchText))
                                 select res).Distinct().ToList();

            if (searchResults.Count() > 0)
                return searchResults;
            else
                return null;
        }
        public ResourceInfo CreateResource(ResourceInfo resourceInfo)
        {
            _db.ResourceInfos.Add(resourceInfo);
            try
            {
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return resourceInfo;
        }
        public bool UpdateResource(ResourceInfo resourceInfo)
        {
            _db.Entry(resourceInfo).State = System.Data.EntityState.Modified;
            try
            {
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void CreateResZipFile(string path, string resName)
        {
            var folderToZip = path;
            string extension = System.IO.Path.GetExtension(resName);
            string fileName = resName.Substring(0, resName.Length - extension.Length);
            var zipFolder = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Resources/"), fileName + ".zip");
            if (!File.Exists(zipFolder))
            {
                ZipFile.CreateFromDirectory(folderToZip, zipFolder);
                File.Delete(Path.Combine(path, resName));
                File.Move(zipFolder, Path.Combine(path, fileName + ".zip"));
            }
        }
        public string MoveResources(FileInfo fileInfo, FileInfo thumbFileInfo, ResourceInfo resourceInfo)
        {
            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Resources/"), resourceInfo.Id.ToString());
            DirectoryInfo d = new DirectoryInfo(path);
            d.Create();
            var newFilePath = Path.Combine(d.ToString(), resourceInfo.ResourceName.ToString());
            var newThumbFilePath = Path.Combine(d.ToString(), resourceInfo.ThumbName);
            File.Move(fileInfo.FullName, newFilePath.ToString());
            File.Move(thumbFileInfo.FullName, newThumbFilePath.ToString());
            return path;
        }
        public bool PostRating(int rating, int fileId)
        {
            if (fileId < 0)
            {
                return false;
            }

            var resourceInfo = this.GetResourceById(fileId);
            var oldUserRating = resourceInfo.UserRating;
            var oldUserCount = resourceInfo.RatingCount;

            if (resourceInfo.RatingCount == 0)
            {
                resourceInfo.RatingCount += 1;
                resourceInfo.UserRating = rating;
            }
            else
            {
                var newRating = oldUserRating + rating;
                resourceInfo.RatingCount += 1;
                resourceInfo.UserRating = newRating;
            }
            _db.SaveChanges();
            return true;
        }
        public bool DeleteZipFile(string path, string resName)
        {
            var zipFolder = GetZipFilePath(path, resName);
            if (File.Exists(zipFolder))
            {
                File.Delete(zipFolder);
                return true;
            }
            return false;
        }

        private string GetZipFilePath(string path, string resName)
        {
            string extension = System.IO.Path.GetExtension(resName);
            string fileName = resName.Substring(0, resName.Length - extension.Length);
            var zipFolder = Path.Combine(path, fileName + ".zip");
            return zipFolder;
        }
        public bool UnZipResourceFile(string path, string resName)
        {
            string unzipPath = Path.Combine(path , "temp");
            DirectoryInfo dir = new DirectoryInfo(unzipPath);
            dir.Create();
            string oldZipFilePath = GetZipFilePath(path,resName);
            ZipFile.ExtractToDirectory(oldZipFilePath, dir.FullName);
            string unzipResourcePath = Path.Combine(unzipPath,resName);
            string resourcePath = Path.Combine(path,resName);
            if (File.Exists(unzipResourcePath))
            {
                File.Move(unzipResourcePath, resourcePath);
                dir.Delete(true);
                return true;
            }
            return false;
        }
        public bool DeleteFile(string path, string resName)
        {
            var fullFilePath = Path.Combine(path, resName);
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
                return true;
            }
            return false;
        }
        public bool DeleteFile(string fileFullPath)
        {
            if (File.Exists(fileFullPath))
            {
                File.Delete(fileFullPath);
                return true;
            }
            return false;
        }
        public ResourceInfo GetResourceInfoFromFormProvider(MultipartFormDataStreamProvider provider)
        {
            var resourceInfo = new ResourceInfo();
            resourceInfo.Title = !string.IsNullOrEmpty(provider.FormData.Get("Title")) ? string.Concat(provider.FormData.Get("Title")) : "";
            resourceInfo.Description = !string.IsNullOrEmpty(provider.FormData.Get("Description")) ? string.Concat(provider.FormData.Get("Description")) : "";
            resourceInfo.CategoryId = !string.IsNullOrEmpty(provider.FormData.Get("CategoryId")) ? Convert.ToInt32(provider.FormData.Get("CategoryId")) : 0;
            resourceInfo.Rank = !string.IsNullOrEmpty(provider.FormData.Get("Rank")) ? string.Concat(provider.FormData.Get("Rank")) : "";
            string[] tags = provider.FormData.Get("Tags").Split(',');
            resourceInfo.Tags = new List<Tag>();
            foreach (var tag in tags)
            {
                if (tag != "")
                {
                    var _tag = this.GetTagByTagName(tag);
                    if (_tag == null)
                    {
                        resourceInfo.Tags.Add(new Tag()
                        {
                            Name = tag
                        });
                    }
                    else
                    {
                        resourceInfo.Tags.Add(_tag);
                    }
                }
            }
            return resourceInfo;
        }

        public bool AddRating(int rating, int fileId)
        {
            var resourceInfo = this.GetResourceById(fileId);//_db.ResourceInfos.FirstOrDefault(x => x.Id == fileId);
			var oldUserRating = resourceInfo.UserRating;
			var oldUserCount = resourceInfo.RatingCount;

			if(resourceInfo.RatingCount == 0)
			{
				resourceInfo.RatingCount += 1;
				resourceInfo.UserRating = rating;
			}
			else
			{
				//var newRating = ((oldUserRating * oldUserCount) + rating) / (oldUserCount + 1);
                var newRating = oldUserRating + rating;
				resourceInfo.RatingCount += 1;
				resourceInfo.UserRating = newRating;
			}
            try{
			_db.SaveChanges();
                return true;
            }
            catch{
                return false;
            }
			
        }

        public bool DeleteResource(long id)
        {
            
           // List<UserFavouriteResource> userFavouriteResources = _db.UserFavouriteResources.Where(x => x.ResourceId == id).ToList();
            var resources = this.GetResourceById(id);

           // List<Downloadhistory> downloadhistoryList = _db.Downloadhistories.Where(x => x.ResourceId == id).ToList();
            //List<Basket> basketList = _db.Baskets.Where(x => x.ResourceId == id).ToList();

            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Resources/"), id.ToString());
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            if (resources == null || !directoryInfo.Exists)
            {
                return false;
            }         
            
            try
            {
                this.RemoveUserFavouriteResourcesByResourceId(id);
                this.RemoveDownloadHistoriesByResourceId(id);
                this.RemoveBasketResourceByResourceId(id);
                _db.ResourceInfos.Remove(resources);

                _db.SaveChanges();
                directoryInfo.Delete(true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public IEnumerable<ResourceInfo> GetResourceByCategoryId(int id)
        {
            return _db.ResourceInfos.Where(r => r.CategoryId == id).AsEnumerable();
        }
    }
}
