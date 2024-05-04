using Ids.Events;
using Ids.Events.Custom;
using Ids.Monitor.ArpCachePoisoning.Events;
using Timeout = Ids.Monitor.ArpCachePoisoning.Events.Timeout;
using MassTransit;

namespace Ids.Monitor.ArpCachePoisoning;

public class ArpCachePoisoningStateMachine : MassTransitStateMachine<ArpCachePoisoningState>
{
    public ArpCachePoisoningStateMachine()
    {
        InstanceState(x => x.CurrentState, RequestHalfCycle, ResponseHalfCycle, FullCycle);

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
		

		Event(() => Timeout, x =>
		{
			x.CorrelateById(context => context.Message.CorrelationId);
		});

		Schedule(() => TimeoutSchedule, 
			x => x.TimeoutTokenId, 
			s =>
			{
				s.Delay = TimeSpan.FromSeconds(30);
				s.Received = r => r.CorrelateById(context => context.Message.CorrelationId);
			});
	
		Initially(
			
				// Init2Request
				When(ArpRequest)
					.Schedule(TimeoutSchedule, context => new Timeout(context.Saga.CorrelationId))
			
					.TransitionTo(RequestHalfCycle)
			, 
				// Init2Response
				When(ArpResponse)
					.Schedule(TimeoutSchedule, context => new Timeout(context.Saga.CorrelationId))
			
					.TransitionTo(ResponseHalfCycle)
		);
		
		During(RequestHalfCycle,
			
				// Request2Request
				When(ArpRequest)
					// Reset timeout
					.Unschedule(TimeoutSchedule)
					.Schedule(TimeoutSchedule, context => new Timeout(context.Saga.CorrelationId))
			
					.TransitionTo(RequestHalfCycle)
			, 
				// Request2Response
				When(ArpResponse)
					.TransitionTo(FullCycle)
		);
		
		During(ResponseHalfCycle,
			
				// Response2Request
				When(ArpRequest)
					// Reset timeout
					.Unschedule(TimeoutSchedule)
					.Schedule(TimeoutSchedule, context => new Timeout(context.Saga.CorrelationId))
			
					.TransitionTo(RequestHalfCycle)
			, 
				// Response2Response
				When(ArpResponse)
					// Publish alert
					.Publish(new Alert("Alert!"))
					
					.Finalize()
		);
		
		During(FullCycle,
			
				// FullCycle2Request
				When(ArpRequest)
					// Reset timeout
					.Unschedule(TimeoutSchedule)
					.Schedule(TimeoutSchedule, context => new Timeout(context.Saga.CorrelationId))
			
					.TransitionTo(RequestHalfCycle)
			, 
				// FullCycle2Response
				When(ArpResponse)
					// Publish alert
					.Publish(new Alert("Alert!"))
					
					.Finalize()
		);
		
    }

	public State RequestHalfCycle { get; } = default!;
	public State ResponseHalfCycle { get; } = default!;
	public State FullCycle { get; } = default!;
	
	public Schedule<ArpCachePoisoningState, Timeout> TimeoutSchedule { get; } = default!;
	
	public Event<ArpRequest> ArpRequest { get; } = default!;
	public Event<ArpResponse> ArpResponse { get; } = default!;
	public Event<Timeout> Timeout { get; } = default!;
}


