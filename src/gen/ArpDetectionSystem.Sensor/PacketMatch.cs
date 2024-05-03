using ArpDetectionSystem.Events.Custom;
using PacketDotNet;

namespace ArpDetectionSystem.Sensor;

public class PacketMatch : IPacketMatch
{
    public object? ParseEvent(Packet packet) => packet switch
    {
	    /* protected region Match ArpRequest on begin */
	    ArpPacket {Operation: ArpOperation.Request} arpRequest 
		/* protected region Match ArpRequest end */
            
		    => new ArpRequest(
			
			    /* protected region ArpRequest Fields on begin */
			    arpRequest.SenderHardwareAddress, 
			    arpRequest.TargetProtocolAddress
			    /* protected region ArpRequest Fields end */
			    
		    ),
        
	    /* protected region Match ArpResponse on begin */
	    ArpPacket {Operation: ArpOperation.Response} arpResponse 
		/* protected region Match ArpResponse end */
		    
		    => new ArpResponse(
			
			    /* protected region ArpResponse Fields on begin */
			    arpResponse.SenderHardwareAddress, 
			    arpResponse.TargetProtocolAddress
			    /* protected region ArpResponse Fields end */
		    ),
        
	    _ => null
    };
}
