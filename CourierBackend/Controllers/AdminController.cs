namespace CourierBackend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CourierBackend.Data;
    using CourierBackend.Models;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly CourierContext _context;

        public AdminController(CourierContext context)
        {
            _context = context;
        }

        // GET: api/Admin
        [HttpGet]
        public ActionResult<IEnumerable<Admin>> GetAdmins()
        {
            return _context.Admins.ToList();
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
        public ActionResult<Admin> PostAdmin(Admin admin)
        {
            _context.Admins.Add(admin);
            _context.SaveChanges();

            return CreatedAtAction("GetAdmin", new { id = admin.Id }, admin);
        }
    }
}