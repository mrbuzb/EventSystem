using System.Net;
using System.Net.Mail;
using EventSystem.Application.Dtos;
using EventSystem.Application.Interfaces;
using EventSystem.Core.Errors;
using EventSystem.Domain.Entities;
using FluentEmail.Core;
using FluentEmail.Smtp;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EventSystem.Application.Services;

public class EventService(IUserRepository _userRepo ,IEventRepository _eventRepo, IValidator<EventCreateDto> _createDtoValidator, IValidator<EventUpdateDto> _updateDtoValidator) : IEventService
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
        var eventId =  await _eventRepo.AddEventAsync(entity);

        var foundEvent = await _eventRepo.GetEventByIdAsync(eventId, userId);

        if (eventCreateDto.GuestUsers is not null && eventCreateDto.GuestUsers.Count() >=1)
        {
            var user = await _userRepo.GetUserByIdAync(userId);
            var existEmails = new List<string>();

            foreach (var email in eventCreateDto.GuestUsers)
            {
                var foundUserId = await _userRepo.CheckEmailExistsAsync(email.Email);
                if (foundUserId is not null&&user.Confirmer.Gmail != email.Email)
                {
                    existEmails.Add(email.Email);
                    foundEvent.Guests.Add(new EventGuest() { EventId = eventId,UserId = foundUserId.Value});
                }
            }

            foreach(var email in existEmails)
            {
                await SendNotification(email, $"{user.FirstName}  {user.LastName}");
                foundEvent.Subscribers++;
            }
        }
        else
        {
            return eventId;
        }
        await _eventRepo.UpdateEventAsync(foundEvent);
        return eventId;
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
        var eventById = await _eventRepo.GetEventByIdAsync(eventId, userId);
        var result =  Converter(eventById);
        result.IsSubscribed = eventById.SubscribedUsers.Any(u=>u.UserId == userId);
        result.CreatedBy = eventById.Creator.UserName;
        return result;
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

    public async Task<List<EventGetDto>> GetAllPublicEvents()
    {
        var events = await _eventRepo.GetAllPublicEvents();
        return events.Select(x=>Converter(x)).ToList();
    }

    public async Task<int> SubscribeEvent(long eventId, long userId)
    {
        return await _eventRepo.SubscribeEvent(eventId, userId);
    }

    public async Task<int> UnSubscribeEvent(long eventId, long userId)
    {
        return await _eventRepo.UnSubscribeEvent(eventId, userId);
    }

    private async Task SendNotification(string gmail,string inviter)
    {
        var sender = new SmtpSender(() => new SmtpClient("smtp.gmail.com")
        {
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Port = 587,
            Credentials = new NetworkCredential("qahmadjon11@gmail.com", "nhksnhhxzdbbnqdw")
        });

        Email.DefaultSender = sender;


        var sendResponse = await Email
            .From("qahmadjon11@gmail.com")
            .To(gmail)
            .Subject("Notification")
            .Body($"User {inviter} invite you his event")
            .SendAsync();
    }
}
