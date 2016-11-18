using System.Web.Mvc;
using IFle.Orchard.SwitchUser.Services;
using Orchard;
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
        private WorkContext _workContext;


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
            _workContext = workContextAccessor.GetContext();
        }
        public ActionResult Index(string userName, string returnUrl = null)
        {
            if (!AdminSwitchUserFactory.IsSwitchUserAllowed(_workContext)) return new HttpUnauthorizedResult();

            var user = _membershipService.GetUser(userName);

            if (user == null) return HttpNotFound();

            _authenticationService.SignIn(user, false);
            _userEventHandler.LoggedIn(user);

            returnUrl = "~/Admin/";
            //if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/Admin/";

            return this.RedirectLocal(returnUrl);
        }
    }
}