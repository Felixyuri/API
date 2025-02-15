using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using Xunit;

public class AuthServiceUnitTests
{
    private readonly AppDbContext _dbContext;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthService _authService;

    public AuthServiceUnitTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _dbContext = new AppDbContext(options);

        _mockConfiguration = new Mock<IConfiguration>();

        _authService = new AuthService(_dbContext, _mockConfiguration.Object);
    }

    [Fact]
    public async Task Register_SaveUser()
    {
        var user = new AuthRequest { Username = "testuser", Password = "password" };

        await _authService.Register(user);

        var savedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
        Assert.NotNull(savedUser);
        Assert.True(BCrypt.Net.BCrypt.Verify("password", savedUser.Password));
    }

    [Fact]
    public async Task Login_GenerateJWTToken()
    {
        var user = new AuthRequest { Username = "testuser", Password = "password" };
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password");

        _dbContext.Users.Add(new User { Username = "testuser", Password = hashedPassword });
        await _dbContext.SaveChangesAsync();

        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("gI9nX3l5MwQrPz2J7rX2zFvY8JpJmW8L2VhQZQ9D4Jg=");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("testIssuer");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("testAudience");

        var token = await _authService.Login(user);

        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task CredentialsAreInvalid()
    {
        var user = new AuthRequest { Username = "testuser", Password = "wrongpassword" };
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password");

        _dbContext.Users.Add(new User { Username = "testuser", Password = hashedPassword });
        await _dbContext.SaveChangesAsync();

        var token = await _authService.Login(user);

        Assert.Empty(token);
    }
}