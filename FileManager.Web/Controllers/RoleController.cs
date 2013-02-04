using FileManager.DAL.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FileManager.BusinessFacade;

namespace FileManager.Web.Controllers
{
    public class RoleController : ApiController
    {
        private FileManagerAuthorizationFacade _fileManagerAuth = new FileManagerAuthorizationFacade();
        private Facade _facade = new Facade();
        // GET api/<controller>
        public IEnumerable<Role> Get()
        {
            return _facade.GetRoles();
        }
    }
}