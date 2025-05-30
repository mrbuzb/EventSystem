using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem.Application.Dtos;

public class EventUpdateDto
{
    public long Id { get; set; }
    public TypeDto Type { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    public int Capasity { get; set; }
}
