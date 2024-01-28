using cAPParel.API.Entities;
using cAPParel.API.Services.VisitServices;
using Microsoft.AspNetCore.Mvc;

namespace cAPParel.API.Controllers
{
    [ApiController]
    [Route("api/visits")]
    public class VisitController : ControllerBase
    {
        private readonly IVisitService _visitService;
        public VisitController(IVisitService visitService)
        {
            _visitService = visitService;
        }
        [HttpGet]
        public async Task<ActionResult<Visit>> GetVisit() 
        {
            var visit = await _visitService.GetVisit();
            return Ok(visit);
        }
        [HttpPost]
        public async Task<IActionResult> AddVisit()
        {
            await _visitService.AddVisit();
            return NoContent();
        }
    }
}
