using System;
using System.Configuration;
using RevStack.Configuration;

namespace RevStack.Commerce.Mvc
{
    public static partial class Settings
    {
       
        public static CurrentOrderStatus OrderConfirmationStatus
        {
            get
            {
                string result = Order.ConfirmationStatus;
                if (!string.IsNullOrEmpty(result)) return (CurrentOrderStatus)Enum.Parse(typeof(CurrentOrderStatus), result);
                return CurrentOrderStatus.Charged;
            }
        }
       
        public static string BagImageBaseUrl
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Bag.Image.BaseUrl"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "/images";
            }
        }
        public static string BagImageWidth
        {
            get
            {
                string result = ConfigurationManager.AppSettings["Bag.Image.Width"];
                if (!string.IsNullOrEmpty(result)) return result;
                else return "";
            }
        }
       
    }
}