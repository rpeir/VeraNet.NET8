// -----------------------------------------------------------------------
// <author>Rodrigo Peireso</author>
// -----------------------------------------------------------------------

using System;
using System.Net.Http;

namespace VeraNet;

/// <summary>
/// Class that holds the connection information for a local Vera controller.
/// </summary>
public class VeraConnectionLocal : VeraConnection
{
    
    /// <summary>
    /// Gets or sets the local ip.
    /// </summary>
    /// <value>
    /// The local ip.
    /// </value>
    public string LocalIp { get; init; }
        
    /// <summary>
    /// Gets or sets the local port.
    /// </summary>
    /// <value>
    /// The local port.
    /// </value>
    public int LocalPort { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VeraConnectionLocal"/> class for local access.
    /// </summary>
    /// <param name="localIp">The local ip.</param>
    /// <param name="localPort">The local port (3480 by default).</param>
    public VeraConnectionLocal(string localIp, int localPort = 3480) : this(new HttpClient(), localIp, localPort) {}
    
    /// <summary>
    /// Initializes a new instance of the <see cref="VeraConnectionLocal"/> class for local access.
    /// </summary>
    /// <param name="httpClient">The Httpclient to make the requests.</param>
    /// <param name="localIp">The local ip.</param>
    /// <param name="localPort">The local port (3480 by default).</param>
    public VeraConnectionLocal(HttpClient httpClient, string localIp, int localPort = 3480)
    {
        this.IsLocalConnection = true;
        this.LocalIp = localIp;
        this.LocalPort = localPort;
        this.HttpClient = httpClient;
        this.HttpClient.BaseAddress = new Uri(this.GetUrl());
    }

    protected sealed override string GetUrl()
    {
        return $"http://{this.LocalIp}:{this.LocalPort}/";
    }
    
}