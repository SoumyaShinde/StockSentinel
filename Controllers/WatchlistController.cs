using Microsoft.AspNetCore.Mvc;
using StockSentinal.Data;
using StockSentinal.DTOs;
using StockSentinal.Services;

namespace StockSentinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WatchlistController : ControllerBase
    {
        private readonly IWatchlistService _service;

        public WatchlistController(IWatchlistService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<WatchlistResponse>>> GetWatchlist([FromQuery] int userId)
        {
            var result = await _service.GetWatchList(userId);
            if (result.Count == 0) 
            {
                return NotFound("Not Stock added to the WatchList!");
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<WatchlistResponse>> AddWatchList([FromQuery]int stockId, int userId)
        {
            var result = await _service.AddWatchList(userId, stockId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteData(int id)
        {
            var result = await _service.RemoveWatchlist(id);
            return Ok(result);
        }
    }
}

