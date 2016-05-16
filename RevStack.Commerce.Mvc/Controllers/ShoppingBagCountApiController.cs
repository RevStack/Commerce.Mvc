using System;
using System.Net;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace RevStack.Commerce.Mvc
{
    public class CommerceShoppingBagCountApiController : ApiController
    {
        private readonly IShoppingBagService<ApplicationShoppingBag, string> _shoppingBagService;
        public CommerceShoppingBagCountApiController(IShoppingBagService<ApplicationShoppingBag, string> shoppingBagService)
        {
            _shoppingBagService = shoppingBagService;
        }

        public virtual async Task<IHttpActionResult> get()
        {
            var count = await _shoppingBagService.GetCountAsync(userId());
            return Content(HttpStatusCode.OK, count);
        }

        protected virtual string userId()
        {
            return (User.Identity.IsAuthenticated) ? User.Identity.GetUserId() : HttpContext.Current.Request.AnonymousID;
        }
    }
}
