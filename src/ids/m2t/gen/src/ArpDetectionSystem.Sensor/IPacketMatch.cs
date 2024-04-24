using PacketDotNet;

namespace ArpDetectionSystem.Sensor;

public interface IPacketMatch
{
    object? ParseEvent(Packet packet);
}
