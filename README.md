
<!-- ABOUT THE PROJECT -->
## About The Project
OutboxService is an open source event logger and exporter tool developed by Trendyol .

### Features

- Writes events to specified sql database table and exports them
- Supports Kafka and RabbitMQ message brokers
- Easily integrable 

#### The Why?
 A year ago we were using only RabbitMQ broker and we had an incident where the broker was unreachable and we lost events. It was then that we decided that we need to log our events in case of a disaster. 

#### The How? 
- Body holds the event payload as json. 
- EventId should be unique for the convenience. For example if you are logging OrderCreated event you can set the OrderCode as EventId. 
- Exchange specifies the event name. 
- StatusId specifies the state of the event, if the event is Exported the StatusId will be 3, the DeliveredDate and DeliveryCount will be set accordingly. If you want to re-export the event you need to set StatusId = 1,  DeliveredDate and DeliveryCount as null. 
- BrokerTypeId can be 1 for RabbitMQ and 2 for Kafka. 
- PickedDate specifies the date the event was logged. 


Database Table Sample: 
```json 
[Id] [bigint] NOT NULL IDENTITY(1, 1),

[EventId] [nvarchar] (200) NOT NULL,

[Body] [ntext] NOT NULL,

[Exchange] [nvarchar] (1000) NOT NULL,

[StatusId] [tinyint] NOT NULL,

[BrokerTypeId] [BIGINT] NULL,

[PickedDate] [datetime2] NULL,

[DeliveredDate] [datetime2] NULL,

[DeliveryCount] [tinyint] NULL,

[CreatedBy] [nvarchar] (200) NULL,

[CreatedDate] [datetime2] (3) NULL
```

### Built With

* [.NET 5](https://www.google.com)
* [Dapper](https://www.google.com)
* [Kafka](https://www.google.com)
* [RabbitMQ.Client](https://www.google.com)
* [NLog](https://www.google.com)

<!-- GETTING STARTED -->
## Getting Started

To use OutboxService follow these simple steps.


<!-- USAGE EXAMPLES -->
## Usage


```c#
public class OutboxService : IOutboxService
{
    private readonly IOutboxRepository _outboxRepository;
    
    public OutboxService(IOutboxRepository outboxRepository)
    {
        _outboxRepository = outboxRepository;
    }
    
    public Outbox CreateOutboxEntry<T>(T @event, string eventId, string createdBy)
    {
        var outboxEntry = new Outbox
        {
            EventId = eventId,
            Body = @event.ToJson(),
            Exchange = GenerateExchangeName<T>(),
            CreatedBy = createdBy,
            CreatedDate = DateTime.Now,
            StatusId = OutboxStatuses.Entered
        };

        return _outboxRepository.Insert(outboxEntry);
    }
    
    private static string GenerateExchangeName<T>()
    {
        return typeof(T).Namespace + ":" + typeof(T).Name;
    }
}
```

After creating the service, when publishing the event we use:
```c#
_outboxService.CreateOutboxEntry(new PlanCreated()
{
    PlanId = plan.Id,
    PlanDetailId = planDetail.Id
}, plan.Id.ToString(), plan.CreatedBy);
```



<!-- ROADMAP -->
## Roadmap

See the [open issues](https://github.com/github_username/repo_name/issues) for a list of proposed features (and known issues).



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.



<!-- CONTACT -->
## Contact

Fatiha Beqirovski - fatihabeqirovski@hotmail.com

Mustafa İkili - mustafaikili@hotmail.com

Semih Çavdar - semihcavdar@hotmail.com
