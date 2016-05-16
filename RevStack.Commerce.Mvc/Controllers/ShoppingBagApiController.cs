using System;
using System.Net;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace RevStack.Commerce.Mvc
{
    public class CommerceShoppingBagApiController : ApiController
    {
        private readonly IShoppingBagService<ApplicationShoppingBag, string> _shoppingBagService;
        private string _baseUrl = Settings.BagImageBaseUrl;
        private string _imageWidth = Settings.BagImageWidth;
        public CommerceShoppingBagApiController(IShoppingBagService<ApplicationShoppingBag, string> shoppingBagService)
        {
            _shoppingBagService = shoppingBagService;
        }
        
        public string BaseImageUrl
        {
            get
            {
                return _baseUrl;
            }
            set
            {
                _baseUrl = value;
            }
        }

        public string ImageWidth
        {
            get
            {
                return _imageWidth;
            }
            set
            {
                _imageWidth = value;
            }
        }

        public virtual async Task<IHttpActionResult> get()
        {
            var bag = await _shoppingBagService.GetAsync(userId());
            return Content(HttpStatusCode.OK, bag);
        }

        public virtual async Task<IHttpActionResult> post(ShoppingBagItem<string> item)
        {
            item.Id = Guid.NewGuid().ToString();
            item.Image = BaseImageUrl + "/" + item.Image + ImageWidth;
            var postedItem = await _shoppingBagService.AddItemAsync(userId(), item);
            return Content(HttpStatusCode.OK, postedItem);
        }

        public virtual async Task<IHttpActionResult> put(ShoppingBagItem<string> item)
        {
            string id = userId();

            if (item.Quantity > 0)
            {
                await _shoppingBagService.UpdateItemAsync(id, item);
            }
            else
            {
                await _shoppingBagService.RemoveItemAsync(id, item.Id);
            }
            return Content(HttpStatusCode.OK, item);
        }

        public virtual async Task<IHttpActionResult> delete(string id)
        {
            bool deleted = await _shoppingBagService.RemoveItemAsync(userId(), id);
            return Content(HttpStatusCode.OK, deleted);
        }

        protected virtual string userId()
        {
            return (User.Identity.IsAuthenticated) ? User.Identity.GetUserId() : HttpContext.Current.Request.AnonymousID;
        }
    }
}
