using Messaging.Infrastructure.Contracts.QueueMessage.Commands;
using Messaging.Infrastructure.Services.MessageQueue;

namespace Messaging.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        //private readonly IMessageQueueService _queueService;
        //public Worker(ILogger<Worker> logger, IMessageQueueService queueService)
        //{
        //    _logger = logger;
        //    _queueService = queueService;
        //}

        private readonly IServiceProvider _serviceProvider;
        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Get the current time
                //var currentTime = DateTime.Now;

                //// Set the desired time for the task to run (e.g., 23:00)
                //var scheduledTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 23, 0, 0);

                //// Calculate the time until the next execution
                //var delay = scheduledTime > currentTime
                //    ? scheduledTime - currentTime
                //    : scheduledTime.AddDays(1) - currentTime;

                //// Wait until the scheduled time
                //await Task.Delay(delay, stoppingToken);

                await Task.Delay(30_000,stoppingToken);

                if (!stoppingToken.IsCancellationRequested)
                {
                    //Call your service method here
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var messageQueueService = scope.ServiceProvider.GetRequiredService<IMessageQueueService>();
                        _logger.LogInformation("requesting a dequeue of 5 messages");
                        await messageQueueService.DequeueMessageAsync(new DequeueMessageCommand(1,new TimeSpan(0,10,0)), stoppingToken);
                        _logger.LogInformation("finished requesting a dequeue of 5 messages");
                    }


                    //_logger.LogInformation("requesting a dequeue of 5 messages");
                    //await _queueService.DequeueMessageAsync(new DequeueMessageCommand(1), stoppingToken);
                    //_logger.LogInformation("finished requesting a dequeue of 5 messages");

                }


            }
            _logger.LogCritical("worker shutting down");
            
        }
    }
}