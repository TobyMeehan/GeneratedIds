using System.Net;
using System.Net.NetworkInformation;

namespace ArpSample.Events.Custom;

public record ArpResponse(PhysicalAddress Result, IPAddress RequestedAddress);