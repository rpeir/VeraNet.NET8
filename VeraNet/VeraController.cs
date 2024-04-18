﻿// -----------------------------------------------------------------------
// <copyright file="VeraController.cs" company="Sebastien.warin.Fr">
//  Copyright 2012 - Sebastien.warin.fr
// </copyright>
// <author>Sebastien Warin, Rodrigo Peireso</author>
// -----------------------------------------------------------------------

// Vera UI API documentation : http://wiki.micasaverde.com/index.php/UI_Simple
namespace VeraNet
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Reflection;
    using System.Threading;
    using System.Text.Json;
    using VeraNet.Objects;

    /// <summary>
    /// Represent the Vera controller.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class VeraController : IDisposable
    {
        private const int REQUEST_INTERVAL_PAUSE_MS = 100;
        private const int DEFAULT_MINIMAL_DELAY_MS = 500;
        private const int DEFAULT_TIMEOUT_SEC = 10;

        private HttpClient _httpClient;
        private CancellationTokenSource _cancellationTokenSource;

        private Thread _thrListener = null;
        private Dictionary<int, Type> _deviceTypes = null;

        /// <summary>
        /// Occurs when data is sent to the Vera.
        /// </summary>
        public event EventHandler<VeraDataSentEventArgs> DataSent;

        /// <summary>
        /// Occurs when data is received from the Vera.
        /// </summary>
        public event EventHandler<VeraDataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Occurs when error occurred.
        /// </summary>
        public event EventHandler<VeraErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>
        /// Occurs when Z-Wave device is updated.
        /// </summary>
        public event EventHandler<DeviceUpdatedEventArgs> DeviceUpdated;

        /// <summary>
        /// Occurs when scene is updated.
        /// </summary>
        public event EventHandler<SceneUpdatedEventArgs> SceneUpdated;

        /// <summary>
        /// Occurs when the house mode is changed.
        /// </summary>
        public event EventHandler<HouseModeChangedEventArgs> HouseModeChanged;

        /// <summary>
        /// Gets a value indicating whether this controller is listening for changes.
        /// </summary>
        /// <value>
        /// <c>true</c> if this controller is listening for changes; otherwise, <c>false</c>.
        /// </value>
        public bool IsListening { get; private set; }

        /// <summary>
        /// Gets or sets the connection information.
        /// </summary>
        /// <value>
        /// The connection information.
        /// </value>
        public VeraConnectionInfo ConnectionInfo { get; set; }

        /// <summary>
        /// Gets or sets the request minimal delay.
        /// </summary>
        /// <value>
        /// The request minimal delay.
        /// </value>
        public TimeSpan RequestMinimalDelay { get; set; }

        /// <summary>
        /// Gets or sets the request timeout.
        /// </summary>
        /// <value>
        /// The request timeout.
        /// </value>
        public TimeSpan RequestTimeout { get; set; }

        /// <summary>
        /// Gets or sets the Vera serial number.
        /// </summary>
        /// <value>
        /// The Vera serial number.
        /// </value>
        public string SerialNumber { get; private set; }

        /// <summary>
        /// Gets or sets the Vera device version.
        /// </summary>
        /// <value>
        /// The Vera device version.
        /// </value>
        public string Version { get; private set; }

        /// <summary>
        /// Gets or sets the Vera model.
        /// </summary>
        /// <value>
        /// The Vera model.
        /// </value>
        public string Model { get; private set; }

        /// <summary>
        /// Gets or sets the temperature unit.
        /// </summary>
        /// <value>
        /// The temperature unit.
        /// </value>
        public string TemperatureUnit { get; private set; }

        /// <summary>
        /// Gets or sets the last update.
        /// </summary>
        /// <value>
        /// The last update.
        /// </value>
        public DateTime LastUpdate { get; private set; }

        /// <summary>
        /// Gets the current load time.
        /// </summary>
        /// <value>
        /// The current load time.
        /// </value>
        public long CurrentLoadTime { get; private set; }

        /// <summary>
        /// Gets the current data version.
        /// </summary>
        /// <value>
        /// The current data version.
        /// </value>
        public long CurrentDataVersion { get; private set; }

        /// <summary>
        /// Gets or sets the state of the current.
        /// </summary>
        /// <value>
        /// The state of the current.
        /// </value>
        public VeraState CurrentState { get; private set; }

        /// <summary>
        /// Gets or sets the current comment.
        /// </summary>
        /// <value>
        /// The current comment.
        /// </value>
        public string CurrentComment { get; private set; }

        /// <summary>
        /// Gets or sets the house's mode.
        /// </summary>
        /// <value>
        /// The house's mode.
        /// </value>
        public VeraHouseMode HouseMode { get; private set; }

        /// <summary>
        /// Gets the sections.
        /// </summary>
        /// <value>
        /// The sections.
        /// </value>
        public ObservableCollection<Section> Sections { get; private set; }

        /// <summary>
        /// Gets the rooms.
        /// </summary>
        /// <value>
        /// The rooms.
        /// </value>
        public ObservableCollection<Room> Rooms { get; private set; }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <value>
        /// The categories.
        /// </value>
        public ObservableCollection<Category> Categories { get; private set; }

        /// <summary>
        /// Gets the scenes.
        /// </summary>
        /// <value>
        /// The scenes.
        /// </value>
        public ObservableCollection<Scene> Scenes { get; private set; }

        /// <summary>
        /// Gets or sets the devices.
        /// </summary>
        /// <value>
        /// The devices.
        /// </value>
        public ObservableCollection<Device> Devices { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="VeraController"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection information.</param>
        /// <param name="startListener">if set to <c>true</c> to listen the changes on Vera device.</param>
        public VeraController(VeraConnectionInfo connectionInfo, bool startListener = false)
        {
            this.Sections = new ObservableCollection<Section>();
            this.Rooms = new ObservableCollection<Room>();
            this.Categories = new ObservableCollection<Category>();
            this.Scenes = new ObservableCollection<Scene>();
            this.Devices = new ObservableCollection<Device>();
            this.IsListening = false;
            this.ConnectionInfo = connectionInfo;
            this._httpClient = connectionInfo.HttpClient;
            this.RequestMinimalDelay = TimeSpan.FromMilliseconds(DEFAULT_MINIMAL_DELAY_MS);
            this.RequestTimeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT_SEC);
            this.LoadDeviceTypes();
            if (startListener)
            {
                this.StartListener();
            }
        }

        /// <summary>
        /// Starts the listener to listen the changes on Vera device.
        /// </summary>
        /// <exception cref="Exception">VeraController not connected !</exception>
        public void StartListener()
        {
            if (!this.IsListening)
            {
                if (this.TestIfAlive())
                {
                    if (this.CurrentDataVersion == 0 && this.CurrentLoadTime == 0)
                    {
                        this.DemandFullRequest();
                    }
                    this.IsListening = true;
                    this._thrListener = new Thread(new ThreadStart(RequestVeraWorker));
                    this._thrListener.Start();
                }
                else
                {
                    throw new Exception("VeraController not connected !");
                }
            }
        }

        /// <summary>
        /// Stops the listener.
        /// </summary>
        public void StopListener()
        {
            if (this.IsListening)
            {
                this.IsListening = false;
                try
                {
                    this._thrListener.Abort();
                }
                catch { }
            }
        }

        /// <summary>
        /// Tests if the Vera is alive.
        /// </summary>
        /// <returns></returns>
        public bool TestIfAlive()
        {
            try
            {
                return this.GetWebResponse("data_request?id=lu_alive").Equals("OK");
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.StopListener();
        }

        /// <summary>
        /// Request a full request.
        /// </summary>
        public void DemandFullRequest()
        {
            this.CurrentDataVersion = 0;
            this.CurrentLoadTime = 0;
        }

        /// <summary>
        /// Waits until the full request is completed.
        /// </summary>
        /// <exception cref="Exception">
        /// The Vera is not alive !
        /// or
        /// Unable to perform a full request when the listener is running. Call StopListener() before !
        /// </exception>
        public void WaitForFullRequest()
        {
            if (!this.IsListening)
            {
                if (this.TestIfAlive())
                {
                    this.DemandFullRequest();
                    this.RequestVera();
                }
                else
                {
                    throw new Exception("The Vera is not alive !");
                }
            }
            else
            {
                throw new Exception("Unable to perform a full request when the listener is running. Call StopListener() before !");
            }
        }

        internal string GetWebResponse(string uri, bool throwException = false)
        {
            var asyncTask = Task.Run(async () => await GetWebResponseAsync(uri, throwException));
            // Wait for the task to complete and get the result
            return asyncTask.Result;
        }

        internal async Task<string> GetWebResponseAsync(string uri, bool throwException = false)
        {
            try
            {
                // Send request and get response
                HttpResponseMessage response = await _httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                // DateSent
                if (DataSent != null)
                {
                    DataSent(this,
                        new VeraDataSentEventArgs()
                            { Length = response.Content.Headers.ContentLength ?? 0, Uri = uri });
                }

                // Read response content
                var strResponse = await response.Content.ReadAsStringAsync();

                return strResponse;
            }
            catch (Exception ex)
            {
                if (throwException && ex is not TaskCanceledException && this.IsListening)
                {
                    throw;
                }

                return string.Empty;
            }
        }

        private void RequestVeraWorker()
        {
            int errorCount = 0;
            while (this.IsListening)
            {
                try
                {
                    this.RequestVera();
                    this.RequestHouseMode();
                    errorCount = 0;
                    Thread.Sleep(REQUEST_INTERVAL_PAUSE_MS);
                }
                catch (Exception ex)
                {
                    if (this.IsListening)
                    {
                        if (this.ErrorOccurred != null)
                        {
                            this.ErrorOccurred(this, new VeraErrorOccurredEventArgs(ex));
                        }

                        if (errorCount < 16)
                        {
                            errorCount = errorCount == 0 ? 1 : errorCount * 2;
                        }

                        Thread.Sleep(2000 * errorCount);
                    }
                }
            }
        }

        /// <summary>
        /// Requests the current house mode.
        /// </summary>
        /// <returns></returns>
        public VeraHouseMode RequestHouseMode()
        {
            VeraHouseMode houseMode =
                (VeraHouseMode)Convert.ToInt32(this.GetWebResponse("data_request?id=variableget&Variable=Mode"));
            if (houseMode != this.HouseMode)
            {
                var eventArgs = new HouseModeChangedEventArgs { NewMode = houseMode, OldMode = this.HouseMode };
                this.HouseMode = houseMode;
                this.HouseModeChanged?.Invoke(this, eventArgs);
            }

            return houseMode;
        }

        /// <summary>
        /// Sets the house mode.
        /// </summary>
        /// <param name="houseMode">The house mode.</param>
        /// <returns></returns>
        public bool SetHouseMode(VeraHouseMode houseMode)
        {
            return this.GetWebResponse(
                "data_request?id=lu_action&serviceId=urn:micasaverde-com:serviceId:HomeAutomationGateway1&action=SetHouseMode&Mode=" +
                ((int)houseMode).ToString()).Contains("<OK>OK</OK>");
        }

        private void RequestVera()
        {
            // Generate URI and get HTTP response content
            string strResponse = this.GetWebResponse(this.GetRequestUri(), true);

            // Check response
            if (string.IsNullOrEmpty(strResponse))
            {
                return;
            }

            // Deserialize JSON
            Dictionary<string, object> jsonResponse = DeserializeJson(strResponse);

            // Clearing datas
            if (this.CurrentLoadTime == 0)
            {
                this.Sections.Clear();
                this.Rooms.Clear();
                this.Categories.Clear();
                this.Scenes.Clear();
                this.Devices.Clear();
            }

            // Load Vera Objects
            this.LoadVeraObjects<Section>(jsonResponse, "sections", this.Sections);
            this.LoadVeraObjects<Room>(jsonResponse, "rooms", this.Rooms);
            this.LoadVeraObjects<Category>(jsonResponse, "categories", this.Categories);
            this.LoadVeraObjects<Scene>(jsonResponse, "scenes", this.Scenes);
            this.LoadVeraObjects<Device>(jsonResponse, "devices", this.Devices, (item) =>
            {
                var categoryId = Convert.ToInt32(item["category"]);
                if (this._deviceTypes.ContainsKey(categoryId))
                {
                    return Activator.CreateInstance(this._deviceTypes[categoryId]) as Device;
                }
                else
                {
                    return new Device();
                }
            });

            // Process metadatas
            if (jsonResponse.ContainsKey("full") && (int)jsonResponse["full"] == 1)
            {
                this.Model = jsonResponse["model"].ToString();
                this.SerialNumber = jsonResponse["serial_number"].ToString();
                this.TemperatureUnit = jsonResponse["temperature"].ToString();
                this.Version = jsonResponse["version"].ToString();
            }

            if (jsonResponse.ContainsKey("state"))
            {
                this.CurrentState = StateUtils.GetStateFromCode(Convert.ToInt32(jsonResponse["state"]));
                ;
            }

            if (jsonResponse.ContainsKey("comment"))
            {
                this.CurrentComment = jsonResponse["comment"].ToString();
            }

            if (jsonResponse.ContainsKey("loadtime"))
            {
                this.CurrentLoadTime = Convert.ToInt64(jsonResponse["loadtime"]);
            }

            if (jsonResponse.ContainsKey("dataversion"))
            {
                this.CurrentDataVersion = Convert.ToInt64(jsonResponse["dataversion"]);
            }

            if (jsonResponse.ContainsKey("mode"))
            {
                this.HouseMode = (VeraHouseMode)Convert.ToInt32(jsonResponse["mode"]);
            }

            // Update OK
            this.LastUpdate = DateTime.Now;

            // Raise DataReceived event
            if (this.DataReceived != null)
            {
                this.DataReceived(this, new VeraDataReceivedEventArgs()
                {
                    DataVersion = this.CurrentDataVersion,
                    LoadTime = this.CurrentLoadTime,
                    Length = strResponse.Length,
                    RawData = strResponse
                });
            }
        }

        private void LoadVeraObjects<TObject>(Dictionary<string, object> jsonValues, string jsonKey,
            ObservableCollection<TObject> listToLoad, Func<Dictionary<string, object>, TObject> createObject = null)
            where TObject : VeraBaseObject, new()
        {
            if (jsonValues.ContainsKey(jsonKey))
            {
                foreach (Dictionary<string, object> item in (object[])jsonValues[jsonKey])
                {
                    var obj = listToLoad.SingleOrDefault(s => s.Id.ToString() == item["id"].ToString());
                    if (obj == null)
                    {
                        if (createObject == null)
                        {
                            obj = new TObject();
                        }
                        else
                        {
                            obj = createObject(item);
                        }

                        obj.VeraController = this;
                        obj.InitializeProperties(item);
                        listToLoad.Add(obj);
                    }
                    else
                    {
                        obj.UpdateProperties(item);
                    }

                    // Raise events
                    if (this.DeviceUpdated != null && obj is Device)
                    {
                        this.DeviceUpdated(this, new DeviceUpdatedEventArgs(obj as Device));
                    }
                    else if (this.SceneUpdated != null && obj is Scene)
                    {
                        this.SceneUpdated(this, new SceneUpdatedEventArgs(obj as Scene));
                    }
                }
            }
        }

        private void LoadDeviceTypes()
        {
            this._deviceTypes = new Dictionary<int, Type>();
            Type deviceType = typeof(Device);
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in asm.GetTypes())
                {
                    if (type != deviceType && type.IsSubclassOf(deviceType))
                    {
                        var attrs = type.GetCustomAttributes(typeof(VeraDeviceAttribute), false);
                        if (attrs != null && attrs.Length > 0)
                        {
                            var category = ((VeraDeviceAttribute)attrs[0]).Category;
                            if (!this._deviceTypes.ContainsKey((int)category))
                            {
                                this._deviceTypes.Add((int)category, type);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the SData request URI, to get multiple data from Vera.
        /// </summary>
        private string GetRequestUri()
        {
            return string.Format("data_request?id=lu_sdata&loadtime={0}&dataversion={1}&minimumdelay={2}&timeout={3}",
                this.CurrentLoadTime, this.CurrentDataVersion, this.RequestMinimalDelay.TotalMilliseconds,
                this.RequestTimeout.TotalSeconds);
        }

        /// <summary>
        /// Helper method to deserialize Vera JSON responses.
        /// This method was created to maintain the original code structure.
        /// </summary>
        /// <param name="jsonString">The JSON response stringified</param>
        /// <returns>
        /// A dictionary with the JSON structure.
        /// The possible object values are the referenced in the <see cref="GetValue"/> method
        /// </returns>
        /// <author>Rodrigo Peireso</author>
        private static Dictionary<string, object> DeserializeJson(string jsonString)
        {
            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                // Access the root element
                JsonElement root = document.RootElement;

                // Create a dictionary to hold the deserialized JSON
                Dictionary<string, object> dictionary = new Dictionary<string, object>();

                // Iterate over each property of the JSON object
                foreach (JsonProperty property in root.EnumerateObject())
                {
                    // Convert the JSON element value to its corresponding native type
                    object value = GetValue(property.Value);

                    // Add the property name and its native type value to the dictionary
                    dictionary.Add(property.Name, value);
                }

                return dictionary;
            }
        }

        /// <summary>
        /// Helper method to convert JSON elements to native types.
        /// It is used with the DeserializeJson method to recursively convert JSON elements to native types.
        /// </summary>
        /// <param name="element">The json element to be converted.</param>
        /// <returns>Can return native types such as number (int, double, long, decimal), bool, string or array objects.</returns>
        /// <author>Rodrigo Peireso</author>
        private static object GetValue(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Number:
                    if (element.TryGetInt32(out int intValue))
                    {
                        return intValue;
                    }
                    else if (element.TryGetDouble(out double doubleValue))
                    {
                        return doubleValue;
                    }
                    else if (element.TryGetInt64(out long longValue))
                    {
                        return longValue;
                    }
                    else if (element.TryGetDecimal(out decimal decimalValue))
                    {
                        return decimalValue;
                    }

                    break;
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.Array:
                    List<object> array = new List<object>();
                    foreach (JsonElement item in element.EnumerateArray())
                    {
                        array.Add(GetValue(item));
                    }

                    return array.ToArray();
                case JsonValueKind.Object:
                    return DeserializeJson(element.GetRawText());
            }

            // Fallback to string representation if value kind is not recognized
            return element.ToString();
        }

        ///<inheritdoc cref="CreateRoomAsync"/>
        public Room CreateRoom(string name)
        {
            var asyncTask = Task.Run(async () => await CreateRoomAsync(name));
            // Wait for the task to complete and get the result
            return asyncTask.Result;
        }

        // TODO: Maybe change to throw exception instead of return null
        /// <summary>
        /// Creates a room in the Vera controller.
        /// </summary>
        /// <param name="name">The name of the room.</param>
        /// <returns>Returns a <see cref="Room"/> instance if created successfully, <c>null</c> otherwise.</returns>
        public async Task<Room> CreateRoomAsync(string name)
        {
            // validate the name
            if (string.IsNullOrEmpty(name)) return null;

            // create the request URI
            var uri = $"data_request?id=room&action=create&name={name}";

            // send the request
            var response = await GetWebResponseAsync(uri);

            // check if the room was created
            if (!response.Contains("OK")) return null;
            
            // update the rooms list
            RequestVera();
            
            // return the created room
            return this.Rooms.FirstOrDefault(room => room.Name == name);
        }

        /// <summary>
        /// Deletes a room from the Vera controller.
        /// </summary>
        /// <param name="room">The room to be deleted.</param>
        /// <returns>Returns <c>True</c> if the room is sucessfully removed.</returns>
        public async Task<bool> DeleteRoomAsync(Room room)
        {
            // create the request URI
            var uri = $"data_request?id=room&action=delete&room={room.Id}";

            // send the request
            var response = await GetWebResponseAsync(uri);

            // check if the object was deleted
            var isDeleted = response.Contains("OK");
            if (isDeleted)
            {
                this.Rooms.Remove(room);
            }

            return isDeleted;
        }

        ///<inheritdoc cref="DeleteRoomAsync"/>
        public bool DeleteRoom(Room room)
        {
            var asyncTask = Task.Run(async () => await DeleteRoomAsync(room));
            // Wait for the task to complete and get the result
            return asyncTask.Result;
        }
        
    }
}
