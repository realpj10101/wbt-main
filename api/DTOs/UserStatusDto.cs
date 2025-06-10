namespace api.DTOs;

public class UserStatusDto
{
    public string UserName { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
}