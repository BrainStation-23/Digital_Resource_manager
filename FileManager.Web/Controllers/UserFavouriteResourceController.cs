using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FileManager.BusinessFacade;
using FileManager.DAL.DataContext;
using FileManager.Model;
using FileManager.Web.ViewModel;

namespace FileManager.Web.Controllers
{
    public class UserFavouriteResourceController : ApiController
    {
        private FileManagerAuthorizationFacade _fileManagerAuth = new FileManagerAuthorizationFacade();
        private Facade _facade = new Facade();
        public bool Post(long id)
        {
            if (id != null)
            {
                var userId = _fileManagerAuth.GetCurrentUserId();                
                if (!_facade.ExistResourceInUserFavourite(userId,id))
                {
                    var userFavResource = new UserFavouriteResource();
                    userFavResource.ResourceId = id;
                    userFavResource.UserId = userId;
                    return _facade.AddUserFavouriteResource(userFavResource); 
                }
            }
            return false;
        }
        public IEnumerable<FavouriteViewModel> Get()
        {
            var userId = _fileManagerAuth.GetCurrentUserId();
            var resources = _facade.GetUserFavouriteResourceByUserId(userId);
            var favourites = GetFavouriteViewModel(resources);
            return favourites;
        }
        public HttpResponseMessage Delete(int id)
        {
            var userId = _fileManagerAuth.GetCurrentUserId();
            
            if (_facade.RemoveUserFavouriteByResourceIdAndUserId(id,userId))
            {
                return Request.CreateResponse(HttpStatusCode.OK, id.ToString());
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);            
        }


        private IEnumerable<FavouriteViewModel> GetFavouriteViewModel(List<ResourceInfo> resourceInfos)
        {
            List<FavouriteViewModel> _favouriteVM = new List<FavouriteViewModel>();
            foreach (var item in resourceInfos)
            {
                var filePath = "/Resources/" + item.Id + "/" + item.ThumbName;

                string extension = System.IO.Path.GetExtension(item.ResourceName);
                string fileName = item.ResourceName.Substring(0, item.ResourceName.Length - extension.Length);

                var downloadPath = "/Resources/" + item.Id + "/" + fileName + ".zip";
                var resourceZipName = fileName + ".zip";
                var favouriteVM = new FavouriteViewModel()
                {
                    Id = item.Id,
                    Title = item.Title,
                    imgPath = filePath,
                    Download = downloadPath,
                    ResourceZipName = resourceZipName
                };
                _favouriteVM.Add(favouriteVM);
            }
            return _favouriteVM;

        }
    }
}
