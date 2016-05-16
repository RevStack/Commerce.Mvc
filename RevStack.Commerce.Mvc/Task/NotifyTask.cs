using System;
using System.Threading.Tasks;
using System.Web;
using RevStack.Mvc;
using RevStack.Identity;
using RevStack.Notification;

namespace RevStack.Commerce.Mvc
{
    public class NotifyBaseTask<TKey> : INotifyTask<TKey>
    {
        protected readonly IIdentityEmailService _service;
        public NotifyBaseTask(IIdentityEmailService service)
        {
            _service = service;
        }
        public NotifyBaseTask() {}

        private NotifyTaskType _taskType;
        private string _identifier;
        public NotifyTaskType TaskType
        {
            get
            {
                return _taskType;
            }

            set
            {
                _taskType = value;
            }
        }
        public string Identifier
        {
            get
            {
                return _identifier;
            }

            set
            {
                _identifier = value;
            }
        } 

        public virtual Task<bool> RunAsync(INotify<TKey> entity)
        {
            throw new NotImplementedException();
        }

        protected string BaseUrl
        {
            get
            {
                var context = new HttpContextWrapper(HttpContext.Current);
                HttpRequestBase request = context.Request;
                var uri = new UriUtility(request);
                return uri.Host;
            }
        }
    }
}