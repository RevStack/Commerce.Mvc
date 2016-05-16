using System;
using System.Collections.Generic;
using RevStack.Mvc;

namespace RevStack.Commerce.Mvc
{
   
    public class CreditCardPaymentTask : ApplicationPaymentTask
        
    {
        public override string PaymentType
        {
            get
            {
                return "CreditCard";
            }
        }

        protected Payment RecordPaymentFields(Payment payment)
        {
            var dict = new Dictionary<string, string>();
            string creditCardNumber = payment.ProcessorFields["CardNumber"];
            string cvvNumber = payment.ProcessorFields["CvvNumber"];
            dict["CardNumber"] = CreditCard.Encode(creditCardNumber);
            dict["CardExpirationMonth"] = payment.ProcessorFields["CardExpirationMonth"];
            dict["CardExpirationYear"] = payment.ProcessorFields["CardExpirationYear"];
            dict["CardExpirationDate"] = payment.ProcessorFields["CardExpirationMonth"] + "/" + payment.ProcessorFields["CardExpirationYear"];
            dict["CvvNumber"] = "**" + cvvNumber.LastChars(1);
            var fields = new List<KeyValueItem>();
            foreach(var item in dict)
            {
                var field = new KeyValueItem
                {
                    Key = item.Key,
                    Value = item.Value
                };
                fields.Add(field);
            }
            payment.PaymentFields = fields;
            payment.ProcessorFields = new Dictionary<string, string>();
            return payment;
        }

    }
}