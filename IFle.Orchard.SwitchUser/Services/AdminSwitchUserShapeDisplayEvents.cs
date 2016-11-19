using System.Linq;
using System.Web.Routing;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Implementation;
using Orchard.Environment.Extensions;
using Orchard.Roles.Models;
using Orchard.UI.Admin;
using Orchard.Users.Models;

namespace IFle.Orchard.SwitchUser.Services
{
    [OrchardFeature("IFle.Orchard.SwitchUser")]
    public class AdminSwitchUserShapeDisplayEvents : ShapeDisplayEvents
    {
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _workContextAccessor;

        public AdminSwitchUserShapeDisplayEvents(
            IWorkContextAccessor workContextAccessor,
            IShapeFactory shapeFactory,
            IContentManager contentManager)
        {
            _contentManager = contentManager;
            _workContextAccessor = workContextAccessor;
            Shape = shapeFactory;
        }

        dynamic Shape { get; set; }

        private bool IsActivable()
        {
            // activate on admin screen only
            if (PermissionService.IsSwitchUserAllowed(_workContextAccessor.GetContext()))
                return true;

            return false;
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
                    _workContextAccessor.GetContext().Layout.Header.Add(Shape.AdminSwitchUser(Users: userNameList));
                }
            });
        }
    }
}