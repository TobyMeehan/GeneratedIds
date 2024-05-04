using PacketDotNet;

namespace Ids.Sensor;

public interface IPacketMatch
{
    object? ParseEvent(Packet packet);
}
