namespace EventSystem.Domain.Entities;

public class User
{
    public long UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string Salt { get; set; }

    public long RoleId { get; set; }
    public UserRole Role { get; set; }

    public long? ConfirmerId { get; set; }
    public UserConfirme? Confirmer { get; set; }

    public ICollection<Event> CreatedEvents { get; set; }
    public ICollection<EventSubscriber> SubscribedEvents { get; set; }
    public ICollection<EventGuest> GuestEvents { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
}
