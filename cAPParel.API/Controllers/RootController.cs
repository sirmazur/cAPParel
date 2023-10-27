using cAPParel.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace cAPParel.API.Controllers
{
    
    [ApiController]
    [Route("api")]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();
            links.Add(
                new LinkDto(
                href: Url.Link("GetRoot", new { }),
                rel: "self",
                method: "GET"));

            links.Add(
                new LinkDto(
                href: Url.Link("GetCategories", new { }),
                rel: "categories",
                method: "GET"));

            links.Add(
                new LinkDto(
                href: Url.Link("CreateCategory", new { }),
                rel: "create_category",
                method: "POST"));
       
            return Ok(links);
        }
       
    }
}
