using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RevStack.Pattern;
using RevStack.Identity;
using RevStack.Identity.Mvc;
using RevStack.Mvc;

namespace RevStack.Commerce.Mvc
{
    
    public class TransactionService<TBag,TOrder,TPayment,TDiscount,TUser,TUserManager,TKey> : ITransactionService<TBag,TOrder,TPayment,TDiscount,TKey>
        where TBag : class, IShoppingBag<TKey>
        where TOrder : class, IOrder<TPayment,TKey>
        where TPayment : class, IPayment
        where TDiscount : class, IDiscount
        where TUser : class, IIdentityUser<TKey>
        where TUserManager : ApplicationUserManager<TUser,TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IRepository<TBag, TKey> _repository;
        private readonly IRepository<TOrder, TKey> _orderRepository;
        private readonly IRepository<TDiscount, string> _discountRepository;
        protected readonly Func<TUserManager> _userManagerFactory;
        private readonly Func<TBag> _bagFactory;
        private readonly Func<TPayment> _paymentFactory;
        private readonly Func<TOrder> _orderFactory;
        private readonly IDiscountTaskList<TKey> _discountTaskList;
        private readonly IOrderTaskList<TBag, TOrder, TPayment, TKey> _orderTaskList;
        public TransactionService(IRepository<TBag, TKey> repository, IRepository<TOrder, TKey> orderRepository, IRepository<TDiscount,string> discountRepository,
            Func<TUserManager> userManagerFactory,Func<TBag> bagFactory, Func<TPayment> paymentFactory, Func<TOrder> orderFactory,
            IDiscountTaskList<TKey> discountTaskList, IOrderTaskList<TBag, TOrder, TPayment, TKey> orderTaskList
            )
        {
            _repository = repository;
            _orderRepository = orderRepository;
            _discountRepository = discountRepository;
            _userManagerFactory = userManagerFactory;
            _bagFactory = bagFactory;
            _paymentFactory = paymentFactory;
            _orderFactory = orderFactory;
            _discountTaskList = discountTaskList;
            _orderTaskList = orderTaskList;
        }

        public virtual async Task<ITransaction<TBag, TOrder,TPayment,TKey>> GetAsync(TKey id)
        {
            ///construct a default transaction instance to mediate the transaction
            var transaction = new Transaction<TBag,TOrder,TPayment,TKey>();
            bool isAuthenticated = true;
            var bag = _repository.Find(x => x.Compare(x.UserId,id)).FirstOrDefault();
            var discounts = _discountRepository.Get();
            var tasks = _discountTaskList.Tasks;
            var orderTasks = _orderTaskList.Tasks;
            var paymentOptionTasks = orderTasks.Select(x=> x).Where(x => x.TaskType == OrderTaskType.PaymentOption);
            var shippingTasks= orderTasks.Select(x=>x).Where(x => x.TaskType == OrderTaskType.ShippingMethod);
            tasks.ToList().ForEach(x => x.Run(bag, discounts, null));
            var userManager = _userManagerFactory();
            var user = await userManager.FindByIdAsync(id);
            if(user==null)
            {
                transaction.BillingAddress = new BillingAddress();
                isAuthenticated = false;
            }
            else
            {
                transaction.BillingAddress = Mapper.Map<BillingAddress>(user);
            }
            if (bag == null)
            {
                bag = _bagFactory();
            }
            paymentOptionTasks.ToList().ForEach(x => x.Run(transaction, isAuthenticated));
            shippingTasks.ToList().ForEach(x => x.Run(transaction, isAuthenticated));
            transaction.ShoppingBag = bag;
            transaction.ShippingAddress = new ShippingAddress();
            transaction.ShippingMethod = new ShippingMethod();
            transaction.UserId = id;
            transaction.Payment = _paymentFactory();
            transaction.Order = _orderFactory();

            return transaction;
        }

       
        public virtual ITransaction<TBag, TOrder,TPayment, TKey> Post(ITransaction<TBag, TOrder, TPayment,TKey> transaction)
        {
            ////populate the order object on the transaction instance
            transaction.Order.BillingAddress = transaction.BillingAddress;
            transaction.Order.ShippingAddress = transaction.ShippingAddress;
            transaction.Order.Discount = transaction.ShoppingBag.Discount;
            transaction.Order.Email = transaction.BillingAddress.Email;
            transaction.Order.Items = transaction.ShoppingBag.Items.Select(x => Mapper.Map<OrderItem<TKey>>(x));
            transaction.Order.DiscountItems = transaction.ShoppingBag.DiscountItems;
            transaction.Order.OrderDate = DateTime.Now;
            transaction.Order.Payment = transaction.Payment;
            transaction.Order.Shipping = transaction.ShoppingBag.Shipping;
            transaction.Order.ShippingAddress = transaction.ShippingAddress;
            transaction.Order.ShippingMethod = transaction.ShippingMethod;
            transaction.Order.Subtotal = transaction.ShoppingBag.Subtotal;
            transaction.Order.Tax = transaction.ShoppingBag.Tax;
            transaction.Order.Total = transaction.ShoppingBag.Total;
            transaction.Order.UserId = transaction.UserId;
            transaction.Order.Notes = transaction.Notes;


            ///insert order
            _orderRepository.Add(transaction.Order);

            ////delete bag
            var bags = _repository.Find(x => x.Compare(x.UserId, transaction.UserId));
            if(bags.Any())
            {
                foreach(var bag in bags)
                {
                    _repository.Delete(bag);
                }
            }
            return transaction;

        }

        public Task<ITransaction<TBag, TOrder,TPayment, TKey>> PostAsync(ITransaction<TBag, TOrder,TPayment, TKey> transaction)
        {
            return Task.FromResult(Post(transaction));
        }

    }

         
}