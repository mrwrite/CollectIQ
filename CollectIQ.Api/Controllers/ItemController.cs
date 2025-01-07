using AutoMapper;
using CollectIQ.Core.Dtos;
using CollectIQ.Core.Models;
using CollectIQ.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollectIQ.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : BaseApiController
    {
        public ItemController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper): base(repository,logger, mapper)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            var items = await _repository.Item.GetAllItemsAsync(true);
            var itemDto = _mapper.Map<IEnumerable<ItemDto>>(items);
            return Ok(itemDto);
        }

        [HttpGet("{id}", Name = "GetItemById")]
        public async Task<IActionResult> GetItemById(Guid id)
        {
            var item = await _repository.Item.GetItemByIdAsync(id, false);
            if (item == null)
            {
                return NotFound();
            }

            var itemDto = _mapper.Map<ItemDto>(item);
            return Ok(itemDto);
        }

        [HttpGet("type/{itemTypeId}")]
        public async Task<IActionResult> GetItemsByType(Guid itemTypeId)
        {
            var items = await _repository.Item.GetItemsByTypeAsync(itemTypeId, true);
            if (items == null || !items.Any())
            {
                _logger.LogError($"No items found for item type id: {itemTypeId}");
                return NotFound();
            }

            var itemsDto = _mapper.Map<IEnumerable<ItemDto>>(items);

            return Ok(itemsDto);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetItemsByUser(Guid userId)
        {
            var items = await _repository.Item.GetItemsByUserIdAsync(userId, true);
            if (items == null || !items.Any())
            {
                _logger.LogError($"No items found for user id: {userId}");
                return NotFound();
            }

            var itemsDto = _mapper.Map<IEnumerable<ItemDto>>(items);

            return Ok(itemsDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] CreateItemDto itemDto)
        {
            if(itemDto == null)
            {
                _logger.LogError("ItemDto object sent from client is null.");
                return BadRequest("ItemDto object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the ItemDto object");
                return UnprocessableEntity(ModelState);
            }

            var itemType = await _repository.ItemType.GetItemTypeByIdAsync(itemDto.ItemTypeId, true);
            if(itemType == null)
            {
                _logger.LogError($"Item Type with id: {itemDto.ItemTypeId} doesn't exist in the database.");
                return NotFound();
            }

            Item item;

            switch (itemType.Name.ToLower())
            {
                case "watch":
                    item = _mapper.Map<Watch>(itemDto);
                    break;
                case "cologne":
                    item = _mapper.Map<Cologne>(itemDto);
                    break;
                case "sneaker":
                    item = _mapper.Map<Sneaker>(itemDto);
                    break;
                default:
                    _logger.LogError("Unsupported ItemType");
                    return BadRequest("Unsupported ItemType");
            }


            await _repository.Item.AddItemAsync(item);
            await _repository.SaveAsync();

            Console.WriteLine($"Tracked Item ID: {item.Id}"); // Check if ID is populated here

            var itemToReturn = _mapper.Map<ItemDto>(item);

            return CreatedAtRoute("GetItemById", new { id = itemToReturn.Id }, itemToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, [FromBody] UpdateItemDto itemDto)
        {
            if (itemDto == null)
            {
                _logger.LogError("itemDto object sent from client is null");
                return BadRequest("ItemDto object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid itemDto object sent from client.");
                return UnprocessableEntity(ModelState);
            }

            var itemEntity = await _repository.Item.GetItemByIdAsync(id, true);
            if (itemEntity == null)
            {
                _logger.LogError($"Item with id: {id}, hasn't been found in db.");
                return NotFound();
            }

            _mapper.Map(itemDto, itemEntity);
            await _repository.Item.UpdateItemAsync(itemEntity);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var item = await _repository.Item.GetItemByIdAsync(id, true);
            if (item == null)
            {
                _logger.LogError($"Item with id: {id}, hasn't been found in db.");
                return NotFound();
            }

            await _repository.Item.DeleteItemAsync(id, true);
            await _repository.SaveAsync();

            return NoContent();
        }


    }
}
