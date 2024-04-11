// -----------------------------------------------------------------------
// <author>Rodrigo Peireso</author>
// -----------------------------------------------------------------------

using System;
using System.Net.Http;

namespace VeraNet;

/// <summary>
/// Class that holds the connection information for a remote Vera controller, using the new cloud service (UI7).
/// </summary>
public class VeraConnectionInfoCloudUi7 : VeraConnectionInfo
{
    public string ServerRelay { get; init; }
    
    public string DeviceId { get; init; }
    
    public string RelaySession { get; init; }
    
    public VeraConnectionInfoCloudUi7(string serverRelay, string deviceId, string relaySession)
    {
        this.IsLocalConnection = false;
        this.ServerRelay = serverRelay;
        this.DeviceId = deviceId;
        this.RelaySession = relaySession;
        this.HttpClient = new HttpClient();
        this.HttpClient.BaseAddress = new Uri(this.GetUrl());
    }
    
    protected override string GetUrl()
    {
        return $"https://{this.ServerRelay}/relay/relay/relay/device/{this.DeviceId}/session/{this.RelaySession}/port_3480/";
    }
}