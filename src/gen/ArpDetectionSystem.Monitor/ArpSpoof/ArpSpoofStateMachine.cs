using ArpDetectionSystem.Events;
using ArpDetectionSystem.Events.Custom;
using ArpDetectionSystem.Monitor.ArpSpoof.Events;
using MassTransit;

namespace ArpDetectionSystem.Monitor.ArpSpoof;

public class ArpSpoofStateMachine : MassTransitStateMachine<ArpSpoofState>
{
    public ArpSpoofStateMachine()
    {
        InstanceState(x => x.CurrentState);

		Event(() => ArpRequest, x => 
		{
			/* protected region Correlate ArpRequest on begin */
			x.CorrelateById();
			/* protected region Correlate ArpRequest end */
		});
		
		Event(() => ArpResponse, x => 
		{
			/* protected region Correlate ArpResponse on begin */
			x.CorrelateById();
			/* protected region Correlate ArpResponse end */
		});
		
		
		Schedule(() => TimeoutSchedule, 
            x => x.CorrelationId, 
            x => x.Delay = TimeSpan.FromSeconds(30));
	
		Initially(
				When(ArpRequest)
					.Unschedule(TimeoutSchedule)
					.Schedule(TimeoutSchedule, context => new Timeout(context.Saga.CorrelationId))
					.TransitionTo(RequestHalfCycle)
			, 	When(ArpResponse)
					.Unschedule(TimeoutSchedule)
					.Schedule(TimeoutSchedule, context => new Timeout(context.Saga.CorrelationId))
					.TransitionTo(ResponseHalfCycle)
		);
		
		During(RequestHalfCycle,
				When(ArpRequest)
					.Unschedule(TimeoutSchedule)
					.TransitionTo(RequestHalfCycle)
			, 	When(ArpResponse)
					.Unschedule(TimeoutSchedule)
					.TransitionTo(FullCycle)
		);
		
		During(ResponseHalfCycle,
				When(ArpRequest)
					.Unschedule(TimeoutSchedule)
					.TransitionTo(RequestHalfCycle)
			, 	When(ArpResponse)
					.Unschedule(TimeoutSchedule)
					.Finalize()
		);
		
		During(FullCycle,
				When(ArpRequest)
					.Unschedule(TimeoutSchedule)
					.TransitionTo(RequestHalfCycle)
			, 	When(ArpResponse)
					.Unschedule(TimeoutSchedule)
					.Finalize()
		);
		
    }

	public State RequestHalfCycle { get; } = default!;
	public State ResponseHalfCycle { get; } = default!;
	public State FullCycle { get; } = default!;
	
	public Schedule<ArpSpoofState, TimeoutSchedule> TimeoutSchedule { get; } = default!;
	
	public Event<ArpRequest> ArpRequest { get; } = default!;
	public Event<ArpResponse> ArpResponse { get; } = default!;
	public Event<Timeout> Timeout { get; } = default!;
}


