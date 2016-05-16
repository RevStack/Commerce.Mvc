using System;
using System.Collections.Generic;
using System.Linq;

namespace RevStack.Commerce.Mvc
{
    #region "Purge Task"
    public class PurgeDiscountTask<TKey> : IDiscountTask<TKey>
    {
        public RuleType RuleType
        {
            get
            {
                return RuleType.Validation;
            }
        }

        public DiscountType Type
        {
            get
            {
                return DiscountType.Purge;
            }
        }

        public IShoppingBag<TKey> Run(IShoppingBag<TKey> bag, IEnumerable<IDiscount> discounts, string code)
        {
            var items = new List<DiscountItem>();
            foreach (var item in bag.DiscountItems)
            {
                var discount = discounts.Where(x => x.Code.ToLower() == item.Code.ToLower()).FirstOrDefault();
                if (discount != null)
                {
                    bool rejectCondition = ((discount.Expires && discount.ExpirationDate < DateTime.Now) || (discount.MinValue != null && discount.MinValue > bag.Subtotal));
                    bool typeCondition = (discount.Type == item.Type && discount.RuleType == item.RuleType);
                    if (!rejectCondition && typeCondition)
                    {
                        items.Add(item);
                    }
                }
            }
            bag.DiscountItems = items;
            bag.Discount = items.Sum(x => x.Total);
            return bag;
        }

        public Tuple<bool, string> Validate(IShoppingBag<TKey> bag, IDiscount discount)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region "Tasks"
    public class SubtotalDiscountDollarTask : DiscountTask
    {
        public override RuleType RuleType
        {
            get
            {
                return RuleType.Subtotal;
            }
        }

        public override DiscountType Type
        {
            get
            {
                return DiscountType.DollarDiscount;
            }
        }

        public override IShoppingBag<string> Run(IShoppingBag<string> bag, IEnumerable<IDiscount> discounts, string code)
        {
            if (code == null)
            {
                return bag;
            }
            var discount = discounts.Where(x => x.Code.ToLower() == code.ToLower() && x.RuleType == RuleType.Subtotal && x.Type == DiscountType.DollarDiscount).FirstOrDefault();
            if (discount == null || !(discount.RuleType == RuleType.Subtotal && discount.Type == DiscountType.DollarDiscount))
            {
                return bag;
            }
            var items = bag.DiscountItems.Where(x => x.Code.ToLower() == code.ToLower());
            var currentItems = bag.DiscountItems.ToList();
            if (!items.Any())
            {
                var item = new DiscountItem
                {
                    RuleType = RuleType.Subtotal,
                    Type = DiscountType.DollarDiscount,
                    Code = code,
                    Description = discount.Description,
                    Sku = null,
                    Total = discount.Amount
                };

                currentItems.Add(item);
                bag.DiscountItems = currentItems;
            }

            bag.Discount = bag.DiscountItems.Sum(x => x.Total);
            return bag;
        }

        public override Tuple<bool, string> Validate(IShoppingBag<string> bag, IDiscount discount)
        {
            var result = validate(bag, discount);
            if (!result.Item1) return result;
            var items = bag.DiscountItems.Where(x => x.RuleType == RuleType.Subtotal);
            if (items.Any())
            {
                return new Tuple<bool, string>(false, "A similar type " + Settings.PromotionLabel + " discount has already been applied");
            }
            else
            {
                return new Tuple<bool, string>(true, null);
            }
        }
    }

    public class SubtotalDiscountPercentageTask : DiscountTask
    {
        public override RuleType RuleType
        {
            get
            {
                return RuleType.Subtotal;
            }
        }

        public override DiscountType Type
        {
            get
            {
                return DiscountType.PercentageDiscount;
            }
        }

        public override IShoppingBag<string> Run(IShoppingBag<string> bag, IEnumerable<IDiscount> discounts, string code)
        {
            if (code == null)
            {
                bag.DiscountItems.Where(x => x.RuleType == RuleType.Subtotal && x.Type == DiscountType.PercentageDiscount).ToList().ForEach(x => x.Total = percentage(discounts, x.Code) * bag.Subtotal);
                bag.Discount = bag.DiscountItems.Sum(x => x.Total);
                return bag;
            }
            var discount = discounts.Where(x => x.Code.ToLower() == code.ToLower() && x.RuleType == RuleType.Subtotal && x.Type == DiscountType.PercentageDiscount).FirstOrDefault();
            if (discount == null || !(discount.RuleType == RuleType.Subtotal && discount.Type == DiscountType.PercentageDiscount))
            {
                return bag;
            }
            DiscountItem item;
            var items = bag.DiscountItems.Where(x => x.Code.ToLower() == code.ToLower());
            var currentItems = bag.DiscountItems.ToList();
            if (!items.Any())
            {
                item = new DiscountItem
                {
                    RuleType = RuleType.Subtotal,
                    Type = DiscountType.PercentageDiscount,
                    Code = code,
                    Description = discount.Description,
                    Sku = null,
                    Total = discount.Percentage * bag.Subtotal
                };

                currentItems.Add(item);
                bag.DiscountItems = currentItems;
            }
            else
            {
                item = items.FirstOrDefault();
                item.Total = discount.Percentage * bag.Subtotal;
            }

            bag.Discount = bag.DiscountItems.Sum(x => x.Total);
            return bag;
        }

        public override Tuple<bool, string> Validate(IShoppingBag<string> bag, IDiscount discount)
        {
            var result = validate(bag, discount);
            if (!result.Item1) return result;
            var items = bag.DiscountItems.Where(x => x.RuleType == RuleType.Subtotal);
            if (items.Any())
            {
                return new Tuple<bool, string>(false, "A similar type " + Settings.PromotionLabel + " discount has already been applied");
            }
            else
            {
                return new Tuple<bool, string>(true, null);
            }
        }

        private decimal percentage(IEnumerable<IDiscount> discounts, string code)
        {
            var discount = discounts.Where(x => x.Code.ToLower() == code.ToLower() && x.RuleType == RuleType.Subtotal && x.Type == DiscountType.PercentageDiscount).FirstOrDefault();
            if (discount == null) return 0;
            else return discount.Percentage;
        }
    }

    #endregion

    
}