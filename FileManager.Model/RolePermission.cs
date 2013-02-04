using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Model
{
    public class RolePermission
    {
        public long Id
        {
            get;
            set;
        }
        public string RoleId
        {
            get;
            set;
        }

        public int  PermissionId
        {
            get;
            set;
        }

        public bool AllowRead
        {
            get;
            set;
        }

        public bool AllowWrite
        {
            get;
            set;
        }

        public bool AllowDelete
        {
            get;
            set;
        }
    }
}
