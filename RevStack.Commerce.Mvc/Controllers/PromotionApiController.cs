using System;
using System.Linq;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using RevStack.Mvc;
using RevStack.Pattern;

namespace RevStack.Commerce.Mvc
{
    public class CommercePromotionApiController : ApiController
    {
        private readonly IShoppingBagService<ApplicationShoppingBag, string> _shoppingBagService;
        private readonly IService<Discount, string> _discountService;
        private readonly IDiscountTaskList<string> _taskList;
        public CommercePromotionApiController(IShoppingBagService<ApplicationShoppingBag, string> shoppingBagService,
            IService<Discount, string> discountService, IDiscountTaskList<string> discountTaskList)
        {
            _shoppingBagService = shoppingBagService;
            _discountService = discountService;
            _taskList = discountTaskList;
        }

        public virtual async Task<IHttpActionResult> get(string code)
        {
            //validate code
            var discounts = _discountService.Get();
            var discount = discounts.Where(x => x.Code.ToLower() == code.ToLower()).FirstOrDefault();
            if (discount == null)
            {
                return new ContentErrorResult(Request, Settings.PromotionInvalidMessage);
            }

            //validate discount
            var bag = await _shoppingBagService.GetAsync(userId());
            var task = _taskList.Tasks.Where(x => x.RuleType == discount.RuleType && x.Type == discount.Type).FirstOrDefault();
            var tuple = task.Validate(bag, discount);
            if (!tuple.Item1)
            {
                return new ContentErrorResult(Request, tuple.Item2);
            }
            //apply discount
            _taskList.Tasks.ForEach(x => x.Run(bag, discounts, code));
            //save modified bag
            bag = await _shoppingBagService.UpdateAsync(bag);
            return Ok(bag);
        }

        protected virtual string userId()
        {
            return (User.Identity.IsAuthenticated) ? User.Identity.GetUserId() : HttpContext.Current.Request.AnonymousID;
        }
    }
}
