using System;
using System.Web.Mvc;
using System.Threading.Tasks;
using RevStack.Mvc;
using RevStack.Notification;

namespace RevStack.Commerce.Mvc
{
    public class CommerceUserController<TKey> : Controller
    {
        private readonly IUserAlertMessageService<TKey> _userAlertMessageService;
        public CommerceUserController(IUserAlertMessageService<TKey> userAlertMessageService)
        {
            _userAlertMessageService = userAlertMessageService;
        }

        public virtual async Task<ActionResult> Notification(TKey id, string key, string value,bool authenticated,string name)
        {
            var result = await NotificationActionAsync(id, key, value,authenticated,name);
            return result;
        }

        [NonAction]
        protected ActionResult NotificationAction(TKey id, string key, string value,bool authenticated,string name)
        {
            var entity = new NotifyAlert<TKey>
            {
                Id = id,
                Key = key,
                Value = value,
                Date = DateTime.Now,
                IsAuthenticated=authenticated,
                Name=name
            };

            var uri = new UriUtility(Request);
            var message = _userAlertMessageService.Get(entity, uri);
            return View(message);
        }

        [NonAction]
        protected Task<ActionResult> NotificationActionAsync(TKey id, string key, string value,bool authenticated,string name)
        {
            return Task.FromResult(NotificationAction(id, key, value,authenticated,name));
        }
    }
}