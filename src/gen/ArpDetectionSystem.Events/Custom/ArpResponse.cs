using System.Net.NetworkInformation;
using System.Net;

namespace ArpDetectionSystem.Events.Custom;

public record ArpResponse(PhysicalAddress Result, IPAddress RequestedAddress);
