using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager.Model;

namespace FileManager.BusinessFacade
{
    public partial class Facade
    {

        public IEnumerable<UserFavouriteResource> GetUserFavouriteResources()
        {
            return _db.UserFavouriteResources.ToList();
        }
        public UserFavouriteResource GetSingleUserFavouriteResourceByResourceIdAndUserId(long resourceId, Guid userId)
        {
            return _db.UserFavouriteResources.Where(x => x.ResourceId == resourceId && x.UserId == userId).FirstOrDefault();
        }
        public IEnumerable<UserFavouriteResource> GetUserFavouriteResourcesByResourceId(long uesourceId)
        {
            return _db.UserFavouriteResources.Where(x => x.ResourceId == uesourceId).ToList();
        }
        public bool RemoveUserFavouriteResourcesByResourceId(long resourceId)
        {
            _db.UserFavouriteResources.Where(x => x.ResourceId == resourceId).ToList().ForEach(d =>
            {
                _db.UserFavouriteResources.Remove(d);
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
        public bool IsInFavouriteList(long resourceId, Guid userId)
        {
            bool isInFafouriteList = false;
            UserFavouriteResource UserFavouriteResource = _db.UserFavouriteResources.Where(x => x.ResourceId == resourceId && x.UserId == userId).FirstOrDefault();
            if (UserFavouriteResource != null)
            {
                isInFafouriteList = true;
            }

            return isInFafouriteList;
        }

        public bool AddUserFavouriteResource(UserFavouriteResource userFavouriteResource)
        {
            try
            {
                _db.UserFavouriteResources.Add(userFavouriteResource);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ExistResourceInUserFavourite(Guid userId,long id)
        {
            return _db.UserFavouriteResources.Where(x => x.UserId == userId && x.ResourceId == id).FirstOrDefault() != null;
        }
        public List<ResourceInfo> GetUserFavouriteResourceByUserId(Guid userId)
        {
            var resources = (from usrfvt in _db.UserFavouriteResources
                             join rsc in _db.ResourceInfos on usrfvt.ResourceId equals rsc.Id
                             where usrfvt.UserId == userId
                             select rsc).ToList();
            return resources;
        }
        public bool RemoveUserFavouriteByResourceIdAndUserId(long id, Guid userId)
        {
            var UserFavouriteResource = _db.UserFavouriteResources.Where(x => x.UserId == userId && x.ResourceId == id).FirstOrDefault();
            if (UserFavouriteResource != null)
            {
                _db.UserFavouriteResources.Remove(UserFavouriteResource);
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
            return false;
        }
    }
}
