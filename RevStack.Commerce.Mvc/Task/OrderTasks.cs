using System;
using System.Collections.Generic;
using RevStack.Configuration;

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
                transaction.Order.TrackingUrl = Order.TrackingAction + "/" + transaction.Order.Id;
            }
            else
            {
                transaction.Order.IsAuthenticatedUser = false;
                transaction.Order.TrackingUrl = Order.TrackingUnauthenticatedAction + "/" + transaction.Order.Id;
            }

            transaction.Order.OrderStatus = new OrderStatus
            {
                Status = Settings.OrderConfirmationStatus,
                Notifications = new List<OrderNotification>
                {
                  new OrderNotification
                  {
                    Date = DateTime.Now,
                    Key = Order.ConfirmationAlertKey,
                    Value = Order.ConfirmationAlertValue
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