using ArpDetectionSystem.Events;
using MassTransit;

namespace ArpDetectionSystem.StateMachines.ArpSpoof;

public class ArpSpoofStateMachine : MassTransitStateMachine<ArpSpoofState>
{
    public ArpSpoofStateMachine()
    {
        InstanceState(x => x.CurrentState);

		Event(() => ThresholdTimeout, x => 
		{
			/* protected region eventCorrelationThresholdTimeout on begin */
			x.CorrelateById();
			
			/* protected region eventCorrelationThresholdTimeout end */
		});
		
		Event(() => ArpRequest, x => 
		{
			/* protected region eventCorrelationArpRequest on begin */
			x.CorrelateById();
			
			/* protected region eventCorrelationArpRequest end */
		});
		
		Event(() => ArpResponse, x => 
		{
			/* protected region eventCorrelationArpResponse on begin */
			x.CorrelateById();
			
			/* protected region eventCorrelationArpResponse end */
		});
		
		
		Schedule(() => Threshold, 
            x => x.CorrelationId, 
            x => x.Delay = TimeSpan.FromSeconds(60));
            
	
		Initially(
				When(ArpRequest)
					.TransitionTo(RequestHalfCycle)
			, 	When(ArpResponse)
					.TransitionTo(ResponseHalfCycle)
		);
		
		During(RequestHalfCycle,
				When(ArpRequest)
					.TransitionTo(RequestHalfCycle)
			, 	When(ArpResponse)
					.TransitionTo(FullCycle)
			, 	When(ThresholdTimeout)
					.Finalize()
		);
		
		During(ResponseHalfCycle,
				When(ArpRequest)
					.TransitionTo(RequestHalfCycle)
			, 	When(ArpResponse)
					.Finalize()
			, 	When(ThresholdTimeout)
					.Finalize()
		);
		
		During(FullCycle,
				When(ArpRequest)
					.TransitionTo(RequestHalfCycle)
			, 	When(ArpResponse)
					.Finalize()
			, 	When(ThresholdTimeout)
					.Finalize()
		);
		
    }

	public State RequestHalfCycle { get; } = default!;
	public State ResponseHalfCycle { get; } = default!;
	public State FullCycle { get; } = default!;
	
	public Schedule<ArpSpoofState, ThresholdTimeout> Threshold { get; } = default!;
	
	public Event<ThresholdTimeout> ThresholdTimeout { get; } = default!;
	public Event<ArpRequest> ArpRequest { get; } = default!;
	public Event<ArpResponse> ArpResponse { get; } = default!;
}


