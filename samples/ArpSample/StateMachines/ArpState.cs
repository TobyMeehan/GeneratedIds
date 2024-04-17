using System.Net;
using MassTransit;

namespace ArpSample.StateMachines;

public class ArpState : SagaStateMachineInstance
{
    public IPAddress RequestedAddress { get; set; }

    public int CurrentState { get; set; }
    public Guid CorrelationId { get; set; }
}