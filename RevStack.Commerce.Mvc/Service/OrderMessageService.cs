using System;
using System.Linq;
using System.Threading.Tasks;
using RevStack.Pattern;
using RevStack.Mvc;
using RevStack.Configuration;

namespace RevStack.Commerce.Mvc
{
    public class OrderMessageService<TOrder,TPayment,TKey> : IOrderMessageService<TOrder,TPayment,TKey>
        where TOrder : class, IOrder<TPayment, TKey>
        where TPayment : class, IPayment
        where TKey : IEquatable<TKey>
    {
        private readonly IRepository<TOrder,TKey> _repository;
        public OrderMessageService(IRepository<TOrder, TKey> repository)
        {
            _repository = repository;
        }

        public OrderMessage<TKey> Get(TKey id,UriUtility uri)
        {
            var order= _repository.Find(x => x.Compare(x.Id, id)).FirstOrDefault();
            if (order == null) return null;
            var host = uri.Host;
            var logoUrl = Company.LogoUrl;
            if (logoUrl.IndexOf("http") != 0) logoUrl = host + logoUrl;
            var count = order.Items.Count();
            string itemText = "";
            if(count > 1)
            {
                itemText = " and " + (count -1).ToString() + " other items";
            }
            var orderEmail = new OrderMessage<TKey>
            {
                Id = order.Id,
                Name = order.BillingAddress.FirstName + " " + order.BillingAddress.LastName,
                ShippingName = order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName,
                Street = order.ShippingAddress.Street,
                Address = order.ShippingAddress.City + ", " + order.ShippingAddress.State + " " + order.ShippingAddress.ZipCode,
                ItemCount = order.Items.Count(),
                MainItem = order.Items.FirstOrDefault().Name,
                ItemText=itemText,
                ShipMessage=Order.EmailShipMessage,
                Email = order.Email,
                Total = order.Total.ToString("C"),
                Day = order.OrderDate.DayOfWeek.ToString(),
                Date = order.OrderDate,
                TrackingUrl = host + order.TrackingUrl,
                Company=Company.Name,
                CompanyAddress=Company.Address,
                CompanyPhone=Company.Phone,
                CompanyLogoUrl=logoUrl,
                CssHightlightColor=Html.HighlightColor,
                CssLinkColor=Html.LinkColor
            };
           
            return orderEmail;
        }

        public Task<OrderMessage<TKey>> GetAsync(TKey id,UriUtility uri)
        {
            return Task.FromResult(Get(id,uri));
        }
    }
}