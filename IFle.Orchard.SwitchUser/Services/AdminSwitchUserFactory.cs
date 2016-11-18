using System.Linq;
using System.Web.Routing;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Implementation;
using Orchard.Environment.Extensions;
using Orchard.UI.Admin;
using Orchard.Users.Models;

namespace IFle.Orchard.SwitchUser.Services
{
    [OrchardFeature("IFle.Orchard.SwitchUser")]
    public class AdminSwitchUserFactory : ShapeDisplayEvents
    {
        private readonly IContentManager _contentManager;
        private readonly WorkContext _workContext;

        public AdminSwitchUserFactory(
            IWorkContextAccessor workContextAccessor,
            IShapeFactory shapeFactory,
            IContentManager contentManager)
        {
            _contentManager = contentManager;

            _workContext = workContextAccessor.GetContext();
            Shape = shapeFactory;
        }

        dynamic Shape { get; set; }

        private bool IsActivable()
        {
            // activate on admin screen only
            if (IsSwitchUserAllowed(_workContext))
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether [is user selector allowed] [the specified work context].
        /// </summary>
        /// <param name="workContext">The work context.</param>
        /// <returns>
        ///   <c>true</c> if [is user selector allowed] [the specified work context]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSwitchUserAllowed(WorkContext workContext)
        {
            var httpContext = workContext.HttpContext;
            // TODO add settings for localhost
            //return httpContext.Request.IsLocal && AdminFilter.IsApplied(new RequestContext(httpContext, new RouteData()));

            return AdminFilter.IsApplied(new RequestContext(httpContext, new RouteData()));
        }

        public override void Displaying(ShapeDisplayingContext context)
        {
            context.ShapeMetadata.OnDisplaying(displayedContext =>
            {
                if (displayedContext.ShapeMetadata.Type == "Layout" && IsActivable())
                {
                    var userNameList = _contentManager.Query<UserPart, UserPartRecord>()
                                        .OrderBy(userPart => userPart.UserName)
                                        .List().Select(user => user.UserName).ToList();
                    _workContext.Layout.Header.Add(Shape.AdminSwitchUser(Users: userNameList));
                }
            });
        }
    }
}