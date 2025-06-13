using EventSystem.Application.Interfaces;
using EventSystem.Core.Errors;
using EventSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventSystem.Infrastructure.Persistence.Repositories;

public class EventRepository(AppDbContext _context) : IEventRepository
{
    public async Task<long> AddEventAsync(Event eventEntity)
    {
        await _context.Events.AddAsync(eventEntity);
        await _context.SaveChangesAsync();
        return eventEntity.Id;
    }

    public async Task DeleteEventAsync(long eventId, long userId)
    {
        var contact = await GetEventByIdAsync(eventId, userId);
        _context.Events.Remove(contact);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Event>> GetAllEvents(long userId) => await _context.Events.Where(_ => _.CreatorId == userId).ToListAsync();

    public async Task<List<EventGuest>> GetAllGuestedEvents(long userId)
    {
        return await _context.EventGuest.Include(_ => _.Event).Where(u => u.UserId == userId).ToListAsync();
    }

    public async Task<List<Event>> GetAllPublicEvents()
    {
        return await _context.Events.Where(x=>x.Type == Domain.Entities.Type.Public).ToListAsync();
    }

    public async Task<Event> GetEventByIdAsync(long eventId, long userId)
    {
        var eventEntity = await _context.Events.Include(x => x.Guests).FirstOrDefaultAsync(x => x.Id == eventId);
        if (eventEntity is null || eventEntity.CreatorId != userId)
        {
            throw new ForbiddenException("User id not allowed or entity not found");
        }
        return eventEntity;
    }

    public async Task<int> SubscribeEvent(long eventId, long userId)
    {

        var foundEvent = await _context.Events.Include(e => e.SubscribedUsers).Include(e => e.Guests).FirstOrDefaultAsync(_ => _.Id == eventId);

        if (foundEvent is null || foundEvent.Subscribers >= foundEvent.Capasity || foundEvent.Date < DateTime.Now || foundEvent.CreatorId == userId)
        {
            throw new EntityNotFoundException("Event not found or expired");
        }

        if (foundEvent.Type == Domain.Entities.Type.Private && !foundEvent.Guests.Any(gev => gev.UserId == userId))
        {
            throw new NotAllowedException("You are not in the guests list");
        }

        if (foundEvent.SubscribedUsers.Any(u => u.UserId == userId))
        {
            throw new NotAllowedException("You are already subscribed");
        }

        await _context.EventSubscribers.AddAsync(new EventSubscriber() {EventId = foundEvent.Id,UserId = userId ,SubscribedAt = DateTime.Now});

        foundEvent.Subscribers++;

        await UpdateEventAsync(foundEvent);

        return foundEvent.Capasity - foundEvent.Subscribers;
    }
    public async Task<int> UnSubscribeEvent(long eventId, long userId)
    {
        var foundEvent = await _context.Events.Include(e => e.SubscribedUsers).Include(e => e.Guests).FirstOrDefaultAsync(_ => _.Id == eventId);

        if (foundEvent is null || foundEvent.Subscribers >= foundEvent.Capasity || foundEvent.Date < DateTime.Now || foundEvent.CreatorId == userId)
        {
            throw new EntityNotFoundException("Event not found or expired");
        }
        var userSubs =  foundEvent.SubscribedUsers.FirstOrDefault(u => u.UserId == userId);
        if (userSubs is null)
        {
            throw new NotAllowedException($"Not subscribed");
        }
        else
        {
            var eventSubscriber = _context.EventSubscribers.FirstOrDefault(u => u.UserId == userId && u.EventId == foundEvent.Id);
            foundEvent.Subscribers--;
            _context.EventSubscribers.Remove(userSubs);
            await _context.SaveChangesAsync();
        }
        await UpdateEventAsync(foundEvent);
        return foundEvent.Capasity-foundEvent.Subscribers;
    }

    public async Task UpdateEventAsync(Event updatedEvent)
    {
        _context.Events.Update(updatedEvent);
        await _context.SaveChangesAsync();
    }
}
