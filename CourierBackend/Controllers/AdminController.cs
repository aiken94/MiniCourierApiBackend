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

    [Route("api/[controller]")]
    [ApiController]
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

        // GET: api/Admin
        //[HttpGet]
        //public async Task<ActionResult<Pagination<AdminResource>>> GetAdmins([FromQuery] PaginationRequest request)
        //{
        //    var query = _context.Admins.AsQueryable();

        //    var result = await _paginationService.PaginateAsync(
        //        query,
        //        request,
        //        AdminResource.FromModel
        //    );

        //    return Ok(result);
        //}

        [HttpGet]
        public async Task<ActionResult> GetAdmins([FromQuery] QueryParameters parameters)
        {
            var query = _context.Admins.AsQueryable();

            Dictionary<string, int> roles = new Dictionary<string, int>();

            roles["admin"] = 0;
            roles["user"] = 1;

            var result = await _queryService.QueryAsync(
                query,
                parameters,
                AdminResource.FromModel,

                // SEARCH LOGIC
                (q, search) => q.Where(a =>
                    a.Name.ToLower().Contains(search.ToLower()) ||
                    a.Email.ToLower().Contains(search.ToLower()) ||
                    a.PhoneNumber.ToLower().Contains(search.ToLower())
                ),

                // FILTER LOGIC
                (q, filters) =>
                {
                    var role = roles[filters["role"]];

                    if (filters.ContainsKey("role"))
                        q = q.Where(a => a.Role == role);

                    return q;
                }
            );

            return Ok(result);
        }

        // GET: api/Admin/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminResource>> GetAdmin(int id)
        {
            var admin = await _adminService.GetByIdAsync(id);

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
                Role = _currentUser.GetRole()
            });
        }
    }
}