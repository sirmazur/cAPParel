using cAPParel.API.Services.FieldsValidationServices;
using cAPParel.API.Services.UserServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace cAPParel.API.Controllers
{
    [ApiController]
    [Route("api/items")]
    public class ItemController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IFieldsValidationService _fieldsValidationService;
        public ItemController(IUserService userService, ProblemDetailsFactory problemDetailsFactory, IFieldsValidationService fieldsValidationService)
        {
            _userService = userService;
            _problemDetailsFactory = problemDetailsFactory;
            _fieldsValidationService = fieldsValidationService;
        }
    }
}
