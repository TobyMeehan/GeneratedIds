using System.Net.NetworkInformation;
using System.Net;

namespace ArpDetectionSystem.Events.Custom;

public record ArpRequest(string SourceMac, string RequestedIp);
