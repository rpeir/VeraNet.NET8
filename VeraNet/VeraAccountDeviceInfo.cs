// -----------------------------------------------------------------------
// <author>Rodrigo Peireso</author>
// -----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace VeraNet;

[ComVisible(true), Guid("414F090A-0C29-436D-BEB8-58FD114C71B1")]
public class VeraAccountDeviceInfo
{
    /// <summary>
    /// The device id, also known as <c>PK_Device</c> or <c>SerialNumber.</c>
    /// </summary>
    public string DeviceId { get; set; }
    
    /// <summary>
    /// The sever relay used to connect to the device.
    /// </summary>
    public string ServerRelay { get; set; }
    
    /// <summary>
    /// The mac address of the device,
    /// </summary>
    public string MacAddress { get; set; }
    
    /// <summary>
    /// Represents if the device is using 2G.
    /// </summary>
    public bool Using2G { get; set; }
    
    /// <summary>
    /// The external ip address of the device.
    /// </summary>
    /// <seealso cref="AccessiblePort"/>
    public string ExternalIp { get; set; }
    
    /// <summary>
    /// The accessible port of the device, of the external ip.
    /// </summary>
    /// <seealso cref="ExternalIp"/>
    public int AccessiblePort { get; set; }
    
    /// <summary>
    /// The internal ip address of the device.
    /// </summary>
    /// <seealso cref="LocalPort"/>
    public string InternalIp { get; set; }
    
    /// <summary>
    /// The time from that the device is alive.
    /// </summary>
    public DateTime AliveDate { get; set; }
    
    /// <summary>
    /// The firmware version of the device.
    /// </summary>
    public string FirmwareVersion { get; set; }
    
    /// <summary>
    /// The prior firmware version of the device.
    /// </summary>
    public string PriorFirmwareVersion { get; set; }
    
    /// <summary>
    /// The upgrade date of the device.
    /// </summary>
    public DateTime UpgradeDate { get; set; }
    
    // TODO: Check what this property is.
    public string Uptime { get; set; }
    
    /// <summary>
    /// The server device used.
    /// </summary>
    public string ServerDevice { get; set; }
    
    /// <summary>
    /// The event server used.
    /// </summary>
    public string ServerEvent { get; set; }
    
    /// <summary>
    /// The support server used.
    /// </summary>
    public string ServerSupport { get; set; }
    
    /// <summary>
    /// The storage server used.
    /// </summary>
    public string ServerStorage { get; set; }
    
    /// <summary>
    /// The Wifi SSID of the device.
    /// </summary>
    public string WifiSsid { get; set; }
    
    /// <summary>
    /// The time zone of the device.
    /// </summary>
    public string Timezone { get; set; }
    
    /// <summary>
    /// The local port of the device.
    /// </summary>
    /// <seealso cref="InternalIp"/>
    public int LocalPort { get; set; }
    
    /// <summary>
    /// The locale of the Z-Wave network.
    /// </summary>
    public string ZWaveLocale { get; set; }
    
    /// <summary>
    /// The version of the Z-Wave protocol used.
    /// </summary>
    public string ZWaveVersion { get; set; }
    
    // TODO: Check what this property is.
    public string BrandingFk { get; set; }
    
    /// <summary>
    /// The platform of the device.
    /// </summary>
    public string Platform { get; set; }
    
    /// <summary>
    /// The language of the device's ui.
    /// </summary>
    public string UiLanguage { get; set; }
    
    /// <summary>
    /// The skin of the device's ui.
    /// </summary>
    public string UiSkin { get; set; }
    
    /// <summary>
    /// Represents if the device has wifi.
    /// </summary>
    public bool HasWifi { get; set; }
    
    /// <summary>
    /// Represents if the device has alarm panel.
    /// </summary>
    public bool HasAlarmPanel { get; set; }
    
    /// <summary>
    /// UI version of the device.
    /// </summary>
    public int UiVersion { get; set; }
    
    // TODO: Check what this property is.
    public string EngineStatus { get; set; }
    
    // TODO: Check what this property is.
    public string DistributionBuild { get; set; }
    
    // TODO: Check what this property is.
    public string AccessPermissions { get; set; }
    
    /// <summary>
    /// The linux firmware version of the device.
    /// </summary>
    public int LinuxFirmware { get; set; }
    
    internal VeraAccountDeviceInfo() {}
}