using System.Net;
using System.Net.NetworkInformation;

namespace ArpDemo.Events.Packets.Arp;

public interface IArpPacket
{
    IPAddress RequestedAddress { get; }
}