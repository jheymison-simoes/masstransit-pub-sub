using MassTransit;
using MassTransitConsumers.Consumers;
using MassTransitConsumers.Consumers.ConsumersConfig;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddSwaggerGen();

services.AddMassTransit(opt =>
{
    // Adiciona o consumidor e suas definições
    opt.AddConsumer<ExampleConsumer, ExampleConsumerDefinition>();
    
    opt.AddDelayedMessageScheduler(); // Permite 
    opt.SetKebabCaseEndpointNameFormatter(); // Seta o padrão de nomes para o formato kebab-case

    opt.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration.GetSection("RabbitMq:Host").Value, "/", h =>
        {
            // Conexão com o rabbitMQ
            h.Username(configuration.GetSection("RabbitMq:Username").Value);
            h.Password(configuration.GetSection("RabbitMq:Password").Value);
        });
        
        cfg.SendTopology.DeadLetterQueueNameFormatter = new CustomDeadLetterQueueNameFormatter(); // Altera a nomenclatura da fila de dlq criada automaticamente
        cfg.SendTopology.ErrorQueueNameFormatter = new CustomErrorQueueNameFormatter(); // Altera a nomentaclura da fila de erros criada automaticamente
        
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
