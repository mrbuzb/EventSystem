namespace EventSystem.Domain.Entities;

public class UserConfirme
{
    public long ConfirmerId { get; set; }
    public string Gmail { get; set; }
    public bool IsConfirmed { get; set; }
    public string ConfirmingCode { get; set; }
    public DateTime ExpiredDate { get; set; }

    public long UserId { get; set; }
    public User User { get; set; }
}
