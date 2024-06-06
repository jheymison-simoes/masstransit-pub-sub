using MassTransit;
using MassTransit.RabbitMqTransport.Topology;
using MassTransit.Transports.Fabric;
using MassTransitTest.Models;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddSwaggerGen();

services.AddMassTransit(opt =>
{
    opt.AddDelayedMessageScheduler();
    opt.SetKebabCaseEndpointNameFormatter();
    
    opt.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration.GetSection("RabbitMq:Host").Value, "/", h =>
        {
            h.Username(configuration.GetSection("RabbitMq:Username").Value);
            h.Password(configuration.GetSection("RabbitMq:Password").Value);
        });
        
        cfg.UseDelayedMessageScheduler();
        
        cfg.Publish<EventDetails>(x =>
        {
            x.ExchangeType = "fanout";
            x.Durable = true;
            x.AutoDelete = false;
        });
        
        cfg.Message<EventDetails>(x => x.SetEntityName("mass-transit-queue-example"));
        
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

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
