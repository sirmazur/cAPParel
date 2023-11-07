using cAPParel.API.Models;
using cAPParel.API.Services.FieldsValidationServices;
using cAPParel.API.Services.ItemServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace cAPParel.API.Controllers
{
    [ApiController]
    [Route("api/items")]
    public class ItemController : ControllerBase
    {

        private readonly IItemService _itemService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IFieldsValidationService _fieldsValidationService;
        public ItemController(IItemService itemService, ProblemDetailsFactory problemDetailsFactory, IFieldsValidationService fieldsValidationService)
        {
            _itemService = itemService;
            _problemDetailsFactory = problemDetailsFactory;
            _fieldsValidationService = fieldsValidationService;
        }

        [HttpPost(Name = "CreateItem")]
        public async Task<ActionResult<ItemDto>> CreateItem(ItemForCreationDto item)
        {
            try
            {
                var result = await _itemService.CreateAsync(item);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
