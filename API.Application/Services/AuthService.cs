
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration) {
        _context = context;
        _configuration = configuration;
    }

    public async Task Register(AuthRequest user) {
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        var newUser = new User {
            Username = user.Username,
            Password = user.Password
        };

        _context.Users.Add(newUser);

        await _context.SaveChangesAsync();
    }

    public async Task<string> Login(AuthRequest user) {
        var existingUser = await GetUserByName(user);

        if (existingUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password)) {
            return string.Empty;
        }

        var token = GenerateJwtToken(existingUser);

        return token;
    }

    public async Task<User?> GetUserByName(AuthRequest user) {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
        
        return existingUser ?? null;
    }

    private string GenerateJwtToken(User user) {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "gI9nX3l5MwQrPz2J7rX2zFvY8JpJmW8L2VhQZQ9D4Jg="));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: new[] { new Claim(ClaimTypes.Name, user.Username) },
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}