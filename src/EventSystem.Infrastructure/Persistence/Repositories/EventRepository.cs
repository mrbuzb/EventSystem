using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public async Task DeleteEventAsync(long eventId,long userId)
    {
        var contact = await GetEventByIdAsync(eventId, userId);
        _context.Events.Remove(contact);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Event>> GetAllEvents(long userId) => await _context.Events.Where(_ => _.CreatorId == userId).ToListAsync();

    public async Task<List<EventGuest>> GetAllGuestedEvents(long userId)
    {
        return await _context.EventGuest.Include(_=>_.Event).Where(u=>u.UserId == userId).ToListAsync();
    }

    public async Task<Event> GetEventByIdAsync(long eventId, long userId)
    {
        var eventEntity = await _context.Events.FirstOrDefaultAsync(x => x.Id == eventId);
        if (eventEntity is null || eventEntity.CreatorId != userId)
        {
            throw new ForbiddenException("User id not allowed or entity not found");
        }
        return eventEntity;
    }

    public async Task UpdateEventAsync(Event updatedEvent)
    {
        _context.Events.Update(updatedEvent);
        await _context.SaveChangesAsync();
    }
}
