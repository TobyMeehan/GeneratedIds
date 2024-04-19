using ArpSample.Events;
using ArpSample.Events.Custom;
using MassTransit;

namespace ArpSample.Monitor.ArpSpoof;

public class ArpSpoofStateMachine : MassTransitStateMachine<ArpSpoofState>
{
    public ArpSpoofStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => Request,
            x =>
            {
                x.CorrelateBy(state => state.RequestedAddress, ctx => ctx.Message.RequestedAddress);
                x.SetSagaFactory(context => new ArpSpoofState
                {
                    CorrelationId = context.CorrelationId ?? NewId.NextGuid(),
                    RequestedAddress = context.Message.RequestedAddress
                });
                x.SelectId(_ => NewId.NextGuid());
            });

        Event(() => Response,
            x =>
            {
                x.CorrelateBy(state => state.RequestedAddress, ctx => ctx.Message.RequestedAddress);
                x.SetSagaFactory(context => new ArpSpoofState
                {
                    CorrelationId = context.CorrelationId ?? NewId.NextGuid(),
                    RequestedAddress = context.Message.RequestedAddress
                });
                x.SelectId(_ => NewId.NextGuid());
            });

        Event(() => ThresholdTimeout, 
            x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

        Schedule(() => Threshold, 
            x => x.CorrelationId, 
            x => x.Delay = TimeSpan.FromSeconds(30));
        
        Initially(
            When(Request)
                .Schedule(Threshold, context => new ThresholdTimeout(context.Saga.CorrelationId))
                .TransitionTo(RequestHalfCycle),
            When(Response)
                .Schedule(Threshold, context => new ThresholdTimeout(context.Saga.CorrelationId))
                .TransitionTo(ResponseHalfCycle),
            Ignore(ThresholdTimeout));
        
        During(RequestHalfCycle,
            When(Request)
                .Unschedule(Threshold)
                .Schedule(Threshold, context => new ThresholdTimeout(context.Saga.CorrelationId)),
            When(Response)
                .TransitionTo(FullCycle),
            When(ThresholdTimeout)
                .Finalize());

        During(ResponseHalfCycle,
            When(Request)
                .Unschedule(Threshold)
                .Schedule(Threshold, context => new ThresholdTimeout(context.Saga.CorrelationId))
                .TransitionTo(RequestHalfCycle),
            When(Response)
                .Publish(new Alert("Multiple ARP responses detected.")),
            When(ThresholdTimeout)
                .Finalize());
        
        During(FullCycle,
            When(Request)
                .Unschedule(Threshold)
                .Schedule(Threshold, context => new ThresholdTimeout(context.Saga.CorrelationId))
                .TransitionTo(RequestHalfCycle),
            When(Response)
                .Publish(new Alert("Multiple ARP responses detected.")),
            When(ThresholdTimeout)
                .Finalize());
    }

    public State RequestHalfCycle { get; } = default!;
    public State ResponseHalfCycle { get; } = default!;
    public State FullCycle { get; } = default!;

    public Schedule<ArpSpoofState, ThresholdTimeout> Threshold { get; } = default!;
    
    public Event<ArpRequest> Request { get; } = default!;
    public Event<ArpResponse> Response { get; } = default!;
    public Event<ThresholdTimeout> ThresholdTimeout { get; } = default!;
}