using System.Net;
using MassTransit;

namespace ArpSample.Monitor.ArpSpoof;

public class ArpSpoofState : SagaStateMachineInstance
{
    public IPAddress RequestedAddress { get; set; }

    public int CurrentState { get; set; }
    public Guid CorrelationId { get; set; }
}