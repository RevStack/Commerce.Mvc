using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using RevStack.Notification;

namespace RevStack.Commerce.Mvc
{
    public class TransactionApiController<TTransaction,TBag,TOrder,TPayment,TDiscount,TKey> : ApiController
        where TTransaction : class,ITransaction<TBag,TOrder,TPayment,TKey>
        where TBag : class,IShoppingBag<TKey>
        where TOrder : class,IOrder<TPayment,TKey>
        where TPayment : class, IPayment
        where TDiscount : class, IDiscount
    {
        protected readonly ITransactionService<TBag, TOrder, TPayment, TDiscount, TKey> _transactionService;
        protected readonly IPaymentTaskList<TBag,TOrder,TPayment,TKey> _paymentTaskList;
        protected readonly IOrderTaskList<TBag,TOrder,TPayment,TKey> _orderTaskList;
        protected readonly INotifyTaskList<TKey> _notifyTaskList;
        public TransactionApiController(ITransactionService<TBag,TOrder,TPayment,TDiscount,TKey> transactionService,
           IPaymentTaskList<TBag,TOrder,TPayment,TKey> paymentTaskList,
           IOrderTaskList<TBag, TOrder, TPayment, TKey> orderTaskList,
            INotifyTaskList<TKey> notifyTaskList
            )
        {
            _transactionService = transactionService;
            _paymentTaskList = paymentTaskList;
            _orderTaskList = orderTaskList;
            _notifyTaskList = notifyTaskList;
        }

        public virtual async Task<IHttpActionResult> post(TTransaction entity)
        {
            
            var task = _paymentTaskList.Tasks.Where(x => x.PaymentType == entity.Payment.PaymentType).SingleOrDefault();

            var transaction=await task.RunAsync(entity);
            if (!transaction.Approved)
            {
                OnOrderFailure(transaction);
                return Content(HttpStatusCode.Forbidden, transaction);
            }

            var orderTasks=_orderTaskList.Tasks.Where(x => x.TaskType == OrderTaskType.Order);
            var notifyTasks = _notifyTaskList.Tasks.Where(x => x.TaskType == NotifyTaskType.OrderConfirmation);

            orderTasks.ToList().ForEach(x => x.Run(transaction, User.Identity.IsAuthenticated));

            _transactionService.Post(transaction);

            notifyTasks.ToList().ForEach(x => x.RunAsync(new NotifyAlert<TKey>
            {
                Id=transaction.Order.Id,
                Name=transaction.BillingAddress.FirstName + " " + transaction.Order.BillingAddress.LastName,
                Date=DateTime.Now,
                Email=transaction.Order.Email,
                PhoneNumber=transaction.Order.BillingAddress.PhoneNumber,
                TrackingUrl=transaction.Order.TrackingUrl
            }
                ));

            OnPostSuccess(transaction);

            return Content(HttpStatusCode.OK, transaction);
        }



        protected virtual void OnPostSuccess(ITransaction<TBag, TOrder, TPayment, TKey> transaction) { }
    

        protected virtual void OnOrderFailure(ITransaction<TBag, TOrder, TPayment, TKey> transaction) { }


    }
}
