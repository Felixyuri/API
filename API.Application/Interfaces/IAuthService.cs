public interface IAuthService
{
    Task Register(AuthRequest user);
    Task<string> Login(AuthRequest user);

    Task<User?> GetUserByName(AuthRequest user);
}