using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(cfg =>
{
    cfg.UsingGrpc((context, options) =>
    {
        options.Host(new Uri("http://localhost:19797"), h =>
        {
            h.AddServer(new Uri("http://localhost:19796"));   
        });
        
        options.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.Run();