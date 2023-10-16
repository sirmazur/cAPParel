﻿using cAPParel.API.Models;
using cAPParel.API.Services.CategoryServices;
using Microsoft.AspNetCore.Mvc;

namespace cAPParel.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }
        [HttpGet("{categoryid}", Name = "GetCategory")]
        public async Task<ActionResult<CategoryDto>> GetGame(int categoryid)
        {
            if (ModelState.IsValid)
            {
                var item = await _categoryService.GetExtendedByIdWithEagerLoadingAsync(categoryid);
                if (item!=null)
                {
                    return Ok(item);
                }
                else
                {
                    return NotFound();
                }

            }
            else
            {
                return BadRequest(ModelState);
            }
        }
       

        [HttpDelete("{categorytodeleteid}")]
        public async Task<ActionResult> DeleteCategory(int categorytodeleteid)
        {
            var operationResult = await _categoryService.DeleteByIdAsync(categorytodeleteid);
            if (operationResult.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(operationResult.HttpResponseCode, operationResult.ErrorMessage);
            }
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategoryAsync(CategoryForCreationDto category)
        {
            var categoryDto = await _categoryService.CreateCategoryAsync(category);
            return CreatedAtRoute("GetCategory", new { categoryId = categoryDto.Id }, categoryDto);
        }

    }
}
