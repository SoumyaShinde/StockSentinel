using Microsoft.AspNetCore.Mvc;
using StockSentinal.DTOs;
using StockSentinal.Services;

namespace StockSentinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _service;

        public AlertController(IAlertService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<AlertResponse>>> GetAlerts([FromQuery] int userId)
        {
            var result = await _service.GetAlert(userId);
            if (!result.Any())
            {
                return NotFound("No alerts configured!");
            }
            return Ok(result);            
        }

        [HttpPost]
        public async Task<ActionResult<AlertResponse>> CreateAlert([FromBody] AlertRequest request)
        {
            var result = await _service.CreateAlert(request);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<ActionResult<AlertResponse>> DeleteAlert([FromQuery] int id)
        {
            var result = await _service.DeleteAlert(id);
            if (!result)
            {
                return new NotFoundResult();
            }
            return Ok("Alert Deleted Successfully!");
        }
    }
}