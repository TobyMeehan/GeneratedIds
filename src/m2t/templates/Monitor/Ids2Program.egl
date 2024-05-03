using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddQuartz();

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddPublishMessageScheduler();

    cfg.AddQuartzConsumers();

    cfg.UsingGrpc((context, options) =>
    {
        options.Host(new Uri(builder.Configuration["Grpc:Host"]));
        
        options.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.Run();