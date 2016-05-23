using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using RevStack.Identity;
using RevStack.Net;
using RevStack.Notification;
using RevStack.Configuration;

namespace RevStack.Commerce.Mvc
{
    
    public class NotifyOrderEmailTask<TKey> : NotifyTask<TKey>
    {
        public NotifyOrderEmailTask(IIdentityEmailService service,IIdentitySmsService smsService) : base(service,smsService)
        {
            TaskType = NotifyTaskType.OrderConfirmation;
        }

        public async override Task<bool> RunAsync(INotify<TKey> entity)
        {
            var id = entity.Id;
            var url = BaseUrl + Order.EmailAction + "/" + id.ToString();
            string page = Http.Get(url);
            var message = new IdentityMessage
            {
                Body = page,
                Destination = entity.Email,
                Subject = Order.EmailSubject
            };

            await _service.SendAsync(message, Company.NotificationEmail, true);
            return true;
        }
    }

    public class NotifyOrderAlertTask<TKey> : NotifyTask<TKey>
    {
        public NotifyOrderAlertTask(IIdentityEmailService service, IIdentitySmsService smsService) : base(service,smsService)
        {
            TaskType = NotifyTaskType.OrderAlert;
        }

        public async override Task<bool> RunAsync(INotify<TKey> entity)
        {
            var id = entity.Id;
            var url = BaseUrl + Order.NotificationAction + "/?id=" + id.ToString() + "&key=" + HttpUtility.UrlEncode(entity.Key) + "&value=" + HttpUtility.UrlEncode(entity.Value);
            string page = Http.Get(url);
            var message = new IdentityMessage
            {
                Body = page,
                Destination = entity.Email,
                Subject = "Order #" + id.ToString() + " " + Order.NotificationLabel + " for " + entity.Name
            };

            await _service.SendAsync(message, Company.NotificationEmail, true);
            return true;
        }
    }

    public class NotifyOrderAdminTask<TKey> : NotifyTask<TKey>
    {
        public NotifyOrderAdminTask(IIdentityEmailService service, IIdentitySmsService smsService) : base(service,smsService)
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
                Destination = Company.NotificationRecipientEmail,
                Subject = "New " + Company.Name + " order from " + entity.Name
            };

            await _service.SendAsync(message, Company.NotificationEmail, true);
            return true;
        }
    }
}