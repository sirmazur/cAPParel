using cAPParel.API.Models;
using cAPParel.API.Services.OrderServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace cAPParel.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost(Name = "PlaceOrder")]
        [Authorize(Policy = "MustBeLoggedIn")]
        public async Task<ActionResult<OrderDto>> PlaceOrder(List<int> pieceIds)
        {
            try
            {
                var createdOrder = await _orderService.CreateOrder(new OrderForCreationDto {
                    PiecesIds = pieceIds, 
                    UserId=int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value)});
                
                return Ok(createdOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "MustBeAdmin")]
        [HttpPatch("{ordertoupdateid}", Name = "PartiallyUpdateOrder")]
        public async Task<IActionResult> PartialUpdateOrder(int ordertoupdateid, JsonPatchDocument<OrderForUpdateDto> patchDocument)
        {
            var operationResult = await _orderService.PartialUpdateAsync(ordertoupdateid, patchDocument);
            if (operationResult.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

    }
}
