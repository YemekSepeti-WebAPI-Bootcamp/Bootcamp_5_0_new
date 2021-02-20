using System;
using System.Threading.Tasks;
using Hotels.API.Models;
using Hotels.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotels.API.Controllers
{
    [Authorize]
    [Route("/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost(Name = nameof(Authenticate))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] TokenRequest req)
        {
            if(req == null)
                return BadRequest();
            
            var result = await _userService.Authenticate(req);
            if(result == null)
                return Unauthorized();
            
            return Ok(result);
        }

    }
}
