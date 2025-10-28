namespace ChatApp.Application.DTOs.Common;

public class UserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public bool IsAdmin { get; set; }
}