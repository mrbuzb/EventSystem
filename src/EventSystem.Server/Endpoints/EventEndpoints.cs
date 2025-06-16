using EventSystem.Application.Dtos;
using EventSystem.Application.Services;
using EventSystem.Core.Errors;
using Microsoft.AspNetCore.Authorization;

namespace EventSystem.Server.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this WebApplication app)
    {
        var userGroup = app.MapGroup("/api/event")
            .WithTags("Event System");

        userGroup.MapPost("/add-event", [Authorize]
        async (EventCreateDto eventCreateDto, HttpContext context, IEventService eventService) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
            {
                throw new ForbiddenException();
            }
            return Results.Ok(await eventService.AddEventAsync(eventCreateDto, long.Parse(userId)));
        })
            .WithName("AddEvent");

        userGroup.MapDelete("/delete-event", [Authorize]
        async (long eventId, HttpContext context, IEventService _service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
            {
                throw new ForbiddenException();
            }
            await _service.DeleteEventAsync(eventId, long.Parse(userId));
            return Results.Ok();
        })
            .WithName("DeleteEvent");

        userGroup.MapPut("/update-event", [Authorize]
        async (EventUpdateDto eventUpdateDto, HttpContext context, IEventService _service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
            {
                throw new ForbiddenException();
            }
            await _service.UpdateEventAsync(eventUpdateDto, long.Parse(userId));
            return Results.Ok();
        })
            .WithName("UpdateEvent");

        userGroup.MapGet("/get-event-by-id", [Authorize]
        async (long eventId, HttpContext context, IEventService _service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
            {
                throw new ForbiddenException();
            }
            var res = await _service.GetEventByIdAsync(eventId, long.Parse(userId));
            return Results.Ok(res);
        })
            .WithName("GetEventById");

        userGroup.MapGet("/get-all-events", [Authorize]
        async (HttpContext context, IEventService _service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
            {
                throw new ForbiddenException();
            }
            var res = await _service.GetAllEvents(long.Parse(userId));
            return Results.Ok(res);
        })
            .WithName("GetAllEvents");

        userGroup.MapGet("/get-all-subscribed-events", [Authorize]
        async (HttpContext context, IEventService _service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
            {
                throw new ForbiddenException();
            }
            var res = await _service.GetAllSubscribedEvents(long.Parse(userId));
            return Results.Ok(res);
        })
            .WithName("GetAllSubscribedEvents");

        userGroup.MapGet("/get-all-public-events",
        async (IEventService _service) =>
        {
            var res = await _service.GetAllPublicEvents();
            return Results.Ok(res);
        })
            .WithName("GetAllPublicEvents");

        userGroup.MapGet("/get-all-guested-events", [Authorize]
        async (HttpContext context, IEventService _service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
            {
                throw new ForbiddenException();
            }
            var res = await _service.GetAllGuestedEvents(long.Parse(userId));
            return Results.Ok(res);
        })
            .WithName("GetAllGuestedEvents");

        userGroup.MapPost("/subscribe-event", [Authorize]
        async (long eventId,HttpContext context, IEventService _service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
            {
                throw new ForbiddenException();
            }
            var res = await _service.SubscribeEvent(eventId,long.Parse(userId));
            return Results.Ok(res);
        })
            .WithName("SubscribeEvent");

        userGroup.MapDelete("/unsubscribe-event", [Authorize]
        async (long eventId,HttpContext context, IEventService _service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
            {
                throw new ForbiddenException();
            }
            var res = await _service.UnSubscribeEvent(eventId,long.Parse(userId));
            return Results.Ok(res);
        })
            .WithName("UnSubscribeEvent");

    }
}
