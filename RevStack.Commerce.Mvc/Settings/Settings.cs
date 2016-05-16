using System;
using System.Configuration;

namespace RevStack.Commerce.Mvc
{
    public static partial class Settings
    {
        public static string Company
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Company"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "My Company";
            }
        }
        public static string CompanyAddress
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Company.Address"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "";
            }
        }
        public static string CompanyPhone
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Company.Phone"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "";
            }
        }
        public static string CompanyLogoUrl
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Company.LogoUrl"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "";
            }
        }
        public static string CompanyNotificationEmail
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Company.Notification.Email"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "dev.null@localhost";
            }
        }
        public static string CompanyNotificationSms
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Company.Notification.Sms"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "0000000000";
            }
        }
        public static string CompanyNotificationRecipientEmail
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Company.Notification.RecipientEmail"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "dev.null@localhost";
            }
        }
        public static string CompanyNotificationRecipientSms
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Company.Notification.RecipientSms"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "0000000000";
            }
        }
        public static int OrderKeyLength
        {
            get
            {
                string length = ConfigurationManager.AppSettings["Commerce.Order.Key.Length"];
                if (string.IsNullOrEmpty(length)) return 9;
                else return Convert.ToInt32(length);
            }
        }
        public static string OrderEmailShipMessage
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Order.Email.ShipMessage"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "We’ll send a confirmation when your items ship";
            }
        }
        public static string OrderEmailSubject
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Order.Email.Subject"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "Your Order Confirmation";
            }
        }
        public static string OrderEmailAction
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Order.Email.Action"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "/Order/Email";
            }
        }
        public static CurrentOrderStatus OrderConfirmationStatus
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Order.Confirmation.DefaultStatus"];
                if (!string.IsNullOrEmpty(result)) return (CurrentOrderStatus)Enum.Parse(typeof(CurrentOrderStatus), result);
                return CurrentOrderStatus.Charged;
            }
        }
        public static string OrderConfirmationAlertKey
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Order.Confirmation.AlertKey"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "Payment Processing";
            }
        }
        public static string OrderConfirmationAlertValue
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Order.Confirmation.AlertValue"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "Credit card payment succussfully processed";
            }
        }
        public static string OrderNotificationAction
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Order.Notification.Action"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "/Order/Notification";
            }
        }
        public static string OrderNotificationLabel
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Order.Notification.Label"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "alert";
            }
        }
        public static string OrderTrackingAction
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Order.Tracking.Action"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "/Order/Track";
            }
        }
        public static string OrderTrackingUnauthenticatedAction
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Order.Tracking.Unauthenticated.Action"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "/Order/Track";
            }
        }
        public static string OrderAdminTrackingUrl
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Order.Admin.Tracking.Url"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "/Order/Track";
            }
        }
        public static string PromotionInvalidMessage
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Promotion.Message.Invalid"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "Invalid promotion code";
            }
        }
        public static string PromotionLabel
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Promotion.Label.Type"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "promotion";
            }
        }
        public static string UserNotificationAction
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.User.Notification.Action"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "/User/Notification";
            }
        }
        public static string UserNotificationLabel
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.User.Notification.Label"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "message";
            }
        }
        public static string UserSignInAction
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.User.SignIn.Action"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "/Identity/Sign-In";
            }
        }
        public static string UserSignUpAction
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.User.SignUp.Action"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "/Identity/Sign-Up";
            }
        }
        public static string UserSignUpKey
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.User.SignUp.Alert.Key"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "Sign Up Confirmation";
            }
        }
        public static string UserSignUpValue
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.User.SignUp.Alert.Value"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "You have successfully registered your account!";
            }
        }
        public static string BagImageBaseUrl
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Bag.Image.BaseUrl"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "/images";
            }
        }
        public static string BagImageWidth
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Bag.Image.Width"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "";
            }
        }
        public static string HtmlHighlightColor
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Html.Email.HighlightColor"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "#2fbd51";
            }
        }
        public static string HtmlLinkColor
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Commerce.Html.Email.LinkColor"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "#5fb3f6";
            }
        }
    }
}