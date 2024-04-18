using MassTransit;
using System.Net;

namespace ArpDetectionSystem.StateMachines.ArpSpoof;

public class ArpSpoofState : SagaStateMachineInstance
{
	public IPAddress RequestedAddress { get; set; }

    public int CurrentState { get; set; }
    public Guid CorrelationId { get; set; }
}