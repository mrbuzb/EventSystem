using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem.Domain.Entities;

public class Event
{
    public long Id { get; set; }
    public Type Type { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    public int Capasity { get; set; }

    public long CreatorId { get; set; }      
    public User Creator { get; set; }

    public ICollection<EventGuest> Guests { get; set; }
}
