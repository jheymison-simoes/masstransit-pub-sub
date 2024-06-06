using System.Net.Mime;
using MassTransit;

namespace MassTransitConsumers.Consumers;

public class ExampleConsumer : IConsumer<EventDetails>
{
    /// <summary>
    /// Toda vez que recebe um evento é aqui que é consumida a mensagem
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public Task Consume(ConsumeContext<EventDetails> context)
    {
        var order = context.Message;
        // throw new Exception("Erro ao consumir a mensagem");
        Console.WriteLine($"Order submitted: {order.Name}");
        return Task.CompletedTask; // teste
    }
}

public class ExampleConsumerDefinition : ConsumerDefinition<ExampleConsumer>
{
    /// <summary>
    /// Definição da fila que receberá os eventos
    /// </summary>
    public ExampleConsumerDefinition()
    {
        EndpointName = "mass-transit-queue-example";
    }
    
    /// <summary>
    /// Configura o consumidor fazendo o bind na exchange 
    /// </summary>
    /// <param name="endpointConfigurator"></param>
    /// <param name="consumerConfigurator"></param>
    /// <param name="context"></param>
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator, 
        IConsumerConfigurator<ExampleConsumer> consumerConfigurator, 
        IRegistrationContext context
    )
    {
        endpointConfigurator.ConfigureConsumeTopology = false;
        endpointConfigurator.DefaultContentType = new ContentType("application/json");
        endpointConfigurator.UseRawJsonDeserializer(); // Necessário pois por padrão o masstransit envolve toda a mensagem em um objeto chamando envelop
        endpointConfigurator.UseMessageRetry(r => r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30)));
        if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbit)
        {
            rabbit.Bind("exchange-event-raw", x =>
            {
                x.ExchangeType = "fanout";
                x.RoutingKey = "";
            });
        }
    }
}

public record EventDetails
{
    public string? Name { get; set; }
    public string? Id { get; set; }
}
