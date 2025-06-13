//using EventSystem.Application.Dtos;
//using EventSystem.Application.Interfaces;
//using EventSystem.Application.Services;
//using EventSystem.Core.Errors;
//using EventSystem.Domain.Entities;
//using FluentValidation;
//using FluentValidation.Results;
//using Moq;
//using Xunit;

//namespace EventSystem.Tests.Unit.Application;

//public class EventServiceTests
//{
//    private readonly Mock<IEventRepository> _mockRepo;
//    private readonly EventService _service;
//    private readonly Mock<IValidator<EventCreateDto>> _mockCreateValidator;
//    private readonly Mock<IValidator<EventUpdateDto>> _mockUpdateValidator;


//    public EventServiceTests()
//    {
//        _mockRepo = new Mock<IEventRepository>();
//        _mockCreateValidator = new Mock<IValidator<EventCreateDto>>();
//        _mockUpdateValidator = new Mock<IValidator<EventUpdateDto>>();

//        _mockCreateValidator.Setup(v => v.Validate(It.IsAny<EventCreateDto>()))
//                      .Returns(new FluentValidation.Results.ValidationResult());

//        _mockUpdateValidator.Setup(v => v.Validate(It.IsAny<EventUpdateDto>()))
//                      .Returns(new FluentValidation.Results.ValidationResult());


//        _service = new EventService(_mockRepo.Object, _mockCreateValidator.Object, _mockUpdateValidator.Object);
//    }


//    [Fact]
//    public async Task AddEventAsync_ShouldReturnEventId_WhenDtoIsValid()
//    {
//        //Arrange
//        var createDto = new EventCreateDto() { Title = "Test Event" };
//        var fakeEventId = 123;
//        var userId = 1;

//        _mockCreateValidator.Setup(v => v.Validate(createDto)).Returns(new ValidationResult());
//        _mockRepo.Setup(r => r.AddEventAsync(It.IsAny<Event>())).ReturnsAsync(fakeEventId);

//        //Act
//        var result = await _service.AddEventAsync(createDto, userId);

//        //Assert

//        Assert.Equal(result, fakeEventId);
//        _mockRepo.Verify(r => r.AddEventAsync(It.IsAny<Event>()), Times.Once);
//    }

//    [Fact]
//    public async Task AddEventAsync_ShouldThrowNotAllowedException_WhenDtoIsNotValid()
//    {
//        //Arrange
//        var createDto = new EventCreateDto() { Title = "" };
//        var failures = new ValidationFailure[] { new ValidationFailure("Title", "Title is required") };
//        _mockCreateValidator.Setup(v => v.Validate(createDto)).Returns(new ValidationResult(failures));
//        //Act & Assert
//        var ex = await Assert.ThrowsAsync<NotAllowedException>(() => _service.AddEventAsync(createDto, 1));
//        Assert.Contains("Title", "Title is required");
//        _mockRepo.Verify(r => r.AddEventAsync(It.IsAny<Event>()), Times.Never);
//    }


//    [Fact]
//    public async Task GetAllEvents_ShouldReturnMappedEventDtos()
//    {
//        //Arrange
//        var userId = 1;
//        var events = new List<Event>
//    {
//        new Event { Id = 1, Title = "Event 1", Location = "Tashkent", Description = "Test", Date = DateTime.Now, CreatorId = userId, Capasity = 100, Type = Domain.Entities.Type.Public }
//    };

//        var mockRepo = new Mock<IEventRepository>();
//        mockRepo.Setup(r => r.GetAllEvents(userId)).ReturnsAsync(events);
//        var service = new EventService(mockRepo.Object, null, null);

//        //Act
//        var result = await service.GetAllEvents(userId);

//        //Assert
//        Assert.Single(result);
//        Assert.Equal("Event 1", result[0].Title);
//    }


//    [Fact]
//    public async Task DeleteEventAsync_ShouldCallRepositoryWithCorrectParameters()
//    {
//        //Arrange
//        var mockRepo = new Mock<IEventRepository>();
//        var service = new EventService(mockRepo.Object, null, null);
//        //Act
//        await service.DeleteEventAsync(5, 10);
//        //Assert
//        mockRepo.Verify(r => r.DeleteEventAsync(5, 10), Times.Once);
//    }


//    [Fact]
//    public async Task UpdateEventAsync_ShouldUpdateEventFieldsCorrectly()
//    {
//        //Arrange
//        var eventUpdateDto = new EventUpdateDto
//        {
//            Id = 1,
//            Title = "Updated Title",
//            Location = "New Location",
//            Description = "Updated Desc",
//            Date = DateTime.Today.AddDays(1),
//            Capasity = 150,
//            Type = (TypeDto)1
//        };

//        var foundEvent = new Event
//        {
//            Id = 1,
//            Title = "Old Title",
//            Location = "Old Location",
//            Description = "Old Desc",
//            Date = DateTime.Today,
//            Capasity = 100,
//            Type = Domain.Entities.Type.Public
//        };

//        var mockRepo = new Mock<IEventRepository>();
//        //Act & Assert
//        mockRepo.Setup(r => r.GetEventByIdAsync(eventUpdateDto.Id, It.IsAny<long>())).ReturnsAsync(foundEvent);

//        var service = new EventService(mockRepo.Object, null, null);
//        await service.UpdateEventAsync(eventUpdateDto, 1);

//        mockRepo.Verify(r => r.UpdateEventAsync(It.Is<Event>(e =>
//            e.Title == eventUpdateDto.Title &&
//            e.Location == eventUpdateDto.Location &&
//            e.Description == eventUpdateDto.Description &&
//            e.Date == eventUpdateDto.Date &&
//            e.Capasity == eventUpdateDto.Capasity &&
//            e.Type == (Domain.Entities.Type)eventUpdateDto.Type
//        )), Times.Once);
//    }
//}
