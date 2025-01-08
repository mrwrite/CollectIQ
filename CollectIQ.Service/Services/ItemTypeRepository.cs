using CollectIQ.Core.Models;
using CollectIQ.Repo.Data;
using CollectIQ.Repo.GenericRepository.Service;
using CollectIQ.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectIQ.Service.Services
{
    public class ItemTypeRepository : RepositoryBase<ItemType>, IItemTypeRepository
    {
        private readonly RepositoryContext _context;

        public ItemTypeRepository(RepositoryContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ItemType> GetItemTypeByIdAsync(Guid itemTypeId, bool trackChanges)
        {
            return await (await FindByConditionAsync(itemType => itemType.Id.Equals(itemTypeId), trackChanges)).FirstOrDefaultAsync();
        }

        public async Task<ItemType> GetItemTypeByNameAsync(string itemTypeName, bool trackChanges)
        {
            return await (await FindByConditionAsync(itemType => itemType.Name.Equals(itemTypeName), trackChanges)).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ItemType>> GetAllItemTypesAsync(bool trackChanges)
        {
            return await _context.ItemTypes.ToListAsync();
        }
    }
}
