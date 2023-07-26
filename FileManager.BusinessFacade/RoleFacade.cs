using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityState = System.Data.Entity.EntityState;

namespace FileManager.BusinessFacade
{
    public partial class Facade
    {
        CodeFirstRoleProvider _roleProvider = new CodeFirstRoleProvider();
        public bool IsUserInRole(string userName,string roleName)
        {
            return _roleProvider.IsUserInRole(userName, roleName);
        }
        public IEnumerable<Role> GetRoles()
        {
            return _db.Roles;
        }
        public IEnumerable<Role> GetRolesExceptAdmin()
        {
            return _db.Roles.Where(x=>!x.RoleName.Equals("admin",StringComparison.OrdinalIgnoreCase)).AsEnumerable();
        }
        public void AddUsersToRoles(string[] Usernames, string[] roles)
        {
            _roleProvider.AddUsersToRoles(Usernames,roles);
        }
        public void RemoveUsersFromRoles(string[] usernames, string[] roles)
        {
            _roleProvider.RemoveUsersFromRoles(usernames, roles);
        }
        public string[] GetRolesForUser(string userName)
        {
            return _roleProvider.GetRolesForUser(userName);
        }
        public Role GetRoleByRoleName(string roleName)
        {
            return _db.Roles.Where(x => x.RoleName == roleName).FirstOrDefault();
        }
        public void CreateRole(string roleName)
        {
            _roleProvider.CreateRole(roleName);
        }
        public bool DeleteRole(string roleName)
        {
            var role = this.GetRoleByRoleName(roleName);
            if (role != null)
            {
                try
                {
                    this.DeleteRolePermissionByRoleId(role.RoleId.ToString());
                    return _roleProvider.DeleteRole(role.RoleName, false);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
                return false;
        }
        public bool UpdateRole(Role role)
        {
            try
            {
                _db.Entry(role).State = (EntityState)System.Data.EntityState.Modified;
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    
    }
}
