using System;
using System.Collections.Generic;
using RevStack.Identity;
using RevStack.Notification;

namespace RevStack.Commerce.Mvc
{
    public class DefaultNotifyTaskList<TKey> : INotifyTaskList<TKey>
    {
        protected readonly IIdentityEmailService _service;
        public DefaultNotifyTaskList(IIdentityEmailService service)
        {
            _service = service;   
        }

        public virtual List<INotifyTask<TKey>> Tasks
        {
            get
            {
                var list = new List<INotifyTask<TKey>>();
                var item = new NotifyOrderEmailTask<TKey>(_service);
                var item2 = new NotifyOrderAlertTask<TKey>(_service);
                var item3 = new NotifyUserAlertTask<TKey>(_service);
                var item4 = new NotifyUserSignUpTask<TKey>(_service);
                var item5 = new NotifyOrderAdminTask<TKey>(_service);
                list.Add(item);
                list.Add(item2);
                list.Add(item3);
                list.Add(item4);
                list.Add(item5);
                return list;
            }
        }
    }
}