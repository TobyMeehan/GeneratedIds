using System.Net.NetworkInformation;
using System.Net;

namespace ArpDetectionSystem.Events;

public record ArpResponse(PhysicalAddress Result, IPAddress RequestedAddress);
