using core.application.Contract.API.Mapper;
using core.domain.entity.EnjoyEventModels;
using core.domain.entity.enums;

namespace core.test.Core.Application.Contract.API.Mapper;

public class EnjoyEventMapperTest
{
    [Theory]
    [InlineData(30,18,1,2,5,3,"18/30",true,12,1,1,1,3)]
    [InlineData(30,18,1,2,5,5,"18/30",true,12,1,1,1,3)]
    [InlineData(30,18,0,0,5,5,"18/30",false,12,1,1,1,3)]
    [InlineData(30, 18, 0, 0, 5, 0, "18/30", false, 12, 1, 1, 1,3)]
    public void ConvertSessionToSessionDTO_ReturnsCorrectCapacityData(int totalCapacity,
                                                            int totalNumOfReserved,
                                                              int numOfFemale,
                                                              int numOfMale,
                                                              int unitsAlowedNum,
                                                              int unitsUsedTicket,
                                                              string resultCapacity,
                                                              bool isReserved,
                                                              int resultRemainingCapacity,
                                                              int numOfGuestFemale,
                                                             int numOfGuestMale,
                                                             int unitsGuestUsedTicket,
                                                             int unitsGuestAllowedNum)
    {

        //Arrange
        var baseTime = DateTime.Now;

        EventSessionModel sessionModel = new EventSessionModel()
        {
            Id = 1,
            Event = new EventModel(),
            StartTime = baseTime.AddDays(1).AddHours(1),
            EndTime = baseTime.AddDays(1).AddHours(2),
            Capacity = totalCapacity,
            GenderType = GenderType.ALL,
            Place = "place",
            Tickets = new List<EventTicketModel>(),
        };

        //Act
        var result = sessionModel.ConvertSessionToSessionDTO(totalNumOfReserved,
                                                             numOfFemale,
                                                             numOfMale,
                                                             unitsAlowedNum,
                                                             unitsUsedTicket,
                                                             numOfGuestFemale,
                                                             numOfGuestMale,                                                             
                                                             unitsGuestUsedTicket,
                                                             unitsGuestAllowedNum);
        //Assert

        Assert.NotNull(result);
        Assert.Equal(isReserved, result.IsReserved);
        Assert.Equal(resultCapacity, result.Capacity);
        Assert.Equal(resultRemainingCapacity, result.RemainingCapacity);
        Assert.Equal(totalCapacity, result.TotalCapacity);
        Assert.Equal(unitsAlowedNum, result.Ticket);
        Assert.Equal(numOfMale, result.MenCount);
        Assert.Equal(numOfFemale, result.WomenCount);
        Assert.Equal(unitsUsedTicket,result.UnitUsedTicket);

    }
    [Theory]
    [InlineData(0,EventState.COMMINGSOON)]
    [InlineData(2, EventState.ONGOING)]
    [InlineData(4, EventState.ARCHIVED)]
    public void ConvertEnjoyEventToGetEnjoyEventDetailDTO_ReturnsCorrectEventContent(int baseTimeOffset,
                                                                                       EventState resultBodyType)
    {
        //Arrange
        var now = DateTime.Now;
        //var baseTime = now.AddDays(baseTimeOffset);
        EventModel eventModel = new()
        {
            Id = 1,
            ComplexId = 1,
            Name = "",
            ReservationStartDate = now.AddDays(1-baseTimeOffset),
            PublishDate = now.AddDays(-1-baseTimeOffset),
            StartDate = now.AddDays(2-baseTimeOffset),
            EndDate = now.AddDays(3-baseTimeOffset),
            LockTimeout = TimeSpan.FromHours(8),
            WebsiteUrl = "",
            SupportPhoneNumber = "",
            EventContent = new(),
            EventSession = new(),
            OwnersMaxReservations = 1,
            MandatoryConsecutiveReservation = true,
            repeatSessionReservation = true,
            SessionDescription = "",
            IsPinned = true,
            CreatedDate = DateTime.Now,
            ModifyDate = DateTime.Now,

        };
        EventContentModel commingSoonContent = new()
        {
            BodyType = BodyType.COMMINGSOON,
            ContentBody = "comming soon",
            Id = 1,
            Media = new List<EventMediaModel>(),
            Event = eventModel
        };
        EventContentModel ongoingContent = new()
        {
            BodyType = BodyType.ONGOING,
            ContentBody = "on going",
            Id = 2,
            Media = new List<EventMediaModel>(),
            Event = eventModel
        };
        EventContentModel archivedContent = new()
        {
            BodyType = BodyType.ARCHIVED,
            ContentBody = "archived",
            Id = 3,
            Media = new List<EventMediaModel>(),
            Event = eventModel
        };

        eventModel.EventContent.Add(commingSoonContent);
        eventModel.EventContent.Add(ongoingContent);
        eventModel.EventContent.Add(archivedContent);

        //Act
        var result = eventModel.ConvertEnjoyEventToGetEnjoyEventDetailDTO(1, new());

        //Assert
        Assert.NotNull(result);
        Assert.Equal(resultBodyType, result.Status);
    }
}
