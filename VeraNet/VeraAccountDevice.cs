// -----------------------------------------------------------------------
// <author>Rodrigo Peireso</author>
// -----------------------------------------------------------------------

using System;
using VeraNet.Objects;

namespace VeraNet;

/// <summary>
/// This class represents a device (Hub) in a Vera account.
/// It is based in the return of the Mios api.
/// It should be used to get the <see cref="VeraController"/>
/// It should be get from the <see cref="VeraCloudConnection"/> class.
/// </summary>
public class VeraAccountDevice
{
    /// <summary>
    /// The device id, also known as <c>PK_Device</c> or <c>SerialNumber.</c>
    /// </summary>
    public string DeviceId { get; set; }
    
    /// <summary>
    /// The device type PK, also known as <c>PK_DeviceType</c>. or <see cref="DeviceCategory"/>
    /// </summary>
    public DeviceCategory DeviceType { get; set; }
    
    /// <summary>
    /// THe device sub type PK, also known as <c>PK_DeviceSubType</c>.
    /// </summary>
    public string DeviceSubType { get; set; }
    
    /// <summary>
    /// The device MAC address.
    /// </summary>
    public string MacAddress { get; set; }
    
    /// <summary>
    /// The remote relay server used.
    /// </summary>
    public string ServerDevice { get; set; }
    
    /// <summary>
    /// The alternative remote relay server used.
    /// </summary>
    public string ServerDeviceAlt { get; set; }
        
    /// <summary>
    /// The Installation PK.
    /// It is returned by the Mios api, but not used in the VeraNet library.
    /// </summary>
    public string InstallationPk { get; set; }
    
    /// <summary>
    /// The device name.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Represents if the device is using 2G.
    /// </summary>
    public bool Using2G { get; set; }
    
    /// <summary>
    /// Represents the time the device was assigned.
    /// </summary>
    public DateTime DeviceAssigned { get; set; }
    
    /// <summary>
    /// Represents if the device is blocked.
    /// </summary>
    public bool Blocked { get; set; }
    
    internal VeraAccountDevice() {}
}