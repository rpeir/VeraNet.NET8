// -----------------------------------------------------------------------
// <copyright file="DeviceCategory.cs" company="Sebastien.warin.Fr">
//  Copyright 2012 - Sebastien.warin.fr
// </copyright>
// <author>Sebastien Warin</author>
// -----------------------------------------------------------------------

// Documentation: https://wiki.mios.com/index.php/Luup_Device_Categories

using System.Runtime.InteropServices;

namespace VeraNet.Objects
{

    /// <summary>
    /// Represent the Vera device's category
    /// </summary>
    [ComVisible((true))]
    public enum DeviceCategory
    {
        Interface = 1,
        // "urn:upnp-org:serviceId:Dimming1"
        DimmableLight = 2,
        // "urn:upnp-org:serviceId:SwitchPower1"
        Switch = 3,
        // "urn:micasaverde-com:serviceId:SecuritySensor1"
        SecuritySensor = 4,
        Thermostat = 5,
        Doorlock = 7,
        // "urn:micasaverde-com:serviceId:WindowCovering1"
        WindowCovering = 8,
        // "urn:micasaverde-com:serviceId:SceneController1"
        SceneController = 14,
        // "urn:micasaverde-com:serviceId:HumiditySensor1"
        HimiditySensor = 16,
        // urn:upnp-org:serviceId:TemperatureSensor1"
        TemperatureSensor = 17,
        // "urn:micasaverde-com:serviceId:EnergyMetering1"
        PowerMeter = 21
    }
}
