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
    public class ItemRepository : RepositoryBase<Item>, IItemRepository
    {
        private readonly RepositoryContext _context;

        public ItemRepository(RepositoryContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddItemAsync(Item item)
            => await CreateAsync(item);

        public async Task DeleteItemAsync(Guid itemId, bool trackChanges)
            => await RemoveAsync(await GetItemByIdAsync(itemId, trackChanges));


        public async Task<IEnumerable<Item>> GetAllItemsAsync(bool trackChanges)
            => await _context.Items
                .Include(i => i.ItemType)
                .ToListAsync();


        public async Task<IEnumerable<ItemType>> GetAllItemTypesAsync(bool trackChanges)
        {
            return await _context.ItemTypes.ToListAsync();
        }

        public async Task<Item> GetItemByIdAsync(Guid itemId, bool trackChanges)
            => await (await FindByConditionAsync(item =>
            item.Id.Equals(itemId), trackChanges)).SingleOrDefaultAsync();

        public async Task<IEnumerable<Item>> GetItemsByUserIdAsync(Guid userId, bool trackChanges)
            => await (await FindByConditionAsync(item =>
            item.UserId.Equals(userId), trackChanges))
            .Include(i => i.ItemType).ToListAsync();

        public async Task<IEnumerable<Item>> GetItemsByTypeAsync(Guid itemTypeId, bool trackChanges)
            => await (await FindByConditionAsync(item =>
            item.ItemTypeId.Equals(itemTypeId), trackChanges))
            .Include(i => i.ItemType).ToListAsync();



        public async Task UpdateItemAsync(Item item)
        {
            await UpdateAsync(item);
        }

        public async Task AddItemsAsync(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                await CreateAsync(item);
            }
        }


    }
}
