namespace CourierBackend.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using CourierBackend.Data;
    using CourierBackend.Models;
    using Microsoft.AspNetCore.Mvc;
    using CourierBackend.Data.Resources;
    using CourierBackend.Services;
    using CourierBackend.Data.Requests;
    using FluentValidation;
    using CourierBackend.Helpers;
    using CourierBackend.Services.Model.Interfaces;
    using CourierBackend.Services.Auth.Interfaces;
    using Microsoft.AspNetCore.Authorization;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly CourierContext _context;
        //private readonly IPaginationService _paginationService;

        private readonly IQueryService _queryService;

        private readonly IValidator<AdminRequest> _validator;

        private readonly IAdminService _adminService;

        private readonly ICurrentAdminService _currentUser;

        public AdminController(CourierContext context, IQueryService queryService, IValidator<AdminRequest> validator, IAdminService adminService, ICurrentAdminService currentUser)
        {
            _context = context;
            _queryService = queryService;
            _validator = validator;
            _adminService = adminService;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<ActionResult> GetAdmins([FromQuery] QueryParameters parameters)
        {
            if (!_currentUser.CanManageAdmin())
            {
                return Unauthorized(ResponseStructures._401Response("You are not authorized to do that!"));
            }

            var query = _context.Admins.AsQueryable();

            var result = await _queryService.QueryAsync(
                query,
                parameters,
                AdminResource.FromModel,

                // SEARCH LOGIC
                (q, search) => q.Where(a =>
                    a.Name.ToLower().Contains(search.ToLower()) ||
                    a.Email.ToLower().Contains(search.ToLower()) ||
                    a.PhoneNumber.ToLower().Contains(search.ToLower())
                )
            );

            return Ok(result);
        }

        // GET: api/Admin/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminResource>> GetAdmin(int id)
        {
            var admin = await _adminService.GetByIdAsync(id);

            if (!_currentUser.CanManageAdmin())
            {
                return Unauthorized(ResponseStructures._401Response("You are not authorized to do that!"));
            }

            return AdminResource.FromModel(admin);
        }

        // POST: api/Admin
        [HttpPost]
        public async Task<IActionResult> PostAdmin(AdminRequest request)
        {
            var result = await _validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                return BadRequest(ResponseStructures.ErrorResponse(result.Errors.ToDictionary()));
            }

            if (!_currentUser.CanManageAdmin())
            {
                return Unauthorized(ResponseStructures._401Response("You are not authorized to do that!"));
            }

            Admin admin = await _adminService.RegisterAsync(request);

            AdminResource resource = AdminResource.FromModel(admin);

            return CreatedAtAction(nameof(GetAdmin), new { id = resource.Id }, ResponseStructures.SuccessResponse(resource));
        }

        [HttpPut("{id}/update")]
        public async Task<IActionResult> PutAdmin([FromBody] AdminRequest request, int id)
        {
            var result = await _validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                return BadRequest(ResponseStructures.ErrorResponse(result.Errors.ToDictionary()));
            }

            Admin admin = await _adminService.UpdatedAsync(request, id);

            AdminResource resource = AdminResource.FromModel(admin);

            return Ok(ResponseStructures.SuccessResponse(resource));
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            return Ok(new
            {
                Id = _currentUser.GetId(),
                Name = _currentUser.GetName(),
                Email = _currentUser.GetEmail(),
                Role = _currentUser.GetRole(),
                CanManageAdmin = _currentUser.CanManageAdmin(),
            });
        }
    }
}