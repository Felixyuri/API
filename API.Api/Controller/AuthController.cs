using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] AuthRequest user) {
        var existingUser = await _authService.GetUserByName(user);

        if (existingUser != null) {
            return Conflict();
        }

        await _authService.Register(user);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] AuthRequest user) {
        var token = await _authService.Login(user);

        if(token != string.Empty) {
            return Ok(new { Token = token });
        }

        return Unauthorized();
    }
}