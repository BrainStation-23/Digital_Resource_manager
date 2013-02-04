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
        public Permission GetPermissionById(int id)
        {
            return _db.Permissions.Where(x => x.Id == id).FirstOrDefault();
        }
        public IEnumerable<Permission> GetPermissions()
        {
            return _db.Permissions.AsEnumerable();
        }
    }
}
