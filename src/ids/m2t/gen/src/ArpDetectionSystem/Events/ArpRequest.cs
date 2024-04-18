using System.Net.NetworkInformation;
using System.Net;

namespace ArpDetectionSystem.Events;

public record ArpRequest(PhysicalAddress Source, IPAddress RequestedAddress);
