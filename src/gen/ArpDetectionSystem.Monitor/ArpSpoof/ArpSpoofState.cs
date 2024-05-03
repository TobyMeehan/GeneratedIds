using MassTransit;
using System.Net;

namespace ArpDetectionSystem.Monitor.ArpSpoof;

public class ArpSpoofState : SagaStateMachineInstance
{
	public string RequestedIp { get; set; }

    public int CurrentState { get; set; }
    public Guid CorrelationId { get; set; }
    public Guid? TimeoutTokenId { get; set; }
}