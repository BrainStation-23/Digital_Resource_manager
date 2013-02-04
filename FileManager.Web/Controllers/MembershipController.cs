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
using System.Data.Entity.Validation;
using FileManager.BusinessFacade;


namespace FileManager.Web.Controllers
{
    public class MembershipController : ApiController
    {
        private FileManagerAuthorizationFacade _fileManagerAuth = new FileManagerAuthorizationFacade();
        private Facade _facade = new Facade();
        // GET api/<controller>
        public List<MembershipViewModel> Get()
        {
            if (!IsAuthorize("read"))
            {
                return null;
            }
            List<MembershipViewModel> membershipViewModelList = new List<MembershipViewModel>();
            MembershipViewModel membershipViewModel = null;
            Guid currentUserId = _fileManagerAuth.GetCurrentUserId();
            var users = _facade.GetUsersWithRoles();
            foreach(User user in users)
            {
                membershipViewModel = new MembershipViewModel();
                membershipViewModel.Username = user.Username;
                membershipViewModel.email = user.Email;
                membershipViewModel.password = "";
                membershipViewModel.role = user.Roles.Count > 0 ? user.Roles.ToList().FirstOrDefault().RoleName : null;

                membershipViewModelList.Add(membershipViewModel);
            }

            return membershipViewModelList;
        }

        // GET api/<controller>/5
        public MembershipViewModel Get(string userName)
        {
            if (!IsAuthorize("read"))
            {
                return null;
            }
            MembershipViewModel membershipViewModel = new MembershipViewModel();
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }
            var user = _facade.GetUserByUserName(userName);
            membershipViewModel.Username = user.Username;
            membershipViewModel.email = user.Email;
            membershipViewModel.role = user.Roles.FirstOrDefault().RoleName;

            return membershipViewModel;
        }

        // POST api/<controller>
        public HttpResponseMessage Post(MembershipViewModel membershipViewModel)
        {
            if (!IsAuthorize("write"))
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            bool isUserCreated = false;
            if (ModelState.IsValid)
            {
                User user = new User();
                user.Username = membershipViewModel.Username;
                user.Password = membershipViewModel.password;
                user.Email = membershipViewModel.email;

                    if (_facade.CreateUser(user))
                    {
                        if (!_facade.IsUserInRole(user.Username, membershipViewModel.role))
                        {
                            _facade.AddUsersToRoles(new string[] { user.Username }, new string[] { membershipViewModel.role });
                        }

                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, membershipViewModel);
                        response.Headers.Location = new Uri(Url.Link("DefaultApi", new
                        {
                            Username = membershipViewModel.Username
                        }));
                        return response;
                    }
             
                else
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put(string userName, MembershipViewModel membershipViewModel)
        {
            if (!IsAuthorize("write"))
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            User user = _facade.GetUserByUserName(userName);
            if (user != null)
            {
                if (ModelState.IsValid && userName == membershipViewModel.Username)
                {
                    
                    var codeFirstProvider = new CodeFirstRoleProvider();
                    _facade.RemoveUsersFromRoles(new string[] { user.Username }, user.Roles.Select(x=>x.RoleName).ToArray());
                    if (!codeFirstProvider.IsUserInRole(user.Username, membershipViewModel.role))
                    {
                        _facade.AddUsersToRoles(new string[] { user.Username }, new string[] { membershipViewModel.role });
                    }
                    if (!String.IsNullOrEmpty(membershipViewModel.password))
                    {
                    user.Password = WebSecurity.GetHash(membershipViewModel.password);
                    user.LastPasswordChangedDate = DateTime.Now;
                    }
                    user.Email = membershipViewModel.email;

                    if(_facade.UpdateUser(user))
                        return Request.CreateResponse(HttpStatusCode.OK);
                    else
                        return Request.CreateResponse(HttpStatusCode.NotFound);                    
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(string userName)
        {
            if (!IsAuthorize("delete"))
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            if(_facade.DeleteUserByUserName(userName))
                return Request.CreateResponse(HttpStatusCode.OK, userName);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound); 
        }
        private bool IsAuthorize(string permissonType)
        {
            return _fileManagerAuth.IsAuthorize("Users", permissonType);
        }
    }
}