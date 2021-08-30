using Microsoft.AspNetCore.Mvc;
using System;

namespace ServiceStub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RandomNumberController : ControllerBase
    {
        [HttpGet()]
        public int Get()
        {
            var randomNumber = new Random().Next();
            Console.WriteLine($"Request received: GET /RandomNumber. Returning random number {randomNumber}");
            return randomNumber;
        }
    }
}
