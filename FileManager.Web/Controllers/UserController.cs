using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FileManager.Model;
using FileManager.DAL.DataContext;
using FileManager.Web.ViewModel;
using System.Web.Security;
using System.Net.Mail;
using System.Web.Configuration;
using System.Configuration;
using System.Net.Configuration;
using System.Text;
using FileManager.BusinessFacade;

namespace FileManager.Web.Controllers
{
    public class UserController : ApiController
    {
        private FileManagerAuthorizationFacade _fileManagerAuth = new FileManagerAuthorizationFacade();
        private Facade _facade = new Facade();

        public List<UserMenu> GetUserInitialMenuList(string userid)
        {
            List<UserMenu> userMenuList = UserRoleWiseMenu(userid, true);
            return userMenuList;
        }

        // GET api/<controller>/5
        public List<UserMenu> GetLogout(string email)
        {
            WebSecurity.Logout();
            HttpContext.Current.Session.Clear();

            List<UserMenu> userMenuList = UserRoleWiseMenu(email, true);
            return userMenuList;
        }

        // GET api/<controller>/5
        public bool GetIsLoggedIn(int id)
        {
            return _fileManagerAuth.HasSession();
        }

        public string  GetCurrentUserId(string  emailaddress)
        {
            return _fileManagerAuth.GetCurrentUserName();
        }

        public List<UserMenu> GetUserRoleWiseMenu(string  useremail)
        {
            useremail = string.IsNullOrEmpty(useremail) ? this.GetCurrentUserId("") : useremail;
            List<UserMenu> userMenuList = UserRoleWiseMenu(useremail,false);

            return userMenuList;
        }

        private List<UserMenu> UserRoleWiseMenu(string email, bool isLogout)
        {
            List<UserMenu> userMenuList = new List<UserMenu>();

            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            if (!isLogout)
            {
                if (_facade.IsUserInRole(_fileManagerAuth.GetCurrentUserName(), "Admin"))
                {
                    GetAdminUserMenu(userMenuList);
                    return userMenuList;
                }
                else
                {
                    string roleId = _fileManagerAuth.GetCurrentUserRoleId();
                    List<RolePermission> rolePermissionList = _facade.GetRolePermissionByRoleId(roleId).ToList();
                   

                    userMenuList.Add(new UserMenu() { MenuOrder=3, MenuId = "navFavourites", MenuName = "Favourites" });
                    userMenuList.Add(new UserMenu() { MenuOrder = 4, MenuId = "navDownloadBasket", MenuName = "Download Basket" });

                    foreach (RolePermission rolePermission in rolePermissionList)
                    {
                        Permission permission = _facade.GetPermissionById(rolePermission.PermissionId);
                        if (permission != null )
                        {
                            GetRoleBaseUserMenu(userMenuList, rolePermission, permission);
                        }
                    }
                    List<UserMenu> userMenuListSorted = userMenuList.OrderBy(x => x.MenuOrder).ToList();
                    return userMenuListSorted;
                }
            }

            return userMenuList;
        }

        private void GetRoleBaseUserMenu(List<UserMenu> userMenuList, RolePermission rolePermission, Permission permission)
        {
            if (permission.PermissionName == "Resource" && rolePermission.AllowWrite)
            {
                userMenuList.Add(new UserMenu() { MenuOrder = 0, MenuId = "navResource", MenuName = "Resource" });
            }
            if (permission.PermissionName == "Resource" && rolePermission.AllowRead)
            {
                userMenuList.Add(new UserMenu() { MenuOrder = 1, MenuId = "navSearch", MenuName = "Search" });
                userMenuList.Add(new UserMenu() { MenuOrder = 2, MenuId = "navList", MenuName = "List" });
            }
            else if (permission.PermissionName == "CreateRole")
            {
                userMenuList.Add(new UserMenu() { MenuOrder = 7, MenuId = "navCreateRole", MenuName = "Create Role" });
            }
            else if (permission.PermissionName == "Users")
            {
                userMenuList.Add(new UserMenu() { MenuOrder = 6, MenuId = "navUsers", MenuName = "Users" });
            }
            else if (permission.PermissionName == "Category")
            {
                userMenuList.Add(new UserMenu() { MenuOrder = 5, MenuId = "navCategory", MenuName = "Category" });
            }
            else if (permission.PermissionName == "DownloadHistory")
            {
                userMenuList.Add(new UserMenu() { MenuOrder = 8, MenuId = "navHistory", MenuName = "Download History" });
            }
        }

        private void GetAdminUserMenu(List<UserMenu> userMenuList)
        {
            //userMenuList.Add(new UserMenu() { MenuId = "navHome", MenuName = "Home" });
            userMenuList.Add(new UserMenu() { MenuId = "navResource", MenuName = "Resource" });
            userMenuList.Add(new UserMenu() { MenuId = "navSearch", MenuName = "Search" });
            userMenuList.Add(new UserMenu() { MenuId = "navList", MenuName = "List" });
            userMenuList.Add(new UserMenu() { MenuId = "navFavourites", MenuName = "Favourites" });
            userMenuList.Add(new UserMenu() { MenuId = "navDownloadBasket", MenuName = "Download Basket" });
            userMenuList.Add(new UserMenu() { MenuId = "navCategory", MenuName = "Category" });
            userMenuList.Add(new UserMenu() { MenuId = "navUsers", MenuName = "Users" });
            userMenuList.Add(new UserMenu() { MenuId = "navCreateRole", MenuName = "Create Role" });
            userMenuList.Add(new UserMenu() { MenuId = "navHistory", MenuName = "Download History" });
        }


        // POST api/<controller>
        public bool Post(LoginInfo loginInfo)
        {
            return _facade.Login(loginInfo.emailAddress, loginInfo.password);
        }
        
        public bool Post(string email)
        {
            return _facade.RecoverPassword(email);
        }
    }
}