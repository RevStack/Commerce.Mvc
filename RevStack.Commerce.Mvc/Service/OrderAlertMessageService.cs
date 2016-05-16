using System;
using System.Linq;
using System.Threading.Tasks;
using RevStack.Mvc;
using RevStack.Pattern;
using RevStack.Notification;

namespace RevStack.Commerce.Mvc
{
    public class OrderAlertMessageService<TOrder, TPayment, TKey> : IOrderAlertMessageService<TOrder, TPayment, TKey>
         where TOrder : class, IOrder<TPayment, TKey>
         where TPayment : class, IPayment
         where TKey : IEquatable<TKey>
    {
        private readonly IRepository<TOrder, TKey> _repository;
        public OrderAlertMessageService(IRepository<TOrder, TKey> repository)
        {
            _repository = repository;
        }


        public OrderAlertMessage<TKey> Get(NotifyAlert<TKey> entity, UriUtility uri)
        {
            var order = _repository.Find(x => x.Compare(x.Id, entity.Id)).FirstOrDefault();
            if (order == null) return null;
            DateTime date = Convert.ToDateTime(entity.Date);
            var host = uri.Host;
            var logoUrl = Settings.CompanyLogoUrl;
            if (logoUrl.IndexOf("http") != 0) logoUrl = host + logoUrl;
            var count = order.Items.Count();
            string itemText = "";
            if (count > 1)
            {
                itemText = " and " + (count - 1).ToString() + " other items";
            }
            var result = new OrderAlertMessage<TKey>
            {
                Id = order.Id,
                Name = order.BillingAddress.FirstName + " " + order.BillingAddress.LastName,
                ShippingName = order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName,
                Street = order.ShippingAddress.Street,
                Address = order.ShippingAddress.City + ", " + order.ShippingAddress.State + " " + order.ShippingAddress.ZipCode,
                ItemCount = order.Items.Count(),
                MainItem = order.Items.FirstOrDefault().Name,
                ItemText = itemText,
                ShipMessage = Settings.OrderEmailShipMessage,
                Email = order.Email,
                Total = order.Total.ToString("C"),
                Day = date.DayOfWeek.ToString(),
                Date = DateTime.Now,
                TrackingUrl = host + order.TrackingUrl,
                Company = Settings.Company,
                CompanyAddress = Settings.CompanyAddress,
                CompanyPhone = Settings.CompanyPhone,
                CompanyLogoUrl = logoUrl,
                CssHightlightColor = Settings.HtmlHighlightColor,
                CssLinkColor = Settings.HtmlLinkColor,
                Key =entity.Key,
                Value=entity.Value
            };

            return result;
        }

        public Task<OrderAlertMessage<TKey>> GetAsync(NotifyAlert<TKey> entity, UriUtility uri)
        {
            return Task.FromResult(Get(entity,uri));
        }
    }
}