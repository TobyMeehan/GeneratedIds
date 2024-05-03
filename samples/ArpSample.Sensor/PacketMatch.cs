using ArpSample.Events;
using ArpSample.Events.Custom;
using PacketDotNet;

namespace ArpSample.Sensor;

public class PacketMatch : IPacketMatch
{
    public object? ParseEvent(Packet packet) => packet switch
    {
        ArpPacket {Operation: ArpOperation.Request} arpRequest 
            
            => new ArpRequest(
                arpRequest.SenderHardwareAddress, 
                arpRequest.TargetProtocolAddress
            ),
        
        
        ArpPacket {Operation: ArpOperation.Response} arpResponse 
            
            => new ArpResponse(
                arpResponse.SenderHardwareAddress, 
                arpResponse.TargetProtocolAddress
                ),
        
        _ => null
    };
}