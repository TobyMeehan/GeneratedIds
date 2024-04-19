using PacketDotNet;

namespace ArpSample.Sensor;

public interface IPacketMatch
{
    object? ParseEvent(Packet packet);
}