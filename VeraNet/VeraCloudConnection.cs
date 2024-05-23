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
using VeraNet.Utils;
using System.Runtime.InteropServices;

namespace VeraNet;

/// <summary>
/// Class that handles the connection to Mios servers to get remote controllers and sessions, in the new cloud service (UI7).
/// </summary>
[ComVisible(true), Guid("D14FBA36-3059-45F6-82E3-3408FD2186C3")]
public class VeraCloudConnection
{
    public VeraCloudConnection() {}
    
    public VeraCloudConnection(string username, string password)
    {
        Username = username;
        Password = password;
    }
    private HttpClient HttpClient { get; init; } = new();
    
    /// <summary>
    /// The username of the Mios account.
    /// </summary>
    public string Username { get; set; }
    
    /// <summary>
    /// The password of the Mios account.
    /// </summary>
    public string Password { private get; set; }
    
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
        var url = $"https://{AuthUrl}/autha/auth/username/{Username}?SHA1Password={sha1Password}&PK_Oem=1";
 
        // send the request
        var response = await HttpClient.GetAsync(url);
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
    
    /// <summary>
    /// Hashes the password using SHA1, to be used in the authentication.
    /// </summary>
    /// <returns>Hashed password.</returns>
    private string GetSha1Password()
    {
        var bytes = Encoding.UTF8.GetBytes(Username + Password + PasswordSeed);
        var hash = SHA1.HashData(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    /// <summary>
    /// Extracts the account identifier from the identity token.
    /// </summary>
    /// <param name="identityToken"></param>
    /// <returns></returns>
    private static string ExtractAccountId(string identityToken)
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
        var response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        // parse the response
        SessionToken = await response.Content.ReadAsStringAsync();
        return SessionToken;
    }
    
    /// <summary>
    /// Get account devices (Hubs) from the Mios servers.
    /// </summary>
    /// <returns>Vera Account Devices of the Mios account.</returns>
    private async Task<List<VeraAccountDevice>> GetAccountDevicesAsync()
    {
        // create the request
        var url = $"https://{ServerAccount}/account/account/account/{AccountId}/devices";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("MMSSession", SessionToken);
        
        // send the request
        var response = await HttpClient.SendAsync(request);
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

        return devices.ToList();
    }

    /// <summary>
    /// Gets addition information of a Vera Account Device (Hub) from the Mios servers.
    /// </summary>
    /// <param name="device">The vera account device (hub).</param>
    /// <returns>Additional Info.</returns>
    public async Task<VeraAccountDeviceInfo> GetAdditionalInfoAsync(VeraAccountDevice device)
    {
        // create the request
        var url = $"https://{device.ServerDevice}/device/device/device/{device.DeviceId}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("MMSSession", SessionToken);
        
        // send the request
        var response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        // parse the response
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        var root = json.RootElement;
        
        // get the info and parse it
        var deviceInfo = new VeraAccountDeviceInfo
        {
            DeviceId = root.GetProperty("PK_Device").GetString(),
            ServerRelay = root.GetProperty("Server_Relay").GetString(),
            MacAddress = root.GetProperty("MacAddress").GetString(),
            Using2G = root.GetProperty("Using_2G").GetInt32() == 1, // the api returns 1 or 0 instead of true or false
            ExternalIp = root.GetProperty("ExternalIP").GetString(),
            AccessiblePort = int.Parse(root.GetProperty("AccessiblePort").GetString()!),
            InternalIp = root.GetProperty("InternalIP").GetString(),
            AliveDate = DateTime.ParseExact(
                root.GetProperty("AliveDate").GetString()!,
                "yyyy-MM-dd HH:mm:ss", null),
            FirmwareVersion = root.GetProperty("FirmwareVersion").GetString(),
            PriorFirmwareVersion = root.GetProperty("PriorFirmwareVersion").GetString(),
            UpgradeDate = DateTime.ParseExact(
                root.GetProperty("UpgradeDate").GetString()!,
                "yyyy-MM-dd HH:mm:ss", null),
            Uptime = root.GetProperty("Uptime").GetString(),
            ServerDevice = root.GetProperty("Server_Device").GetString(),
            ServerEvent = root.GetProperty("Server_Event").GetString(),
            ServerSupport = root.GetProperty("Server_Support").GetString(),
            ServerStorage = root.GetProperty("Server_Storage").GetString(),
            WifiSsid = root.GetProperty("WifiSsid").GetString(),
            Timezone = root.GetProperty("Timezone").GetString(),
            LocalPort = int.Parse(root.GetProperty("LocalPort").GetString()!),
            ZWaveLocale = root.GetProperty("ZWaveLocale").GetString(),
            ZWaveVersion = root.GetProperty("ZWaveVersion").GetString(),
            BrandingFk = root.GetProperty("FK_Branding").GetString(),
            Platform = root.GetProperty("Platform").GetString(),
            UiLanguage = root.GetProperty("UILanguage").GetString(),
            UiSkin = root.GetProperty("UISkin").GetString(),
            HasWifi = root.GetProperty("HasWifi").GetString() == "1", // the api returns "1" or "0" instead of true or false
            HasAlarmPanel = root.GetProperty("HasAlarmPanel").GetString() == "1", // the api returns "1" or "0" instead of true or false
            UiVersion = int.Parse(root.GetProperty("UI").GetString()),
            EngineStatus = root.GetProperty("EngineStatus").GetString(),
            DistributionBuild = root.GetProperty("DistributionBuild").GetString(),
            AccessPermissions = root.GetProperty("AccessPermissions").GetString(),
            LinuxFirmware = root.GetProperty("LinuxFirmware").GetInt32()
        };
        
        return deviceInfo;
    }
    
    /// <inheritdoc cref="GetAdditionalInfoAsync"/>
    public VeraAccountDeviceInfo GetAdditionalInfo(VeraAccountDevice device)
    {
        var asyncTask = Task.Run(async () => await GetAdditionalInfoAsync(device));
        // Wait for the task to complete and get the result
        return asyncTask.Result;
    }
    
    /// <summary>
    /// Gets the devices (hubs) of the account.
    /// </summary>
    /// <returns>A list of Vera Devices (hubs) associated with the account.</returns>
    public async Task<List<VeraAccountDevice>> GetDevicesAsync()
    {
        await AuthenticateAsync();
        await GetSessionTokenAsync();
        return await GetAccountDevicesAsync();
    }
    
    /// <inheritdoc cref="GetDevicesAsync"/>
    public List<VeraAccountDevice> GetDevices()
    {
        var asyncTask = Task.Run(async () => await GetDevicesAsync());
        // Wait for the task to complete and get the result
        return asyncTask.Result;
    }

    /// <summary>
    /// Creates a Vera Controller instance from a Vera Account Device (Hub).
    /// </summary>
    /// <param name="device">The vera account device (hub).</param>
    /// <param name="startListener">If the vera hub start listening when created.</param>
    /// <returns>A Vera Controller instance</returns>
    public async Task<VeraController> GetControllerAsync(VeraAccountDevice device, bool startListener = false)
    {
        await AuthenticateAsync();
        await GetSessionTokenAsync();
        var deviceInfo = await GetAdditionalInfoAsync(device);
        
        // if is a device with UI version less than 7, use the old cloud connection
        if (deviceInfo.UiVersion < 7)
            return new VeraController(new VeraConnectionCloudOld(Username, Password, int.Parse(device.DeviceId)), startListener);
        
        var relaySession = await GetRelayServerSessionTokenAsync(deviceInfo);
        return new VeraController(new VeraConnectionCloudUi7(deviceInfo.ServerRelay, deviceInfo.DeviceId, relaySession), startListener);
    }
    
    /// <inheritdoc cref="GetControllerAsync"/>
    public VeraController GetController(VeraAccountDevice device, bool startListener = false)
    {
        var asyncTask = Task.Run(async () => await GetControllerAsync(device, startListener));
        // Wait for the task to complete and get the result
        return asyncTask.Result;
    }

    /// <summary>
    /// Gets the session token from the relay server of a vera device.
    /// It is needed to request a new session for each server.
    /// </summary>
    /// <param name="deviceInfo">The vera device info.</param>
    /// <returns>The Session Token for the Relay Server.</returns>
    private async Task<string> GetRelayServerSessionTokenAsync(VeraAccountDeviceInfo deviceInfo)
    {
        // create the request
        var url = $"https://{deviceInfo.ServerRelay}/info/session/token";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("MMSAuth", IdentityToken);
        request.Headers.Add("MMSAuthSig", IdentitySignature);
        
        // send the request
        var response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        // parse the response
        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Get the available compatible devices with the Vera Controller Device.
    /// </summary>
    /// <param name="device">The Vera Controller.</param>
    /// <returns>
    /// A json that contains DeviceWizardCategory[] and KitDevice[].
    /// The DeviceWizardCategory contains <c>PK_DeviceWizardCategory</c> and <c>LS_DeviceWizardCategory</c>.
    /// The KitDevice always contains <c>PK_KitDevice</c>, <c>Name { text }</c>, <c>RequireMac</c>, <c>Protocol</c>, <c>NonSpecific</c>, <c>Invisible</c> and <c>Exclude</c>.
    /// Each specific KitDevice could contain other properties.
    /// </returns>
    public async Task<Dictionary<string, object>> GetKitDevicesAsync(VeraAccountDevice device)
    {
        // get the device info
        var deviceInfo = await GetAdditionalInfoAsync(device);
        
        // create the request
        var url =
            $"https://{deviceInfo.ServerRelay}/www/" +
            $"{deviceInfo.FirmwareVersion}-{deviceInfo.UiLanguage}" +
            $"/kit/KitDevice.json";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        
        // send the request
        var response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        // parse the response
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        var root = json.RootElement;
        return JsonHelper.DeserializeJson(root);
    }
    
    /// <inheritdoc cref="GetKitDevicesAsync"/>
    public Dictionary<string, object> GetKitDevices(VeraAccountDevice device)
    {
        var asyncTask = Task.Run(async () => await GetKitDevicesAsync(device));
        // Wait for the task to complete and get the result
        return asyncTask.Result;
    }
}