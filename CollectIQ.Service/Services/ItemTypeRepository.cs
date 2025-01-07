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

        public ItemTypeRepository(RepositoryContext context) : base(context)
        {

        }

        public async Task<ItemType> GetItemTypeByIdAsync(Guid itemTypeId, bool trackChanges)
        {
            return await (await FindByConditionAsync(itemType => itemType.Id.Equals(itemTypeId), trackChanges)).FirstOrDefaultAsync();
        }
    }
}
