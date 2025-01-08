using CollectIQ.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectIQ.Service.Interfaces
{
    public interface IItemTypeRepository
    {
        Task<ItemType> GetItemTypeByIdAsync(Guid itemTypeId, bool trackChanges);
        Task<ItemType> GetItemTypeByNameAsync(string itemTypeName, bool trackChanges);

        Task<IEnumerable<ItemType>> GetAllItemTypesAsync(bool trackChanges);
    }
}
