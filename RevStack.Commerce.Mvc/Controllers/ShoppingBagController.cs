using System;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using RevStack.Pattern;
using Elliptical.Mvc;


namespace RevStack.Commerce.Mvc
{
    public class CommerceShoppingBagController<TBag,TKey> : Controller
        where TBag : class, IShoppingBag<TKey>
        where TKey : IConvertible
    {
        private readonly IShoppingBagService<TBag, TKey> _service;
        private readonly IDiscountTaskList<TKey> _taskList;
        private readonly IService<Discount, string> _discountService;
        public CommerceShoppingBagController(IShoppingBagService<TBag, TKey> service, IService<Discount, string> discountService, IDiscountTaskList<TKey> discountTaskList)
        {
            _service = service;
            _discountService = discountService;
            _taskList = discountTaskList;
        }

        public virtual async Task<ActionResult> Index()
        {
            var result = await IndexActionAsync();
            return result;
        }

        [NonAction]
        protected ActionResult IndexAction()
        {
            var bag = _service.Get(userId());
            ///------------validate the bag's discounts, if any -------------------------------///
            //get list of discounts
            var discounts = _discountService.Get();
            //select the tasks
            var tasks = _taskList.Tasks;
            //apply the tasks to the bag
            tasks.ToList().ForEach(x => x.Run(bag, discounts, null));
            //save the modified bag
            _service.Update(bag);
            return View(bag).WithSerialization<ApplicationShoppingBag>("bag");
        }

        [NonAction]
        protected Task<ActionResult> IndexActionAsync()
        {
            return Task.FromResult(IndexAction());
        }

        protected virtual TKey userId()
        {
            return  User.Identity.GetUserId<TKey>();
        }
    }
}