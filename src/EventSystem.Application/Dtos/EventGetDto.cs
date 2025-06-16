namespace EventSystem.Application.Dtos;

public class EventGetDto
{
    public long Id { get; set; }
    public TypeDto Type { get; set; }
    public string CreatedBy { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    public int Capasity { get; set; }
    public int SubscribersCount { get; set; }
    public long CreatorId { get; set; }
    public bool IsSubscribed { get; set; }
}
