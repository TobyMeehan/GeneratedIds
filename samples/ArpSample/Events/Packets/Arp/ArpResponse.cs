using System.Net;
using System.Net.NetworkInformation;

namespace ArpDemo.Events.Packets.Arp;

public record ArpResponse(PhysicalAddress Source, IPAddress RequestedAddress) : IArpPacket;