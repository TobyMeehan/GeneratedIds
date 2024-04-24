using ArpDetectionSystem.Events.Custom;
using PacketDotNet;

namespace ArpDetectionSystem.Sensor;

public class PacketMatch : IPacketMatch
{
    public object? ParseEvent(Packet packet) => packet switch
    {
    	/* protected region Match ArpRequest on begin */
    	arpRequest
    	/* protected region Match ArpRequest end */ => new ArpRequest(/* protected region ArpRequest Fields on begin */
    	                                                              
    	                                                              /* protected region ArpRequest Fields end */),
    	/* protected region Match ArpResponse on begin */
    	arpResponse
    	/* protected region Match ArpResponse end */ => new ArpResponse(/* protected region ArpResponse Fields on begin */
    	                                                                
    	                                                                /* protected region ArpResponse Fields end */),
        
        _ => null
    };
}
