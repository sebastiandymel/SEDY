using System;
using Microsoft.AspNetCore.Mvc;

namespace ServerAPI.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        private static DateTime? _firstRequest;

        [HttpGet("status")]        
        public IActionResult Status() => Ok();

        [HttpGet("ready")]
        public IActionResult Ready()
        {
            if (!_firstRequest.HasValue)
            {
                _firstRequest = DateTime.Now;
                return StatusCode(500);
            }

            if (DateTime.Now - _firstRequest.Value < TimeSpan.FromMinutes(5))
            {
                return StatusCode(500);
            }
            return StatusCode(200);
        }
    }
}
