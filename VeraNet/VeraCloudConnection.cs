// -----------------------------------------------------------------------
// <author>Rodrigo Peireso</author>
// -----------------------------------------------------------------------

// The code in this class and in <see cref="VeraConnectionInfoCloudUi7"/> 
// is a refactored version based of the code of cgmartin/vera_auth_test.sh
// (https://gist.github.com/cgmartin/466bd2d3724de6c04743d61cf0de2066#file-vera_auth_test-sh)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VeraNet.Objects;

namespace VeraNet;

/// <summary>
/// Class that handles the connection to Mios servers to get remote controllers ans sessions, in the new cloud service (UI7).
/// </summary>
public class VeraCloudConnection(string username, string password, HttpClient httpClient)
{
    /// <summary>
    /// The Mios authentication server URL.
    /// </summary>
    private const string AuthUrl = "vera-us-oem-autha.mios.com";
    
    /// <summary>
    /// The password seed, used to hash the password.
    /// </summary>
    private const string PasswordSeed = "oZ7QE6LcLJp6fiWzdqZc";

    /// <summary>
    /// The identity token is used to get a new session token.
    /// </summary>
    private string IdentityToken { get; set; }
    
    /// <summary>
    /// The identity signature is used to get a new session token.
    /// </summary>
    private string IdentitySignature { get; set; }
    
    /// <summary>
    /// The Mios account server URL, used to get the session token and the account devices.
    /// </summary>
    private string ServerAccount { get; set; }
    
    /// <summary>
    /// The account identifier, also known as the PK_Account.
    /// </summary>
    private string AccountId { get; set; }
        
    /// <summary>
    /// The session token is used during the session.
    /// It is used to connect to the Server Account, to get the identity information, and to get the account devices.
    /// </summary>
    private string SessionToken { get; set; }
    
    public VeraCloudConnection(string username, string password) : this(username, password, new HttpClient()) {}

    /// <summary>
    /// Authenticate the user in the Mios servers and gets the identity token, signature and server account,
    /// used to get the session token.
    /// </summary>
    /// <returns>Identity Token</returns>
    private async Task<string> AuthenticateAsync()
    {
        // hash the password
        var sha1Password = GetSha1Password();
        
        // create the request
        var url = $"https://{AuthUrl}/autha/auth/username/{username}?SHA1Password={sha1Password}&PK_Oem=1";
 
        // send the request
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        // parse the response
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        var root = json.RootElement;
        
        // get the returned values
        IdentityToken = root.GetProperty("Identity").GetString();
        IdentitySignature = root.GetProperty("IdentitySignature").GetString();
        ServerAccount = root.GetProperty("Server_Account").GetString();
        AccountId = ExtractAccountId(IdentityToken);
        return IdentityToken;
    }

    private string ExtractAccountId(string identityToken)
    {
        var data = Convert.FromBase64String(identityToken);
        var json = JsonDocument.Parse(data);
        return json.RootElement.GetProperty("PK_Account").GetInt32().ToString();
    }

    /// <summary>
    /// Gets the session token from the Mios servers, to be used in the session with the remote cloud.
    /// </summary>
    /// <returns>Session Token</returns>
    private async Task<string> GetSessionTokenAsync()
    {
        // create the request
        var url = $"https://{ServerAccount}/info/session/token";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("MMSAuth", IdentityToken);
        request.Headers.Add("MMSAuthSig", IdentitySignature);

        // send the request
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        // parse the response
        SessionToken = await response.Content.ReadAsStringAsync();
        return SessionToken;
    }
    
    /// <summary>
    /// Get account devices (Hubs) from the Mios servers.
    /// </summary>
    /// <returns>Vera Account Devices of the Mios account.</returns>
    private async Task<IEnumerable<VeraAccountDevice>> GetAccountDevicesAsync()
    {
        // create the request
        var url = $"https://{ServerAccount}/account/account/account/{AccountId}/devices";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("MMSSession", SessionToken);
        
        // send the request
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        // parse the response
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        var root = json.RootElement;
        
        // get the devices and parse them
        var devices = root.GetProperty("Devices")
            .EnumerateArray()
            .Select(device => new VeraAccountDevice
            {
                DeviceId = device.GetProperty("PK_Device").GetString(),
                DeviceType = (DeviceCategory)Enum.Parse(typeof(DeviceCategory),
                    device.GetProperty("PK_DeviceType").GetString()!),
                DeviceSubType = device.GetProperty("PK_DeviceSubType").GetString(),
                MacAddress = device.GetProperty("MacAddress").GetString(),
                ServerDevice = device.GetProperty("Server_Device").GetString(),
                ServerDeviceAlt = device.GetProperty("Server_Device_Alt").GetString(),
                InstallationPk = device.GetProperty("PK_Installation").GetString(),
                Name = device.GetProperty("Name").GetString(),
                DeviceAssigned = DateTime.ParseExact(
                    device.GetProperty("DeviceAssigned").GetString()!,
                    "yyyy-MM-dd HH:mm:ss", null),
                Using2G = device.GetProperty("Using_2G").GetString() == "1", // the api returns "1" or "0" instead of true or false
                Blocked = device.GetProperty("Blocked").GetInt32() == 1 // the api returns 1 or 0 instead of true or false
            });

        return devices;
    }

    /// <summary>
    /// Hashes the password using SHA1, to be used in the authentication.
    /// </summary>
    /// <returns>Hashed password.</returns>
    private string GetSha1Password()
    {
        var bytes = Encoding.UTF8.GetBytes(username + password + PasswordSeed);
        var hash = SHA1.HashData(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
    
    /// <summary>
    /// Gets the devices (hubs) of the account.
    /// </summary>
    public async Task<IEnumerable<VeraAccountDevice>> GetDevicesAsync()
    {
        await AuthenticateAsync();
        await GetSessionTokenAsync();
        return await GetAccountDevicesAsync();
    }
    
}