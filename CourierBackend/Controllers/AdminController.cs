namespace CourierBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CourierBackend.Data;
    using CourierBackend.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using CourierBackend.Data.Resource;
    using CourierBackend.Services;

    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly CourierContext _context;
        //private readonly IPaginationService _paginationService;
        private readonly IQueryService _queryService;

        private readonly PasswordHasher<Admin> _passwordHasher;

        public AdminController(CourierContext context, IQueryService queryService)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Admin>();
            //_paginationService = paginationService;
            _queryService = queryService;
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

            var result = await _queryService.QueryAsync(
                query,
                parameters,
                AdminResource.FromModel,

                // SEARCH LOGIC
                (q, search) => q.Where(a =>
                    a.Name.ToLower().Contains(search.ToLower()) ||
                    a.Email.ToLower().Contains(search.ToLower()) ||
                    a.PhoneNumber.ToLower().Contains(search.ToLower())
                )//,

                // FILTER LOGIC
                //(q, filters) =>
                //{
                //    if (filters.ContainsKey("role"))
                //        q = q.Where(a => a.RoleType == filters["role"]);

                //    return q;
                //}
            );

            return Ok(result);
        }

        // GET: api/Admin/5
        [HttpGet("{id}")]
        public ActionResult<Admin> GetAdmin(int id)
        {
            var admin = _context.Admins.Find(id);

            if (admin == null)
            {
                return NotFound();
            }

            return admin;
        }

        // POST: api/Admin
        [HttpPost]
        public ActionResult<Admin> PostAdmin(Admin request)
        {

            if (request == null)
            {
                return BadRequest(new { message = "Invalid request" });
            }

            // Hash password
            request.PasswordHash = _passwordHasher.HashPassword(request, request.PasswordHash);

            _context.Admins.Add(request);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAdmin), new { id = request.Id }, new
            {
                message = "Admin created successfully",
                data = new
                {
                    request.Id,
                    request.Name,
                    request.Email,
                    request.PhoneNumber,
                    request.Role
                }
            });
        }
    }
}