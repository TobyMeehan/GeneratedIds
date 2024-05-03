using ArpDetectionSystem.Events;
using ArpDetectionSystem.Events.Custom;
using ArpDetectionSystem.Monitor.ArpSpoof;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ArpDetectionSystem.Monitor.Tests;

public class ArpSpoofStateMachineTests
{
    [Fact]
    public async Task Attack_FullCycle_DistinctReplies()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<ArpSpoofStateMachine, ArpSpoofState>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();
        
        var sagaHarness = harness.GetSagaStateMachineHarness<ArpSpoofStateMachine, ArpSpoofState>();

        string requestMac = "55:70:20:c8:f5:2d";
        string attackerMac = "45:1c:49:23:43:7b";
        string validMac = "7d:7e:0a:ed:99:a6";
        string requestedIp = "10.10.10.10";
        
        await harness.Bus.Publish(new ArpRequest(
            SourceMac: requestMac, 
            RequestedIp: requestedIp));
        
        Assert.True(await sagaHarness.Consumed.Any<ArpRequest>());

        await harness.Bus.Publish(new ArpResponse(
            ResultMac: attackerMac,
            RequestedIp: requestedIp));
        
        Assert.True(await sagaHarness.Consumed.Any<ArpResponse>());

        await harness.Bus.Publish(new ArpResponse(
            ResultMac: validMac,
            RequestedIp: requestedIp));
        
        Assert.True(await sagaHarness.Consumed.Any<ArpResponse>());
        
        Assert.True(await harness.Published.Any<Alert>(), "Alert not published!");
    }

    [Fact]
    public async Task Attack_ReplyHalfCycle_DistinctReplies()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<ArpSpoofStateMachine, ArpSpoofState>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();
        
        var sagaHarness = harness.GetSagaStateMachineHarness<ArpSpoofStateMachine, ArpSpoofState>();

        string requestedIp = "11.11.11.11";
        string validMac = "34:15:76:29:38:9e";
        string attackerMac = "b9:15:63:47:0a:7f";
        
        await harness.Bus.Publish(new ArpResponse(
            ResultMac: validMac,
            RequestedIp: requestedIp));
        
        Assert.True(await sagaHarness.Consumed.Any<ArpResponse>());

        await harness.Bus.Publish(new ArpResponse(
            ResultMac: attackerMac,
            RequestedIp: requestedIp));
        
        Assert.True(await sagaHarness.Consumed.Any<ArpResponse>());
        
        Assert.True(await harness.Published.Any<Alert>(), "Alert not published!");
    }

    [Fact]
    public async Task Legit_RequestHalfCycle_SingleRequest()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<ArpSpoofStateMachine, ArpSpoofState>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();
        
        var sagaHarness = harness.GetSagaStateMachineHarness<ArpSpoofStateMachine, ArpSpoofState>();

        string requestedIp = "12.12.12.12";
        string sourceMac = "3b:9e:90:15:c6:13";

        await harness.Bus.Publish(new ArpRequest(
            SourceMac: sourceMac,
            RequestedIp: requestedIp));
        
        Assert.True(await sagaHarness.Consumed.Any<ArpRequest>());
        
        Assert.False(await harness.Published.Any<Alert>());
    }

    [Fact]
    public async Task Legit_RequestHalfCycle_MultipleRequests()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<ArpSpoofStateMachine, ArpSpoofState>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();
        
        var sagaHarness = harness.GetSagaStateMachineHarness<ArpSpoofStateMachine, ArpSpoofState>();

        string requestedIp = "13.13.13.13";
        string sourceMac1 = "2b:53:3f:04:9b:af";
        string sourceMac2 = "3a:99:ae:79:ee:26";
        
        await harness.Bus.Publish(new ArpRequest(
            SourceMac: sourceMac1,
            RequestedIp: requestedIp));
        
        Assert.True(await sagaHarness.Consumed.Any<ArpRequest>());
        
        await harness.Bus.Publish(new ArpRequest(
            SourceMac: sourceMac2,
            RequestedIp: requestedIp));
        
        Assert.True(await sagaHarness.Consumed.Any<ArpRequest>());
        
        Assert.False(await harness.Published.Any<Alert>());
    }

    [Fact]
    public async Task Legit_FullCycle_SingleReply()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<ArpSpoofStateMachine, ArpSpoofState>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        string requestedIp = "14.14.14.14";
        string requestMac = "a7:f8:f4:c2:52:13";
        string resultMac = "97:f0:ea:aa:ce:1a";
        
        await harness.Bus.Publish(new ArpRequest(
            SourceMac: requestMac, 
            RequestedIp: requestedIp));

        var sagaHarness = harness.GetSagaStateMachineHarness<ArpSpoofStateMachine, ArpSpoofState>();
        
        Assert.True(await sagaHarness.Consumed.Any<ArpRequest>());

        await harness.Bus.Publish(new ArpResponse(
            ResultMac: resultMac,
            RequestedIp: requestedIp));
        
        Assert.True(await sagaHarness.Consumed.Any<ArpResponse>());
        
        Assert.False(await harness.Published.Any<Alert>());
    }

    [Fact]
    public async Task Legit_ReplyHalfCycle_SingleReply()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<ArpSpoofStateMachine, ArpSpoofState>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();
        
        var sagaHarness = harness.GetSagaStateMachineHarness<ArpSpoofStateMachine, ArpSpoofState>();

        string requestedIp = "15.15.15.15";
        string resultMac = "9b:1c:da:5c:30:cd";
        
        await harness.Bus.Publish(new ArpResponse(
            ResultMac: resultMac,
            RequestedIp: requestedIp));
        
        Assert.True(await sagaHarness.Consumed.Any<ArpResponse>());
        
        Assert.False(await harness.Published.Any<Alert>());
    }
}