// -----------------------------------------------------------------------
// <copyright file="Room.cs" company="Sebastien.warin.Fr">
//  Copyright 2012 - Sebastien.warin.fr
// </copyright>
// <author>Sebastien Warin</author>
// -----------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace VeraNet.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Represent a room
    /// </summary>
    [ComVisible(true)]
    public class Room : VeraBaseObject
    {
        /// <summary>
        /// Gets the section identifier of this room.
        /// </summary>
        /// <value>
        /// The section identifier of this room.
        /// </value>
        public int SectionId { get; internal set; }

        /// <summary>
        /// Gets the section of this room.
        /// </summary>
        /// <value>
        /// The section of this room.
        /// </value>
        public Section Section
        {
            get
            {
                if (this.VeraController != null)
                {
                    return this.VeraController.Sections.Where(s => s.Id == this.SectionId).FirstOrDefault();
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the Vera devices in this room.
        /// </summary>
        /// <value>
        /// The Vera devices in this room.
        /// </value>
        public ObservableCollection<Device> Devices
        {
            get
            {
                if (this.VeraController != null)
                {
                    return new ObservableCollection<Device>(this.VeraController.Devices.Where(d => d.RoomId == this.Id));
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the scenes in this room.
        /// </summary>
        /// <value>
        /// The scenes in this room.
        /// </value>
        public ObservableCollection<Scene> Scenes
        {
            get
            {
                if (this.VeraController != null)
                {
                    return new ObservableCollection<Scene>(this.VeraController.Scenes.Where(s => s.RoomId == this.Id));
                }
                return null;
            }
        }

        internal override void InitializeProperties(Dictionary<string, object> values)
        {
            base.InitializeProperties(values);
            this.SectionId = Convert.ToInt32(values["section"]);
        }

        internal override void UpdateProperties(Dictionary<string, object> values)
        {
            base.UpdateProperties(values);
            this.UpdateProperty(values, "section", "SectionId", (v) => { this.SectionId = Convert.ToInt32(v); return true; });
        }

        /// <summary>
        /// Renames the name of this room.
        /// </summary>
        /// <param name="newName">The new room name.</param>
        /// <returns>Returns <c>True</c> if changed successfully, <c>False</c> otherwise.</returns>
        public async Task<bool> RenameAsync(string newName)
        {
            // Check if the new name is not empty
            if (string.IsNullOrEmpty(newName)) return false;
            
            // Create the request
            var uri = $"data_request?id=room&action=rename&room={this.Id}&name={newName}";
            var response = await this.VeraController.GetWebResponseAsync(uri);
            
            // Check if the request was successful
            var updated = response.Contains("OK");
            if (!updated) return false;
            
            // Update the name
            this.Name = newName;
            return true;
        }
        
        /// <inheritdoc cref="RenameAsync"/>
        public bool Rename(string newName)
        {
            var asyncTask = Task.Run(async () => await this.RenameAsync(newName));
            // Wait for the task to complete and get the result
            return asyncTask.Result;
        }
    }
    
}
