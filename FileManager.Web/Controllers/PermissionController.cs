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
using FileManager.BusinessFacade;

namespace FileManager.Web.Controllers
{
    public class PermissionController : ApiController
    {
        private FileManagerAuthorizationFacade _fileManagerAuth = new FileManagerAuthorizationFacade();
        private Facade _facade = new Facade();

        // GET api/<controller>
        public List<RolePermissionViewModel> Get()
        {
            if (!IsAuthorize("read"))
            {
                return null ;
            }
            List<RolePermissionViewModel> rolePermissionViewModelList = GetRolePermissionViewModel();
            return rolePermissionViewModelList;
        }

        [ActionName("GetAllPermission")]
        public List<PermissionViewModel> GetPermissionType(int permission)
        {
            List<PermissionViewModel> permissionViewModelList = GetPermissionViewModel().ToList<PermissionViewModel>();

            return permissionViewModelList;
        }

        private IEnumerable<PermissionViewModel> GetPermissionViewModel()
        {
            List<PermissionViewModel> permissionViewModelList = new List<PermissionViewModel>();
            PermissionViewModel permissionViewModel = null;
            foreach (Permission item in _facade.GetPermissions())
            {
                permissionViewModel = new PermissionViewModel();
                permissionViewModel.PermissionName = item.PermissionName;
                permissionViewModel.PermissionNameRead = item.PermissionName.Replace(" ", "") + "Read";
                permissionViewModel.PermissionNameWrite = item.PermissionName.Replace(" ", "") + "Write";
                permissionViewModel.PermissionNameDelete = item.PermissionName.Replace(" ", "") + "Delete";

                permissionViewModel.ReadClass = "Enabled";
                permissionViewModel.WriteClass = "Enabled";
                permissionViewModel.DeleteClass = "Enabled";
                if (!item.ReadApplicable)
                {
                    permissionViewModel.ReadClass = "Disabled";
                }
               
                if (!item.WriteApplicable)
                {
                    permissionViewModel.WriteClass = "Disabled";
                }
                if (!item.DeleteApplicable)
                {
                    permissionViewModel.DeleteClass = "Disabled";
                }

                permissionViewModelList.Add(permissionViewModel);
            }

            return permissionViewModelList;
        }

        [ActionName("GetPermissionByRoleName")]
        public RolePermissionModel GetPermissionByRoleName(string roleName)
        {
            if (!IsAuthorize("read"))
            {
                return null;
            }
            RolePermissionModel rolePermissionModel = new RolePermissionModel();
            List<PermissionViewModel> permissionViewModelList = new List<PermissionViewModel>();
            PermissionViewModel permissionViewModel = null;

            if (_fileManagerAuth.HasSession())
            {

                List<Permission> permissionList = _facade.GetPermissions().ToList();

                if (roleName.Length > 0)
                {
                    rolePermissionModel.RoleName = roleName;
                    Role userRole = _facade.GetRoleByRoleName(roleName);
                    string userRoleIdStr = userRole.RoleId.ToString();
                    List<RolePermission> rolePermissionList = _facade.GetRolePermissionByRoleId(userRoleIdStr).ToList();
                    foreach (RolePermission rolePermission in rolePermissionList)
                    {
                        Permission permission = permissionList.Where(x => x.Id == rolePermission.PermissionId).FirstOrDefault();
                        if (permission != null)
                        {
                            permissionViewModel = new PermissionViewModel();
                            permissionViewModel.PermissionName = permission.PermissionName;
                            permissionViewModel.AllowRead = rolePermission.AllowRead;
                            permissionViewModel.AllowWrite = rolePermission.AllowWrite;
                            permissionViewModel.AllowDelete = rolePermission.AllowDelete;
                            permissionViewModel.PermissionNameRead = permission.PermissionName.Replace(" ","") + "Read";
                            permissionViewModel.PermissionNameWrite = permission.PermissionName.Replace(" ", "") + "Write";
                            permissionViewModel.PermissionNameDelete = permission.PermissionName.Replace(" ", "") + "Delete";

                            if (permission.ReadApplicable)
                            {
                                permissionViewModel.ReadClass = rolePermission.AllowRead ? "Checked" : "UnChecked";
                            }
                            else
                            {
                                permissionViewModel.ReadClass = "Disabled";
                            }
                            if (permission.WriteApplicable)
                            {
                                permissionViewModel.WriteClass = rolePermission.AllowWrite ? "Checked" : "UnChecked";
                            }
                            else
                            {
                                permissionViewModel.WriteClass = "Disabled";
                            }
                            if (permission.DeleteApplicable)
                            {
                                permissionViewModel.DeleteClass = rolePermission.AllowDelete ? "Checked" : "UnChecked";
                            }
                            else
                            {
                                permissionViewModel.DeleteClass = "Disabled";
                            }

                            permissionViewModelList.Add(permissionViewModel);
                        }
                    }

                    foreach (Permission item in permissionList)
                    {
                        PermissionViewModel permissionViewModellocal = permissionViewModelList.Where(x => x.PermissionName == item.PermissionName).FirstOrDefault();
                        if (permissionViewModellocal == null)
                        {
                            permissionViewModel = new PermissionViewModel();
                            permissionViewModel.PermissionName = item.PermissionName;
                            permissionViewModel.AllowRead = false;
                            permissionViewModel.AllowWrite = false;
                            permissionViewModel.AllowDelete = false;
                            permissionViewModel.PermissionNameRead = item.PermissionName.Replace(" ", "") + "Read";
                            permissionViewModel.PermissionNameWrite = item.PermissionName.Replace(" ", "") + "Write";
                            permissionViewModel.PermissionNameDelete = item.PermissionName.Replace(" ", "") + "Delete";
                            permissionViewModel.ReadClass = "UnChecked";
                            permissionViewModel.WriteClass ="UnChecked";
                            permissionViewModel.DeleteClass ="UnChecked";
                            permissionViewModelList.Add(permissionViewModel);
                        }
                    }
                }
            }
            
            rolePermissionModel.PermissionViewModelList = permissionViewModelList;
            return rolePermissionModel;
        }

