using EventSystem.Application.Dtos;
using EventSystem.Application.Interfaces;
using EventSystem.Core.Errors;
using EventSystem.Domain.Entities;
using FluentValidation;

namespace EventSystem.Application.Services;

public class EventService(IEventRepository _eventRepo, IValidator<EventCreateDto> _createDtoValidator, IValidator<EventUpdateDto> _updateDtoValidator) : IEventService
{

    private Event Converter(EventCreateDto dto)
    {
        return new Event
        {
            Capasity = dto.Capasity,
            Type = (Domain.Entities.Type)dto.Type,
            Date = dto.Date,
            Description = dto.Description,
            Location = dto.Location,
            Title = dto.Title,
        };
    }

    private EventGetDto Converter(Event eventEntity)
    {
        return new EventGetDto
        {
            Title = eventEntity.Title,
            Location = eventEntity.Location,
            Description = eventEntity.Description,
            Date = eventEntity.Date,
            Id = eventEntity.Id,
            CreatorId = eventEntity.CreatorId,
            Capasity = eventEntity.Capasity,
            Type = (TypeDto)eventEntity.Type,
        };
    }

    public async Task<long> AddEventAsync(EventCreateDto eventCreateDto, long userId)
    {
        var resultOfValidator = _createDtoValidator.Validate(eventCreateDto);
        if (!resultOfValidator.IsValid)
        {
            string errorMessages = string.Join("; ", resultOfValidator.Errors.Select(e => e.ErrorMessage));
            throw new NotAllowedException(errorMessages);
        }
        var entity = Converter(eventCreateDto);
        entity.CreatorId = userId;
        return await _eventRepo.AddEventAsync(entity);
    }

    public async Task DeleteEventAsync(long eventId, long userId)
    {
        await _eventRepo.DeleteEventAsync(eventId, userId);
    }

    public async Task<List<EventGetDto>> GetAllEvents(long userId)
    {
        var events = await _eventRepo.GetAllEvents(userId);
        return events.Select(Converter).ToList();
    }

    public async Task<List<EventGetDto>> GetAllGuestedEvents(long userId)
    {
        var guestedEvents = await _eventRepo.GetAllGuestedEvents(userId);
        return guestedEvents.Select(x => Converter(x.Event)).ToList();
    }

    public async Task<EventGetDto> GetEventByIdAsync(long eventId, long userId)
    {
        return Converter(await _eventRepo.GetEventByIdAsync(eventId, userId));
    }

    public async Task UpdateEventAsync(EventUpdateDto eventUpdateDto, long userId)
    {
        var foundEvent = await _eventRepo.GetEventByIdAsync(eventUpdateDto.Id, userId);
        foundEvent.Type = (Domain.Entities.Type)eventUpdateDto.Type;
        foundEvent.Date = eventUpdateDto.Date;
        foundEvent.Description = eventUpdateDto.Description;
        foundEvent.Location = eventUpdateDto.Location;
        foundEvent.Title = eventUpdateDto.Title;
        foundEvent.Capasity = eventUpdateDto.Capasity;
        await _eventRepo.UpdateEventAsync(foundEvent);
    }
}
