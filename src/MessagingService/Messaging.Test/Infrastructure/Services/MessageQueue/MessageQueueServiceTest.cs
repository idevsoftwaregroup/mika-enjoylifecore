using Messaging.Infrastructure.Contracts.Message.Commands;
using Messaging.Infrastructure.Contracts.QueueMessage.Commands;
using Messaging.Infrastructure.Domain;
using Messaging.Infrastructure.Services.DeliveryProviders;
using Messaging.Infrastructure.Services.MessageQueue;
using Messaging.Infrastructure.Services.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;

namespace Messaging.Test.Infrastructure.Services.MessageQueue;

public class MessageQueueServiceTest
{
    //[Fact]
    //public async Task DequeueMessageAsync_WhenMessageIsReceived_ProcessesAndAcks()
    //{
    //    // Arrange
    //    var settings = new MessageQueueSettings
    //    {
    //        UriString = "amqp://guest:guest@localhost:5672",
    //        ClientProvidedName = "Messaging API",
    //        ExchangeName = "Messaging Service ExchangeName",
    //        RoutingKey = "Messaging Service RoutingKey",
    //        QueueName = "Messaging Service QueueName"
    //    };

    //    var connectionProviderMock = new Mock<RabbitConnectionProvider>();
    //    var dbContextMock = new Mock<MessagingDBContext>();
    //    var messageDeliveryServiceMock = new Mock<IMessageDeliveryService>();

    //    var cancellationToken = CancellationToken.None;

    //    var messageQueueService = new MessageQueueService(
    //        Options.Create(settings),
    //        connectionProviderMock.Object,
    //        dbContextMock.Object,
    //        messageDeliveryServiceMock.Object
    //    );

    //    // Create a mock message
    //    var mockMessage = new Message
    //    {
    //        Id = 1,
    //        Recipients = new List<Recipient>
    //        {
    //            new Recipient("recipientKey1", false, false),
    //            new Recipient("recipientKey2", false, false),
    //        }
    //    };


    //    // Configure the dbContextMock to return the mock message
    //    dbContextMock.Setup(db => db.Messages.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Message, bool>>>(), cancellationToken))
    //        .ReturnsAsync(mockMessage);

    //    // Configure the messageDeliveryServiceMock to return a mock result
    //    messageDeliveryServiceMock.Setup(service => service.SendInstantMessageAsync(It.IsAny<SendInstantMessageCommand>(), cancellationToken))
    //        .ReturnsAsync(new SendInstantMessageCommandResult(true,false,null));
    //    // Configure the connectionProviderMock to return a mock channel
    //    connectionProviderMock.Setup(provider => provider.GetChannel()).Returns(mockChannel.Object);



    //    // Act
    //    var result = await messageQueueService.DequeueMessageAsync(new DequeueMessageCommand(5), cancellationToken);

    //    // Assert
    //    // Assert that the message has been processed
    //    dbContextMock.Verify(db => db.Messages.Update(It.IsAny<Message>()), Times.Once);
    //    dbContextMock.Verify(db => db.SaveChangesAsync(cancellationToken), Times.Once);
    //    messageDeliveryServiceMock.Verify(service => service.SendInstantMessageAsync(It.IsAny<SendInstantMessageCommand>(), cancellationToken), Times.Exactly(2)); // for each recipient
    //    connectionProviderMock.Verify(provider => provider.GetChannel(), Times.Once);

    //    // Assert the returned result
    //    Assert.NotNull(result);
        
    //    //Asseti the message
    //    Assert.Collection<Recipient>(mockMessage.Recipients,(r) => { Assert.True(r.Sent); },(r) => { Assert.True(r.Sent); } );
    //}
}
