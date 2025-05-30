using System.Threading.Tasks;
using HospitalApp.Web.HospitalApi.Models.DTOs;
using HospitalApp.Web.HospitalApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalApp.Web.HospitalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(UserRegisterDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.Register(userDto);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(UserLoginDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var response = await _authService.Login(userDto);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
