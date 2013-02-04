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
        public IEnumerable<RolePermission> GetRolePermissionByRoleId(string roleId)
        {
            return _db.RolePermissions.Where(x => x.RoleId == roleId);
        }
        public IEnumerable<RolePermission> GetRolePermissions()
        {
            return _db.RolePermissions.AsEnumerable();
        }
        public bool AddRolePermission(RolePermission rolePermission)
        {
            try
            {
                _db.RolePermissions.Add(rolePermission);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool UpdateRolePermisson(RolePermission rolePermission)
        {
            try
            {
                _db.RolePermissions.Remove(rolePermission);
                _db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        public bool DeleteRolePermissionByRoleId(string roleId)
        {
            try
            {
                List<RolePermission> rolePermissionList = _db.RolePermissions.Where(x => x.RoleId == roleId).ToList();
                foreach (RolePermission item in rolePermissionList)
                {
                    _db.RolePermissions.Remove(item);
                    _db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
