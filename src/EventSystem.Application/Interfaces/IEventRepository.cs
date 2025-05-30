using EventSystem.Domain.Entities;

namespace EventSystem.Application.Interfaces;

public interface IEventRepository
{
    Task<long> AddEventAsync(Event eventEntity);
    Task<Event> GetEventByIdAsync(long eventId,long userId);
    Task<List<Event>> GetAllEvents(long userId);
    Task UpdateEventAsync(Event updatedEvent);
    Task DeleteEventAsync(long eventId, long userId);
    Task<List<EventGuest>> GetAllGuestedEvents(long userId);
}