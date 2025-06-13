using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem.Domain.Entities;

public class EventSubscriber
{
        public long EventId { get; set; }
        public Event Event { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }

        public DateTime SubscribedAt { get; set; }
}
