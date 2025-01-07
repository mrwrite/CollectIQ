using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectIQ.Service.Interfaces
{
    public interface IRepositoryManager
    {
        IUserAuthenticationRepository UserAuthentication { get; }
        IItemRepository Item { get; }
        IItemTypeRepository ItemType { get; }

        Task SaveAsync();
    }
}
