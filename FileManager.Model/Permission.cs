using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Model
{
    public class Permission
    {
        public int Id
        {
            get;
            set;
        }
        public string PermissionName
        {
            get;
            set;
        }
        public bool ReadApplicable
        {
            get;
            set;
        }
        public bool WriteApplicable
        {
            get;
            set;
        }
        public bool DeleteApplicable
        {
            get;
            set;
        }
    }
}
