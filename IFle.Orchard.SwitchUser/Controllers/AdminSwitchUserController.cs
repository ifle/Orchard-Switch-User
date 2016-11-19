using System;
using System.Web.Mvc;
using IFle.Orchard.SwitchUser.Services;
using Orchard;
using Orchard.Caching;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.UI.Admin;
using Orchard.Users.Events;

namespace IFle.Orchard.SwitchUser.Controllers
{
    [OrchardFeature("IFle.Orchard.SwitchUser")]
    [Admin]
    public class AdminSwitchUserController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserEventHandler _userEventHandler;
        private IWorkContextAccessor _workContextAccessor;


        public AdminSwitchUserController(
            IAuthorizer authorizer,
            IMembershipService membershipService,
            IAuthenticationService authenticationService,
            IUserEventHandler userEventHandler,
            IWorkContextAccessor workContextAccessor)
        {
            _membershipService = membershipService;
            _authenticationService = authenticationService;
            _userEventHandler = userEventHandler;
            _workContextAccessor = workContextAccessor;
        }
        public ActionResult Index(string userName, string returnUrl = null)
        {
            var workContext = _workContextAccessor.GetContext();
            if (!PermissionService.IsSwitchUserAllowed(workContext))
                return new HttpUnauthorizedResult();

            var user = _membershipService.GetUser(userName);

            if (user == null) return new HttpUnauthorizedResult();
            
            _authenticationService.SignIn(user, false);
            _userEventHandler.LoggedIn(user);

            returnUrl = "~/Admin/";
            //if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/Admin/";
            //if(_workContextAccessor.HttpContext.Request.IsLocal)
            //    return this.RedirectLocal(returnUrl);

            // regirect with new session cookie for init session flag, that allows to user has a user combo
            var redirectId = Guid.NewGuid().ToString();
            workContext.HttpContext.Application.Add(redirectId, "Ok");
            return this.RedirectToAction("CompleteSwitch", new { RedirectId = redirectId, ReturnUrl = returnUrl });
        }

        public ActionResult CompleteSwitch(string redirectId, string returnUrl = null)
        {
            var workContext = _workContextAccessor.GetContext();
            if(workContext.HttpContext.Application[redirectId] == null)
                return new HttpUnauthorizedResult();

            workContext.HttpContext.Application.Remove(redirectId);
            if (workContext.HttpContext.Session != null)
                workContext.HttpContext.Session["SwitchUserAllowed"] = "Ok";

            return this.RedirectLocal(returnUrl);
        }
    }
}