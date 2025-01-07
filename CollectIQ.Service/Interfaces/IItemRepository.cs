using CollectIQ.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectIQ.Service.Interfaces
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllItemsAsync(bool trackChanges);
        Task<Item> GetItemByIdAsync(Guid itemId, bool trackChanges);
        Task<IEnumerable<Item>> GetItemsByTypeAsync(Guid itemTypeId, bool trackChanges);  
        Task<IEnumerable<Item>> GetItemsByUserIdAsync(Guid userId, bool trackChanges);
        Task AddItemAsync(Item item);
        Task UpdateItemAsync(Item item);
        Task DeleteItemAsync(Guid itemId, bool trackChanges);
        Task<IEnumerable<ItemType>> GetAllItemTypesAsync(bool trackChanges);
        Task AddItemsAsync(IEnumerable<Item> items);

    }
}
