using MassTransit;
using SharpPcap;

namespace ArpSample;

public class PcapHostedService : IHostedService
{
    private readonly ILogger<PcapHostedService> _logger;
    private readonly IBus _bus;
    private readonly ILiveDevice _device;

    public PcapHostedService(ILogger<PcapHostedService> logger, IBus bus, ILiveDevice device)
    {
        _logger = logger;
        _bus = bus;
        _device = device;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _device.Open();

        _device.OnPacketArrival += (_, capture) => OnPacketParsed?.Invoke(capture.GetPacket());
        OnPacketParsed += async capture => await PublishPacketAsync(capture);
        
        _device.StartCapture();
        
        _logger.LogInformation("Capture started on {Device}", _device.Name);
        return Task.CompletedTask;
    }

    private event Action<RawCapture>? OnPacketParsed;

    private async Task PublishPacketAsync(RawCapture capture)
    {
        
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _device.StopCapture();

        _device.Close();
        
        return Task.CompletedTask;
    }
}