using AutoMapper;
using CollectIQ.Core.Models;
using CollectIQ.Repo.Data;
using CollectIQ.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;


namespace CollectIQ.Service.Services
{
    public class RepositoryManager : IRepositoryManager
    {
        private RepositoryContext _repositoryContext;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IMapper _mapper;
        private IConfiguration _configuration;
        public IUserAuthenticationRepository _userAuthenticationRepository;
        public IItemRepository _itemRepository;
        public IItemTypeRepository _itemTypeRepository;
        public IEmailManager _emailManager;
        public IHttpContextAccessor _httpContextAccessor;
       
        public RepositoryManager(RepositoryContext repositoryContext, UserManager<User> userManager, IMapper mapper, IConfiguration configuration, IEmailManager emailManager, IHttpContextAccessor httpContextAccessor, SignInManager<User> signInManager)
        {
            _repositoryContext = repositoryContext;
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _emailManager = emailManager;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
        }

        public Task SaveAsync() => _repositoryContext.SaveChangesAsync();

        public IUserAuthenticationRepository UserAuthentication
        {
            get
            {
                if (_userAuthenticationRepository is null)
                    _userAuthenticationRepository = new UserAuthenticationRepository(_userManager, _configuration, _mapper, _emailManager, _httpContextAccessor, _signInManager);

                return _userAuthenticationRepository;
            }
            set
            {
            }
        }

        public IItemRepository Item
        {
            get
            {
                if (_itemRepository is null)
                    _itemRepository = new ItemRepository(_repositoryContext);
                return _itemRepository;
            }

        }

        public IItemTypeRepository ItemType
        {
            get
            {
                if (_itemTypeRepository is null)
                    _itemTypeRepository = new ItemTypeRepository(_repositoryContext);
                return _itemTypeRepository;
            }

        }
    }
}
