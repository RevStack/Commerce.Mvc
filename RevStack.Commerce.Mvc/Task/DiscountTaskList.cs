using System;
using System.Collections.Generic;


namespace RevStack.Commerce.Mvc
{
    public class ApplicationDiscountTaskList : IDiscountTaskList<string>
    {
        public List<IDiscountTask<string>> Tasks
        {
            get
            {
                return new List<IDiscountTask<string>>
                {
                    new PurgeDiscountTask<string>(),
                    new SubtotalDiscountDollarTask(),
                    new SubtotalDiscountPercentageTask()
                };
            }
        }
    }

    public class DefaultDiscountTaskList : IDiscountTaskList<string>
    {
        public List<IDiscountTask<string>> Tasks
        {
            get
            {
                return new List<IDiscountTask<string>>();
            }
        }
    }
}