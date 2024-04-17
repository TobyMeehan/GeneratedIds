using ArpDemo.Events;
using ArpDemo.Events.Packets.Arp;
using MassTransit;

namespace ArpSample.StateMachines;

public class ArpStateMachine : MassTransitStateMachine<ArpState>
{
    public ArpStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => Request, 
            x => x.CorrelateByRequestedAddress());

        Event(() => Response, 
            x => x.CorrelateByRequestedAddress());

        Event(() => ThresholdTimeout, 
            x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

        Schedule(() => Threshold, 
            x => x.CorrelationId, 
            x => x.Delay = TimeSpan.FromSeconds(30));
        
        Initially(
            When(Request)
                .ScheduleTimeout(Threshold)
                .TransitionTo(RequestHalfCycle),
            When(Response)
                .ScheduleTimeout(Threshold)
                .TransitionTo(ResponseHalfCycle),
            Ignore(ThresholdTimeout));
        
        During(RequestHalfCycle,
            When(Request)
                .Unschedule(Threshold)
                .ScheduleTimeout(Threshold),
            When(Response)
                .TransitionTo(FullCycle),
            When(ThresholdTimeout)
                .Finalize());

        During(ResponseHalfCycle,
            When(Request)
                .Unschedule(Threshold)
                .ScheduleTimeout(Threshold)
                .TransitionTo(RequestHalfCycle),
            When(Response)
                .Publish(new Alert()),
            When(ThresholdTimeout)
                .Finalize());
        
        During(FullCycle,
            When(Request)
                .Unschedule(Threshold)
                .ScheduleTimeout(Threshold)
                .TransitionTo(RequestHalfCycle),
            When(Response)
                .Publish(new Alert()),
            When(ThresholdTimeout)
                .Finalize());
    }

    public State RequestHalfCycle { get; } = default!;
    public State ResponseHalfCycle { get; } = default!;
    public State FullCycle { get; } = default!;

    public Schedule<ArpState, ThresholdTimeout> Threshold { get; } = default!;
    
    public Event<ArpRequest> Request { get; } = default!;
    public Event<ArpResponse> Response { get; } = default!;
    public Event<ThresholdTimeout> ThresholdTimeout { get; } = default!;
}

public static class ArpStateMachineExtensions
{
    public static void CorrelateByRequestedAddress<TPacket>(
        this IEventCorrelationConfigurator<ArpState, TPacket> config) where TPacket : class, IArpPacket
    {
        config.CorrelateBy(state => state.RequestedAddress, ctx => ctx.Message.RequestedAddress);
        config.SetSagaFactory(context => new ArpState
        {
            CorrelationId = context.CorrelationId ?? NewId.NextGuid(),
            RequestedAddress = context.Message.RequestedAddress
        });
        config.SelectId(_ => NewId.NextGuid());
    }

    public static EventActivityBinder<ArpState, TPacket> ScheduleTimeout<TPacket>(
        this EventActivityBinder<ArpState, TPacket> binder, Schedule<ArpState, ThresholdTimeout> schedule) where TPacket : class, IArpPacket
    {
        return binder.Schedule(schedule, context => new ThresholdTimeout(context.Saga.CorrelationId));
    }
}