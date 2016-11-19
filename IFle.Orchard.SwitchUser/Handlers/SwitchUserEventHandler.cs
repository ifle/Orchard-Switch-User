using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Security;
using Orchard.Users.Events;

namespace IFle.Orchard.SwitchUser.Handlers
{
    public class SwitchUserEventHandler : IUserEventHandler
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        public SwitchUserEventHandler(IWorkContextAccessor workContextAccessor) {
            _workContextAccessor = workContextAccessor;
        }

        public void Creating(UserContext context) {
            
        }

        public void Created(UserContext context) {
            
        }

        public void LoggingIn(string userNameOrEmail, string password) {
            
        }

        public void LoggedIn(IUser user) {
            
        }

        public void LogInFailed(string userNameOrEmail, string password) {
            
        }

        public void LoggedOut(IUser user) {
            var workContext = _workContextAccessor.GetContext();
            if (workContext.HttpContext.Session != null)
                workContext.HttpContext.Session.Abandon();
        }

        public void AccessDenied(IUser user) {
            
        }

        public void ChangedPassword(IUser user) {
            
        }

        public void SentChallengeEmail(IUser user) {
            throw new NotImplementedException();
        }

        public void ConfirmedEmail(IUser user) {
            throw new NotImplementedException();
        }

        public void Approved(IUser user) {
            throw new NotImplementedException();
        }
    }
}