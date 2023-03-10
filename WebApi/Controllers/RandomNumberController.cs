using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RandomNumberController : ControllerBase
    {

        [HttpGet()]
        public IActionResult Get()
        {
            var randomNumber = new Random().Next(maxValue: 100);

            if (randomNumber < 25) //25% of an error response
                return Problem("Random error to test the client's resiliency");

            return Ok(randomNumber);
        }
    }
}
