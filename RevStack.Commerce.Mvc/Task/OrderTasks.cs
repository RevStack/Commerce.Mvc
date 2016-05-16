using System;
using System.Collections.Generic;


namespace RevStack.Commerce.Mvc
{

    public class OrderConfirmationTask : ApplicationOrderTask
    {
        public OrderConfirmationTask()
        {
            TaskType = OrderTaskType.Order;
        }

        public override ITransaction<ApplicationShoppingBag, ApplicationOrder, Payment, string> Run(ITransaction<ApplicationShoppingBag, 
            ApplicationOrder, Payment, string> transaction, bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                transaction.Order.IsAuthenticatedUser = true;
                transaction.Order.TrackingUrl = Settings.OrderTrackingAction + "/" + transaction.Order.Id;
            }
            else
            {
                transaction.Order.IsAuthenticatedUser = false;
                transaction.Order.TrackingUrl = Settings.OrderTrackingUnauthenticatedAction + "/" + transaction.Order.Id;
            }

            transaction.Order.OrderStatus = new OrderStatus
            {
                Status = Settings.OrderConfirmationStatus,
                Notifications = new List<OrderNotification>
                {
                  new OrderNotification
                  {
                    Date = DateTime.Now,
                    Key = Settings.OrderConfirmationAlertKey,
                    Value = Settings.OrderConfirmationAlertValue
                  }
               }
            };

            return transaction;
        }
    }

    public class DefaultPaymentOptionTask : ApplicationOrderTask
    {
        public DefaultPaymentOptionTask()
        {
            TaskType = OrderTaskType.PaymentOption;
        }

        public override ITransaction<ApplicationShoppingBag, ApplicationOrder, Payment, string> Run(ITransaction<ApplicationShoppingBag, ApplicationOrder, Payment, string> transaction, bool isAuthenticated)
        {
            transaction.PaymentOptions = new List<PaymentOption>();
            return transaction;
        }
    }

}