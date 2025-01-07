using AutoMapper;
using CollectIQ.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CollectIQ.Api.Controllers
{
    
    public class BaseApiController : ControllerBase
    {
        protected readonly IRepositoryManager _repository;
        protected readonly ILoggerManager _logger;
        protected readonly IMapper _mapper;

        public BaseApiController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

    }
}
