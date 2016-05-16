using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using RevStack.Pattern;
using System.Web.OData.Query;
using System.Web.OData;
using System.Web.OData.Extensions;
using Microsoft.AspNet.Identity;

namespace RevStack.Commerce.Mvc
{
    public class CommerceOrderApiController : ApiController
    {
        private readonly IService<ApplicationOrder, string> _orderService;
        public CommerceOrderApiController(IService<ApplicationOrder, string> orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        public virtual async Task<IHttpActionResult> Get(ODataQueryOptions<ApplicationOrder> options)
        {
            var id = User.Identity.GetUserId();
            ODataQuerySettings settings = new ODataQuerySettings() { };
            var result = await _orderService.FindAsync(x => x.UserId == id);
            IQueryable query = options.ApplyTo(result.AsQueryable(), settings);
            var pagedResult = new PageResult<ApplicationOrder>(query as IQueryable<ApplicationOrder>, Request.ODataProperties().NextLink, Request.ODataProperties().TotalCount);
            return Content(HttpStatusCode.OK, pagedResult);
        }

        public virtual async Task<IHttpActionResult> Get(string id)
        {
            var result = await _orderService.FindAsync(x => x.Id == id);
            var entity = result.FirstOrDefault();
            return Ok(entity);
        }
    }
}
