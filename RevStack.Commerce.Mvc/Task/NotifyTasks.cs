using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using RevStack.Identity;
using RevStack.Net;
using RevStack.Notification;

namespace RevStack.Commerce.Mvc
{
    
    public class NotifyOrderEmailTask<TKey> : NotifyBaseTask<TKey>
    {
        public NotifyOrderEmailTask(IIdentityEmailService service) : base(service)
        {
            TaskType = NotifyTaskType.OrderConfirmation;
        }

        public async override Task<bool> RunAsync(INotify<TKey> entity)
        {
            var id = entity.Id;
            var url = BaseUrl + Settings.OrderEmailAction + "/" + id.ToString();
            string page = Http.Get(url);
            var message = new IdentityMessage
            {
                Body = page,
                Destination = entity.Email,
                Subject = Settings.OrderEmailSubject
            };

            await _service.SendAsync(message, Settings.CompanyNotificationEmail, true);
            return true;
        }
    }

    public class NotifyOrderAlertTask<TKey> : NotifyBaseTask<TKey>
    {
        public NotifyOrderAlertTask(IIdentityEmailService service) : base(service)
        {
            TaskType = NotifyTaskType.OrderAlert;
        }

        public async override Task<bool> RunAsync(INotify<TKey> entity)
        {
            var id = entity.Id;
            var url = BaseUrl + Settings.OrderNotificationAction + "/?id=" + id.ToString() + "&key=" + HttpUtility.UrlEncode(entity.Key) + "&value=" + HttpUtility.UrlEncode(entity.Value);
            string page = Http.Get(url);
            var message = new IdentityMessage
            {
                Body = page,
                Destination = entity.Email,
                Subject = "Order #" + id.ToString() + " " + Settings.OrderNotificationLabel + " for " + entity.Name
            };

            await _service.SendAsync(message, Settings.CompanyNotificationEmail, true);
            return true;
        }
    }

    public class NotifyUserAlertTask<TKey> : NotifyBaseTask<TKey>
    {
        public NotifyUserAlertTask(IIdentityEmailService service) : base(service)
        {
            TaskType = NotifyTaskType.UserAlert;
        }

        public async override Task<bool> RunAsync(INotify<TKey> entity)
        {
            var id = entity.Id;
            var url = BaseUrl + Settings.UserNotificationAction + "/?id=" + id.ToString() + "&key=" + HttpUtility.UrlEncode(entity.Key) 
                + "&value=" + HttpUtility.UrlEncode(entity.Value) + "&authenticated=" + entity.IsAuthenticated + "&name=" + HttpUtility.UrlEncode(entity.Name);
            string page = Http.Get(url);
            var message = new IdentityMessage
            {
                Body = page,
                Destination = entity.Email,
                Subject = Settings.Company + " " + Settings.UserNotificationLabel + " for " + entity.Name
            };

            await  _service.SendAsync(message, Settings.CompanyNotificationEmail, true);
            return true;
        }
    }

    public class NotifyUserSignUpTask<TKey> : NotifyBaseTask<TKey>
    {
        public NotifyUserSignUpTask(IIdentityEmailService service) : base(service)
        {
            TaskType = NotifyTaskType.UserSignUp;
        }

        public async override Task<bool> RunAsync(INotify<TKey> entity)
        {
            var id = entity.Id;
            var url = BaseUrl + Settings.UserNotificationAction + "/?id=" + id.ToString() + "&key=" + HttpUtility.UrlEncode(entity.Key)
                + "&value=" + HttpUtility.UrlEncode(entity.Value) + "&authenticated=" + entity.IsAuthenticated + "&name=" + HttpUtility.UrlEncode(entity.Name);
            string page = Http.Get(url);
            var message = new IdentityMessage
            {
                Body = page,
                Destination = entity.Email,
                Subject = Settings.Company + " user registration confirmation for " + entity.Name
            };

            await _service.SendAsync(message, Settings.CompanyNotificationEmail, true);
            return true;
        }
    }

    public class NotifyOrderAdminTask<TKey> : NotifyBaseTask<TKey>
    {
        public NotifyOrderAdminTask(IIdentityEmailService service) : base(service)
        {
            TaskType = NotifyTaskType.AdminAlert;
        }

        public async override Task<bool> RunAsync(INotify<TKey> entity)
        {
            string newLine = "<br>";
            var id = entity.Id;
            string body = "Dear Order Administrator:" + newLine + newLine;
            body += "A new order #" + id.ToString() + " has been submitted by " + entity.Name + "." + newLine + newLine;
            body += "Login at " + "<a href=\"" + entity.TrackingUrl + "\">" + entity.TrackingUrl + "</a> to manage.";

            var message = new IdentityMessage
            {
                Body = body,
                Destination = Settings.CompanyNotificationRecipientEmail,
                Subject = "New " + Settings.Company + " order from " + entity.Name
            };

            await _service.SendAsync(message, Settings.CompanyNotificationEmail, true);
            return true;
        }
    }
}