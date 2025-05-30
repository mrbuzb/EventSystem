namespace EventSystem.Application.Dtos;

public class RefreshRequestDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
