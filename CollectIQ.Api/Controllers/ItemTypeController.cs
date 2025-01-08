using AutoMapper;
using CollectIQ.Core.Dtos;
using CollectIQ.Core.Models;
using CollectIQ.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollectIQ.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemTypeController : BaseApiController
    {
        public ItemTypeController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) : base(repository, logger, mapper)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetItemTypes()
        {
            var itemTypes = await _repository.ItemType.GetAllItemTypesAsync(true);
            var itemTypeDto = _mapper.Map<IEnumerable<ItemTypeDto>>(itemTypes);
            return Ok(itemTypeDto);
        }
    }
}
