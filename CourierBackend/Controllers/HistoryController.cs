namespace CourierBackend.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using CourierBackend.Models;
    using Microsoft.AspNetCore.Mvc;
    using CourierBackend.Data.Resources;
    using CourierBackend.Data.Requests;
    using FluentValidation;
    using CourierBackend.Helpers;
    using CourierBackend.Services.Model.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IValidator<DeliveryHistoryRequest> _validator;

        private readonly IDeliveryHistoryService _deliveryHistoryService;

        public HistoryController(IValidator<DeliveryHistoryRequest> validator, IDeliveryHistoryService deliveryHistoryService)
        {
            _validator = validator;
            _deliveryHistoryService = deliveryHistoryService;
        }

        [HttpGet("package/{id}")]
        public async Task<IActionResult> GetHistories(int id)
        {
            var results = await _deliveryHistoryService.GetHistoriesAsync(id);

            return Ok(results);
        }

        // POST: api/Admin
        [HttpPost("create")]
        public async Task<IActionResult> CreateHistory(DeliveryHistoryRequest request)
        {
            var result = await _validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                return BadRequest(ResponseStructures.ErrorResponse(result.Errors.ToDictionary()));
            }

            PackageDeliveryHistory history = await _deliveryHistoryService.CreateAsync(request);

            DeliveryHistoryResource resource = DeliveryHistoryResource.FromModel(history);

            return CreatedAtAction(nameof(GetHistories), new { id = request.PackageId }, ResponseStructures.SuccessResponse(resource));
        }

        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> DeleteHistory(int id)
        {
            bool status = await _deliveryHistoryService.DeleteAsync(id);

            if (status)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}