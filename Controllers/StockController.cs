using Microsoft.AspNetCore.Mvc;
using StockSentinal.DTOs;
using StockSentinal.Services;

namespace StockSentinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _service;
        public StockController(IStockService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<StockResponse>>> GetAllStock()
        {
            var result = await _service.GetAllStocks();
            if (!result.Any())
            {
                return NotFound("Stock details not found");
            }
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<StockResponse>> SearchStockBySymbol([FromQuery] string symbol)
        {
            var result = await _service.SearchStockBySymbol(symbol);
            if(result == null)
            {
                return NotFound("No Stock found for symbol:"+symbol);
            }
            return Ok(result);
        }

        [HttpGet("price")]
        public async Task<ActionResult<decimal>> StockPriceBySymbol([FromQuery] string symbol)
        {
            var result = await _service.GetStockPrices(symbol);
            return Ok(result);
        }
    }
}

