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
    /// <summary>
    /// The relay server used to connect to the Vera controller via the cloud.
    /// </summary>
    public string ServerRelay { get; init; }
    
    /// <summary>
    /// The device id of the Vera controller.
    /// </summary>
    public string DeviceId { get; init; }
    
    /// <summary>
    /// The relay session used to connect to the Vera controller via the cloud.
    /// </summary>
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
    
    protected sealed override string GetUrl()
    {
        return $"https://{this.ServerRelay}/relay/relay/relay/device/{this.DeviceId}/session/{this.RelaySession}/port_3480/";
    }
}