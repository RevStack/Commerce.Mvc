using System;
using System.Collections.Generic;
using RevStack.Identity;
using RevStack.Notification;

namespace RevStack.Commerce.Mvc
{
    public class CommerceNotifyTaskList<TKey> : INotifyTaskList<TKey>
    {
        protected readonly IIdentityEmailService _service;
        protected readonly IIdentitySmsService _smsService;
        public CommerceNotifyTaskList(IIdentityEmailService service, IIdentitySmsService smsService)
        {
            _service = service;
            _smsService = smsService;
        }

        public virtual List<INotifyTask<TKey>> Tasks
        {
            get
            {
                var list = new List<INotifyTask<TKey>>();
                var item = new NotifyOrderEmailTask<TKey>(_service,_smsService);
                var item2 = new NotifyOrderAlertTask<TKey>(_service,_smsService);
                var item3 = new NotifyUserAlertTask<TKey>(_service,_smsService);
                var item4 = new NotifyUserSignUpTask<TKey>(_service,_smsService);
                var item5 = new NotifyOrderAdminTask<TKey>(_service,_smsService);
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