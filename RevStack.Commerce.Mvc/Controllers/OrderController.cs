using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using RevStack.Mvc;
using RevStack.Pattern;
using RevStack.Notification;

namespace RevStack.Commerce.Mvc
{
    public class CommerceOrderController<TOrder,TPayment,TKey> : Controller
        where TOrder : class, IOrder<TPayment,TKey>
        where TPayment : class, IPayment
    {
        private readonly IService<TOrder, TKey> _orderService;
        private readonly IOrderMessageService<TOrder, TPayment, TKey> _orderMessageService;
        private readonly IOrderAlertMessageService<TOrder, TPayment, TKey> _orderAlertMessageService;
        public CommerceOrderController(IService<TOrder, TKey> orderService,
            IOrderMessageService<TOrder, TPayment, TKey> orderMessageService, 
            IOrderAlertMessageService<TOrder, TPayment, TKey> orderAlertMessageService)
        {
            _orderService = orderService;
            _orderMessageService = orderMessageService;
            _orderAlertMessageService = orderAlertMessageService;
        }

        public virtual async Task<ActionResult> Email(TKey id)
        {
            var result = await EmailActionAsync(id);
            return result;
        }

        public virtual async Task<ActionResult> Notification(TKey id,string key, string value)
        {
            var result = await NotificationActionAsync(id,key,value);
            return result;
        }

        public virtual async Task<ActionResult> Track(TKey id)
        {
            var result = await TrackActionAsync(id);
            return result;
        }

        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult History(TKey id)
        {
            return View();
        }

        public virtual ActionResult Detail(TKey id)
        {
            return View("History");
        }

        [NonAction]
        protected ActionResult EmailAction(TKey id)
        {
            var uri = new UriUtility(Request);
            var orderMessage = _orderMessageService.Get(id, uri);

            return View(orderMessage);
        }

        [NonAction]
        protected Task<ActionResult> EmailActionAsync(TKey id)
        {
            return Task.FromResult(EmailAction(id));
        }

        [NonAction]
        protected ActionResult NotificationAction(TKey id,string key,string value)
        {
            var entity = new NotifyAlert<TKey>
            {
                Id = id,
                Key = key,
                Value = value,
                Date = DateTime.Now

            };
            var uri = new UriUtility(Request);
            var message = _orderAlertMessageService.Get(entity, uri);
            return View(message);
        }

        [NonAction]
        protected Task<ActionResult> NotificationActionAsync(TKey id,string key, string value)
        {
            return Task.FromResult(NotificationAction(id,key,value));
        }

        [NonAction]
        protected ActionResult TrackAction(TKey id)
        {
            var entity = _orderService.Find(x => x.Compare(x.Id,id)).FirstOrDefault();
            return View(entity);
        }

        [NonAction]
        protected Task<ActionResult> TrackActionAsync(TKey id)
        {
            return Task.FromResult(TrackAction(id));
        }

    }
}