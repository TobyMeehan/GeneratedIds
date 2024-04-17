using System.Net;
using System.Net.NetworkInformation;

namespace ArpDemo.Events.Packets.Arp;

public record ArpRequest(PhysicalAddress Source, IPAddress RequestedAddress) : IArpPacket;