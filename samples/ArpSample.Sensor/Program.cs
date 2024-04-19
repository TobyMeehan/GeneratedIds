using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddMassTransit(cfg =>
{
    cfg.UsingGrpc((context, options) =>
    {
        options.Host(new Uri(builder.Configuration["Grpc:Host"]), h =>
        {
            h.AddServer(new Uri(builder.Configuration["Grpc:Monitor"]));   
        });
        
        options.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.Run();