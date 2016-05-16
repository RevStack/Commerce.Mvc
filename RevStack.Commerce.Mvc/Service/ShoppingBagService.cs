using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RevStack.Pattern;
using RevStack.Mvc;

namespace RevStack.Commerce.Mvc
{
   
    public class ShoppingBagService<TBag,TKey> : IShoppingBagService<TBag,TKey>
        where TBag : class, IShoppingBag<TKey>
        where TKey : IEquatable<TKey>, IConvertible
    {
        private readonly IRepository<TBag, TKey> _repository;
        private readonly Func<TBag> _bagFactory;
        public ShoppingBagService(IRepository<TBag, TKey> repository, Func<TBag> bagFactory)
        {
            _repository = repository;
            _bagFactory = bagFactory;
        }
        public TBag Get(TKey id)
        {
            var bags = get(id);
            if(bags.Any())
            {
                return bags.FirstOrDefault();
            }
            else
            {
                return createBag(id);
            }
        }

        public Task<TBag> GetAsync(TKey id)
        {
            return Task.FromResult(Get(id));
        }


        public ShoppingBagItem<TKey> AddItem(TKey id, ShoppingBagItem<TKey> item)
        {
            if (EqualityComparer<TKey>.Default.Equals(item.Id,default(TKey)))
            {
                new Exception("bag item requires an assigned TKey Id");
            }
            var bag = getBag(id);
            var items = bag.Items.ToList();
            items.Add(item);
            bag.Items = items;
            bag = calculateBag(bag);
            _repository.Update(bag);

            return item;
        }

        public Task<ShoppingBagItem<TKey>> AddItemAsync(TKey id, ShoppingBagItem<TKey> item)
        {
            return Task.FromResult(AddItem(id, item));
        }

        public ShoppingBagItem<TKey> UpdateItem(TKey id, ShoppingBagItem<TKey> item)
        {
            var bag = getBag(id);
            var oldItem = bag.Items.Where(x => x.Compare(x.Id,item.Id)).FirstOrDefault();
            var index = bag.Items.ToList().IndexOf(oldItem);
            var items = bag.Items.ToList();
            items[index] = item;
            bag.Items = items;
            bag = calculateBag(bag);
            _repository.Update(bag);

            return item;
        }

        public Task<ShoppingBagItem<TKey>> UpdateItemAsync(TKey id, ShoppingBagItem<TKey> item)
        {
            return Task.FromResult(UpdateItem(id, item));
        }

        public TBag Update(TBag bag)
        {
            bag = calculateBag(bag);
            _repository.Update(bag);
            return bag;
        }

        public Task<TBag> UpdateAsync(TBag bag)
        {
            return Task.FromResult(Update(bag));
        }

        public bool RemoveItem(TKey id, TKey itemId)
        {
            var bag = getBag(id);
            var item = bag.Items.Select(x => x).Where(x => x.Compare(x.Id, itemId)).FirstOrDefault();
            var items = bag.Items.ToList();
            items.Remove(item);
            bag.Items = items;
            bag = calculateBag(bag);
            bag = _repository.Update(bag);

            return true;
        }

        public Task<bool> RemoveItemAsync(TKey id, TKey itemId)
        {
            return Task.FromResult(RemoveItem(id, itemId));
        }

        public bool Empty(TKey id)
        {
            var oldBag = getBag(id);
            var newBag = _bagFactory();
            newBag.Id = oldBag.Id;
            newBag.UserId = id;
            _repository.Update(newBag);

            return true;
        }

        public Task<bool> EmptyAsync(TKey id)
        {
            return Task.FromResult(Empty(id));
        }

        public int GetCount(TKey id)
        {
            return getBag(id).Items.Sum(x => x.Quantity);
        }

        public Task<int> GetCountAsync(TKey id)
        {
            return Task.FromResult(GetCount(id));
        }

        public bool Move(TKey id, TKey oldId)
        {
            var bag = getBag(id);
            if (bag != null)
            {
                bag.UserId = id;
                _repository.Update(bag);
            }
            return true;
        }

        public Task<bool> MoveAsync(TKey id, TKey oldId)
        {
            return Task.FromResult(Move(id, oldId));
        }

        #region "private"

        private IEnumerable<TBag> get(TKey id)
        {
            return _repository.Find(x => x.Compare(x.UserId, id));
        }

        private TBag createBag(TKey id)
        {
            var bag = _bagFactory();
            bag.UserId = id;
            _repository.Add(bag);
            return bag;
        }

        private TBag getBag(TKey id)
        {
            var bags = get(id);
            if (bags.Any()){
                return bags.FirstOrDefault();
            }
            else
            {
                return createBag(id);
            }
        }

        private TBag calculateBag(TBag bag)
        {
            var subtotal = bag.Items.Sum(x => x.Total);
            var tax = Convert.ToDecimal(bag.Tax);
            var discount = Convert.ToDecimal(bag.Discount);
            var shipping = Convert.ToDecimal(bag.Shipping);
            var total = subtotal + tax - discount + shipping;
            bag.Subtotal = subtotal;
            bag.Total = total;

            return bag;
           
        }
        private TBag format(TBag bag)
        {
            bag.Subtotal = decimal.Round(bag.Subtotal, 2, MidpointRounding.AwayFromZero);
            bag.Tax = decimal.Round(bag.Tax, 2, MidpointRounding.AwayFromZero);
            bag.Discount = decimal.Round(Convert.ToDecimal(bag.Discount), 2, MidpointRounding.AwayFromZero);
            bag.Shipping = decimal.Round(bag.Shipping, 2, MidpointRounding.AwayFromZero);
            bag.Total = decimal.Round(bag.Total, 2, MidpointRounding.AwayFromZero);

            return bag;
        }
        #endregion
    }
}