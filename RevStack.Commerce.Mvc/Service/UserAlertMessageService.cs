using System;
using System.Threading.Tasks;
using RevStack.Mvc;
using RevStack.Notification;

namespace RevStack.Commerce.Mvc
{
    public class UserAlertMessageService<TKey> : IUserAlertMessageService<TKey>
    {
        public UserAlertMessage<TKey> Get(NotifyAlert<TKey> entity, UriUtility uri)
        {
            var host = uri.Host;
            var logoUrl = Settings.CompanyLogoUrl;
            if (logoUrl.IndexOf("http") != 0) logoUrl = host + logoUrl;
            string trackingLabel = "Sign Up";
            string trackingUrl = Settings.UserSignUpAction;
            if(entity.IsAuthenticated)
            {
                trackingLabel = "Sign In";
                trackingUrl = Settings.UserSignInAction;
            }
            var result = new UserAlertMessage<TKey>
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
                Company = Settings.Company,
                CompanyAddress = Settings.CompanyAddress,
                CompanyPhone = Settings.CompanyPhone,
                CompanyLogoUrl = logoUrl,
                CssHightlightColor = Settings.HtmlHighlightColor,
                CssLinkColor = Settings.HtmlLinkColor,
                Key = entity.Key,
                Value = entity.Value
            };

            return result;
        }

        public Task<UserAlertMessage<TKey>> GetAsync(NotifyAlert<TKey> entity, UriUtility uri)
        {
            return Task.FromResult(Get(entity, uri));
        }
    }
}