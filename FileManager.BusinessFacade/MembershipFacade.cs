using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager.DAL.DataContext;
using EntityState = System.Data.Entity.EntityState;

namespace FileManager.BusinessFacade
{
    public partial class Facade
    {
        public IEnumerable<User> GetUsersWithRoles()
        {
            return _db.Users.Include(o => o.Roles).AsEnumerable();
        }
        public IEnumerable<User> GetUsers()
        {
            return _db.Users.AsEnumerable();
        }
        public string GetUserNameByUserId(Guid userId)
        {
            return _db.Users.Where(x => x.UserId == userId).FirstOrDefault().Username;
        }
        public Guid GetUserIdByUserName(string userName)
        {
            return _db.Users.Where(x => x.Username.Equals(userName,StringComparison.OrdinalIgnoreCase)).FirstOrDefault().UserId;
        }
        public User GetUserByUserName(string username)
        {
            if (!string.IsNullOrEmpty(username))
                return _db.Users.Where(x => x.Username == username).FirstOrDefault();
            else
                return null;
        }
        public bool UpdateUser(User user)
        {
            _db.Entry(user).State = (EntityState)System.Data.EntityState.Modified;
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
        public bool CreateUser(User user)
        {
            try
            {
                WebSecurity.Register(user.Username, user.Password, user.Email, true, user.Username, user.Username);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DeleteUserByUserName(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                var user = _db.Users.Where(x => x.Username == username).FirstOrDefault();
                if (user == null)
                    return false;
                try
                {
                    _db.Users.Remove(user);
                    _db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
                return false;
        }

    }
}
