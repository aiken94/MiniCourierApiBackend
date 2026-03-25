namespace CourierBackend.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using CourierBackend.Data;
    using CourierBackend.Models;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly CourierContext _context;

        public PackageController(CourierContext context)
        {
            _context = context;
        }

        // GET: api/Package
        [HttpGet]
        public ActionResult<IEnumerable<Package>> GetPackages()
        {
            return _context.Packages.ToList();
        }

        // GET: api/Package/5
        [HttpGet("{id}")]
        public ActionResult<Package> GetPackage(int id)
        {
            var package = _context.Packages.Find(id);

            if (package == null)
            {
                return NotFound();
            }

            return package;
        }

        // POST: api/Package
        [HttpPost]
        public ActionResult<Package> PostPackage(Package package)
        {
            _context.Packages.Add(package);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetPackage), new { id = package.Id }, package);
        }
    }
}