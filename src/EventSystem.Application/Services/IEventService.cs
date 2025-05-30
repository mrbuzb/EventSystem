using EventSystem.Application.Dtos;
using EventSystem.Domain.Entities;

namespace EventSystem.Application.Services;

public interface IEventService
{
    Task<long> AddEventAsync(EventCreateDto eventCreateDto,long userId);
    Task<EventGetDto> GetEventByIdAsync(long eventId, long userId);
    Task<List<EventGetDto>> GetAllEvents(long userId);
    Task UpdateEventAsync(EventUpdateDto updatedEvent,long userId);
    Task DeleteEventAsync(long eventId, long userId);
    Task<List<EventGetDto>> GetAllGuestedEvents(long userId);
}