namespace CatalogoZap.Models;

public record ProfileModel
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Bio { get; set; }
    public required string Phone { get; set; }
    public required string LogoUrl { get; set; }
    public required string CreatedAt { get; set; }
    public required string Email { get; set; }
    public bool Premium { get; set; }
    public required string Password { get; set; }
}
