using System;
using System.Threading.Tasks;
using RevStack.Mvc;
using RevStack.Notification;
using RevStack.Configuration;

namespace RevStack.Commerce.Mvc
{
    public class UserAlertMessageService<TKey> : IUserAlertMessageService<TKey>
    {
        public AlertMessage<TKey> Get(NotifyAlert<TKey> entity, UriUtility uri)
        {
            var host = uri.Host;
            var logoUrl = Company.LogoUrl;
            if (logoUrl.IndexOf("http") != 0) logoUrl = host + logoUrl;
            string trackingLabel = "Sign Up";
            string trackingUrl = User.SignUpAction;
            if(entity.IsAuthenticated)
            {
                trackingLabel = "Sign In";
                trackingUrl = User.SignInAction;
            }
            var result = new AlertMessage<TKey>
            {
                Id=entity.Id,
                IsAuthenticated =entity.IsAuthenticated,
                PhoneNumber =entity.PhoneNumber,
                Email =entity.Email,
                Name =entity.Name,
                Date =DateTime.Now,
                Day = DateTime.Now.DayOfWeek.ToString(),
                TrackingUrl =host + trackingUrl,
                TrackingLabel =trackingLabel,
                Company = Company.Name,
                CompanyAddress = Company.Address,
                CompanyPhone = Company.Phone,
                CompanyLogoUrl = logoUrl,
                CssHightlightColor = Html.HighlightColor,
                CssLinkColor = Html.LinkColor,
                Key = entity.Key,
                Value = entity.Value
            };

            return result;
        }

        public Task<AlertMessage<TKey>> GetAsync(NotifyAlert<TKey> entity, UriUtility uri)
        {
            return Task.FromResult(Get(entity, uri));
        }
    }
}