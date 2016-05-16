using System;
using System.Linq;
using System.Web.Http;
using System.Threading.Tasks;

namespace RevStack.Commerce.Mvc
{
    public class CommerceOrderTaxApiController : ApiController
    {
        private readonly IShoppingBagService<ApplicationShoppingBag, string> _shoppingBagService;
        private readonly IOrderTaskList<ApplicationShoppingBag, ApplicationOrder, Payment, string> _orderTaskList;
        public CommerceOrderTaxApiController(IShoppingBagService<ApplicationShoppingBag, string> shoppingBagService, 
            IOrderTaskList<ApplicationShoppingBag, ApplicationOrder, Payment, string> orderTaskList)
        {
            _shoppingBagService = shoppingBagService;
            _orderTaskList = orderTaskList;
        }

        public virtual async Task<IHttpActionResult> post(Transaction<ApplicationShoppingBag, ApplicationOrder, Payment, string> transaction)
        {
            var taxTasks = _orderTaskList.Tasks.Where(x => x.TaskType == OrderTaskType.Tax).ToList();
            taxTasks.ForEach(x => x.Run(transaction, User.Identity.IsAuthenticated));
            var bag = transaction.ShoppingBag;
            await _shoppingBagService.UpdateAsync(bag);   
            return Ok(transaction);
        }
    }
}
