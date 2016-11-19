using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Roles.Models;
using Orchard.UI.Admin;

namespace IFle.Orchard.SwitchUser.Services
{
    public class PermissionService
    {
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

            if (!AdminFilter.IsApplied(new RequestContext(httpContext, new RouteData())))
                return false;

            //if (httpContext.Request.IsLocal)
            //    return true;

            if(IsAdminUser(workContext))
                return true;

            if(httpContext.Session["SwitchUserAllowed"] != null)
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether [is admin user] [the specified user].
        /// </summary>
        /// <param name="workContext">The work context.</param>
        /// <returns>
        ///   <c>true</c> if [is admin user] [the specified user]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAdminUser(WorkContext workContext)
        {
            if (workContext == null)
                return false;

            var user = workContext.CurrentUser;
            if (user == null)
                return false;

            if (workContext.CurrentSite.SuperUser == user.UserName)
                return true;

            var roles = user.As<IUserRoles>();
            if (roles == null)
                return false;

            return roles.Roles != null ? roles.Roles.Contains("Administrator") : false;
        }
    }
}