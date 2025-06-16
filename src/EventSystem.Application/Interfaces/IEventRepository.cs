using EventSystem.Domain.Entities;

namespace EventSystem.Application.Interfaces;

public interface IEventRepository
{
    Task<long> AddEventAsync(Event eventEntity);
    Task<int> SubscribeEvent(long eventId, long userId);
    Task<int> UnSubscribeEvent(long eventId, long userId);
    Task<Event> GetEventByIdAsync(long eventId,long userId);
    Task<List<Event>> GetAllEvents(long userId);
    Task<List<EventSubscriber>> GetAllSubscribedEvents(long userId);
    Task UpdateEventAsync(Event updatedEvent);
    Task DeleteEventAsync(long eventId, long userId);
    Task<List<Event>> GetAllPublicEvents();
    Task<List<EventGuest>> GetAllGuestedEvents(long userId);
}