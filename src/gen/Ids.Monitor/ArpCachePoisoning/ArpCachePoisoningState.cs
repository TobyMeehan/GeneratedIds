using MassTransit;
using ;

namespace Ids.Monitor.ArpCachePoisoning;

public class ArpCachePoisoningState : SagaStateMachineInstance
{
	public string RequestedAddress { get; set; }

    public int CurrentState { get; set; }
    public Guid CorrelationId { get; set; }
	public Guid? TimeoutTokenId { get; set; }
}