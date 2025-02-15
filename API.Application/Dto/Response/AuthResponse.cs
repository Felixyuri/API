public class AuthResponse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public AuthResponse(int id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

    public static AuthResponse ToApi(User user)
    {
        return new AuthResponse(
            id: user.Id,
            name: user.Username
        );
    }
}