        [ActionName("GetUserPermission")]
        public IEnumerable<PermissionViewModel> GetUserPermission(string origin)
        {
            List<PermissionViewModel> permissionViewModelList = new List<PermissionViewModel>();
            PermissionViewModel permissionViewModel = null;
            Role userRole = null;

            if (_fileManagerAuth.HasSession())
            {
                string useremail = _fileManagerAuth.GetCurrentUserName();
                string[] userRoles = _facade.GetRolesForUser(useremail);
                List<string> detailPermissions = null;
                List<Permission> permissionList = _facade.GetPermissions().ToList();

                string userRoleStr = userRoles[0];
                if (userRoleStr.Length > 0)
                {
                    userRole = _facade.GetRoleByRoleName(userRoleStr);
                    if (userRole.RoleName.Equals("Admin",StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (Permission permission in permissionList)
                        {
                            permissionViewModel = new PermissionViewModel();
                            permissionViewModel.PermissionName = permission.PermissionName;
                            permissionViewModel.AllowRead = true;
                            permissionViewModel.AllowWrite = true;
                            permissionViewModel.AllowDelete = true;
                            permissionViewModelList.Add(permissionViewModel);
                        }
                    }
                    else
                    {
                        if (userRole != null)
                        {
                            if (origin == "resourcedetail")
                            {
                                detailPermissions = new List<string> { "Resource", "Download" };

                            }
                            else if (origin == "category")
                            {
                                detailPermissions = new List<string> { "Category"};
                            }
                            else if (origin == "users")
                            {
                                detailPermissions = new List<string> { "Users" };
                            }
                            else if (origin == "createrole")
                            {
                                detailPermissions = new List<string> { "CreateRole" };
                            }
                            else if (origin == "downloadhistory")
                            {
                                detailPermissions = new List<string> { "DownloadHistory" };
                            }

                            string userRoleIdStr = userRole.RoleId.ToString();
                            List<RolePermission> rolePermissionList = _facade.GetRolePermissionByRoleId(userRoleIdStr).ToList();

                            foreach (var item in detailPermissions)
                            {
                                Permission permission = permissionList.Where(x => x.PermissionName == item).FirstOrDefault();
                                if (permission != null)
                                {
                                    List<RolePermission> rolePermissionListLocal = rolePermissionList.Where(x => x.PermissionId == permission.Id).ToList();
                                    foreach (RolePermission rolePermission in rolePermissionListLocal)
                                    {
                                        permissionViewModel = new PermissionViewModel();
                                        permissionViewModel.PermissionName = permission.PermissionName;
                                        permissionViewModel.AllowRead = rolePermission.AllowRead;
                                        permissionViewModel.AllowWrite = rolePermission.AllowWrite;
                                        permissionViewModel.AllowDelete = rolePermission.AllowDelete;
                                        permissionViewModelList.Add(permissionViewModel);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return permissionViewModelList;
        }
        // POST api/<controller>
        public RolePermissionModel Post(RoleViewModel roleViewModel)
        {
            if (!IsAuthorize("write"))
            {
                return null;
            }

            bool isSuccess = false;
            RolePermissionModel rolePermissionModel = new RolePermissionModel();
            List<RolePermissionViewModel> rolePermissionViewModelList = new List<RolePermissionViewModel>();

            List<Role> allRoles = _facade.GetRoles().ToList();
            List<Permission> permissionList = null;
            Role role = allRoles.Where(x => x.RoleName == roleViewModel.UserRole).FirstOrDefault();
            if (role == null)
            { 
                _facade.CreateRole(roleViewModel.UserRole);

                allRoles = _facade.GetRoles().ToList();
                role = allRoles.Where(x => x.RoleName == roleViewModel.UserRole).FirstOrDefault();

                permissionList = _facade.GetPermissions().ToList();
                if (roleViewModel.Userpermissions != null)
                {
                    foreach (PermissionViewModel item in roleViewModel.Userpermissions)
                    {
                        Permission permission = permissionList.Where(x => x.PermissionName == item.PermissionName).FirstOrDefault();
                        if (permission != null)
                        {
                            _facade.AddRolePermission(new RolePermission() { RoleId = role.RoleId.ToString(), PermissionId = permission.Id, AllowRead = item.AllowRead, AllowWrite = item.AllowWrite, AllowDelete = item.AllowDelete });
                        }
                    }
                    isSuccess = true;
                }
                if (isSuccess)
                {
                    rolePermissionViewModelList = GetRolePermissionViewModel();
                }
            }

            rolePermissionModel.PermissionViewModelList = GetPermissionViewModel().ToList<PermissionViewModel>();
            rolePermissionModel.RolePermissionViewModelList = rolePermissionViewModelList;
            return rolePermissionModel;
        }

        private List<RolePermissionViewModel> GetRolePermissionViewModel()
        {
            List<RolePermissionViewModel> rolePermissionViewModelList = new List<RolePermissionViewModel>();
            List<Role> allRoles = _facade.GetRolesExceptAdmin().ToList();
            List<Permission> permissionList = _facade.GetPermissions().ToList();

            RolePermissionViewModel rolePermissionViewModel = new RolePermissionViewModel();
            

            foreach (Role item in allRoles)
            {
                rolePermissionViewModel = new RolePermissionViewModel();
                rolePermissionViewModel.RoleName = item.RoleName;

                var permissionListLocal = _facade.GetRolePermissionByRoleId(item.RoleId.ToString()).ToList();
                if (permissionListLocal.Count > 0)
                {
                    foreach (RolePermission rolePermission in permissionListLocal)
                    {
                        Permission permission = permissionList.Where(y => y.Id == rolePermission.PermissionId).FirstOrDefault();
                        if (permission != null)
                        {
                            string rwd = "";
                            if (rolePermission.AllowRead && rolePermission.AllowWrite && rolePermission.AllowDelete)
                            {
                                rwd = "(r-w-d)";
                            }
                            else if (rolePermission.AllowRead && rolePermission.AllowWrite && !rolePermission.AllowDelete)
                            {
                                rwd = "(r-w-)";
                            }
                            else if (rolePermission.AllowRead && ! rolePermission.AllowWrite && rolePermission.AllowDelete)
                            {
                                rwd = "(r--d)";
                            }
                            else if (! rolePermission.AllowRead && rolePermission.AllowWrite && rolePermission.AllowDelete)
                            {
                                rwd = "(-w-d)";
                            }
                            else if (rolePermission.AllowRead)
                            {
                                rwd = "(r--)";
                            }
                            else if (rolePermission.AllowWrite)
                            {
                                rwd = "(-w-)";
                            }
                            else if (rolePermission.AllowDelete)
                            {
                                rwd = "(--d)";
                            }
                            rolePermissionViewModel.Permission += permission.PermissionName + rwd + ",";
                        }
                    }
                    rolePermissionViewModel.Permission = rolePermissionViewModel.Permission.Substring(0, rolePermissionViewModel.Permission.Length - 1);
                }
                rolePermissionViewModelList.Add(rolePermissionViewModel);
            }

            return rolePermissionViewModelList;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put(string roleName, RoleViewModel roleViewModel)
        {
            if (!IsAuthorize("write"))
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            if (ModelState.IsValid && roleName == roleViewModel.UserRole && roleName != string.Empty)
            {
                try
                {
                    Role role = _facade.GetRoleByRoleName(roleName);

                    if (role != null)
                    {
                        role.RoleName = roleViewModel.UserRole;
                        _facade.UpdateRole(role);

                        string roleIdStr = role.RoleId.ToString();
                        List<RolePermission> rolePermissionList = _facade.GetRolePermissionByRoleId(roleIdStr).ToList();
                        foreach (RolePermission item in rolePermissionList)
                        {
                            _facade.UpdateRolePermisson(item);
                        }

                        List<Permission> permissionList = _facade.GetPermissions().ToList();
                        foreach (PermissionViewModel item in roleViewModel.Userpermissions)
                        {
                            Permission permission = permissionList.Where(x => x.PermissionName == item.PermissionName).FirstOrDefault();
                            if (permission != null)
                            {
                                _facade.AddRolePermission(new RolePermission() { RoleId = role.RoleId.ToString(), PermissionId = permission.Id, AllowRead = item.AllowRead, AllowWrite = item.AllowWrite, AllowDelete = item.AllowDelete });
                            }
                        }
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(string roleName)
        {
            if (!IsAuthorize("delete"))
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            
            if (_facade.DeleteRole(roleName))
            {
                return Request.CreateResponse(HttpStatusCode.OK, roleName);
            }
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
        private bool IsAuthorize(string permissonType)
        {
            return _fileManagerAuth.IsAuthorize("CreateRole", permissonType);
        }
    }
}