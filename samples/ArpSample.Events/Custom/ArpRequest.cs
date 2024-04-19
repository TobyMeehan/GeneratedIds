using System.Net;
using System.Net.NetworkInformation;

namespace ArpSample.Events.Custom;

public record ArpRequest(PhysicalAddress Source, IPAddress RequestedAddress);