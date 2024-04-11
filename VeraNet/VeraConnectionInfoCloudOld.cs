// -----------------------------------------------------------------------
// <author>Rodrigo Peireso</author>
// -----------------------------------------------------------------------


using System;
using System.Net.Http;

namespace VeraNet;

/// <summary>
/// Class that holds the connection information for a remote Vera controller, using the old cloud service (UI6 or older).
/// </summary>
public class VeraConnectionInfoCloudOld : VeraConnectionInfo
{
    /// <summary>
    /// Gets or sets the remote user.
    /// </summary>
    /// <value>
    /// The remote user.
    /// </value>
    public string RemoteUser { get; init; }
    /// <summary>
    /// Gets or sets the remote password.
    /// </summary>
    /// <value>
    /// The remote password.
    /// </value>
    public string RemotePassword { get; init; }
    /// <summary>
    /// Gets or sets the remote serial.
    /// </summary>
    /// <value>
    /// The remote serial.
    /// </value>
    public int RemoteSerial { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VeraConnectionInfoCloudOld"/> class for remote access.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="password">The password.</param>
    /// <param name="sn">The sn.</param>
    public VeraConnectionInfoCloudOld(string user, string password, int sn)
    {
        this.IsLocalConnection = false;
        this.RemotePassword = password;
        this.RemoteSerial = sn;
        this.RemoteUser = user;
        this.HttpClient = new HttpClient();
        this.HttpClient.BaseAddress = new Uri(this.GetUrl());
    }
    
    protected sealed override string GetUrl()
    {
        return $"https://fwd2.mios.com/{this.RemoteUser}/{this.RemotePassword}/{this.RemoteSerial}/";
    }


}