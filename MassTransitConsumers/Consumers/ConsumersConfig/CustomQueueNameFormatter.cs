using MassTransit;

namespace MassTransitConsumers.Consumers.ConsumersConfig;

public class CustomDeadLetterQueueNameFormatter : IDeadLetterQueueNameFormatter
{
    public string FormatDeadLetterQueueName(string queueName)
    {
        return queueName + "-dlq";
    }
}

public class CustomErrorQueueNameFormatter : IErrorQueueNameFormatter
{
    public string FormatErrorQueueName(string queueName)
    {
        return queueName + "-dlq";
    }
}