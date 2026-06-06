namespace CourierBackend.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using CourierBackend.Models;
    using Microsoft.AspNetCore.Mvc;
    using CourierBackend.Data.Resources;
    using CourierBackend.Services;
    using CourierBackend.Data.Requests;
    using FluentValidation;
    using CourierBackend.Helpers;
    using CourierBackend.Services.Model.Interfaces;
    using Microsoft.AspNetCore.Authorization;

    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IValidator<PackageRequest> _validator;

        private readonly IValidator<TrackPackageRequest> _trackValidator;

        private readonly IPackageService _packageService;

        public PackageController(IValidator<PackageRequest> packageValidator, IValidator<TrackPackageRequest> trackValidator, IPackageService packageService)
        {
            _validator = packageValidator;
            _trackValidator = trackValidator;
            _packageService = packageService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPackages([FromQuery] QueryParameters parameters)
        {
            var results = await _packageService.GetPackagesAsync(parameters);

            return Ok(results);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult> GetPackage(int id)
        {
            var package = await _packageService.GetByIdAsync(id);

            if (package == null)
            {
                return NotFound(
                    ResponseStructures._404Response("Package not found")
                );
            }


            PackageResource resource = PackageResource.FromModel(package);

            return Ok(ResponseStructures.SuccessResponse(resource));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> CreatePackage([FromForm] PackageRequest request)
        {
            var result = await _validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                return BadRequest(ResponseStructures.ErrorResponse(result.Errors.ToDictionary()));
            }

            Package package = await _packageService.CreateAsync(request);

            PackageResource resource = PackageResource.FromModel(package);

            return CreatedAtAction(nameof(GetPackage), new { id = package.Id }, ResponseStructures.SuccessResponse(resource));
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> PutPackage([FromForm] PackageRequest request, int id)
        {
            var result = await _validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                return BadRequest(ResponseStructures.ErrorResponse(result.Errors.ToDictionary()));
            }

            var package = await _packageService.GetByIdAsync(id);

            if (package is null)
            {
                return NotFound(
                    ResponseStructures._404Response("Package not found")
                );
            }

            await _packageService.UpdateAsync(request, package);

            PackageResource resource = PackageResource.FromModel(package);

            return Ok(ResponseStructures.SuccessResponse(resource));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePackage(int id)
        {
            var package = await _packageService.GetByIdAsync(id);

            if (package is null)
            {
                return NotFound(
                    ResponseStructures._404Response("Package not found")
                );
            }

            await _packageService.DeleteAsync(package);

            return NoContent();
        }

        [HttpPost("track")]
        public async Task<IActionResult> TrackPackage([FromBody] TrackPackageRequest request)
        {
            var result = await _trackValidator.ValidateAsync(request);

            if (!result.IsValid)
            {
                return BadRequest(ResponseStructures.ErrorResponse(result.Errors.ToDictionary()));
            }

            var package = await _packageService.GetByTrackingNumberAsync(request.TrackingNumber);

            if (package is null)
            {
                return NotFound(
                    ResponseStructures._404Response("Package not found")
                );
            }

            TrackingResource resource = TrackingResource.FromModel(package);

            return Ok(ResponseStructures.SuccessResponse(resource));
        }
    }
}