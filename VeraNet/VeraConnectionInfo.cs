// -----------------------------------------------------------------------
// <copyright file="VeraConnectionInfo.cs" company="Sebastien.warin.Fr">
//  Copyright 2012 - Sebastien.warin.fr
// </copyright>
// <author>Sebastien Warin, Rodrigo Peireso</author>
// -----------------------------------------------------------------------

using System.Net.Http;
namespace VeraNet
{
    /// <summary>
    /// Represents connection information to Vera Controller.
    /// This class is abstract and should be inherited to create a specific connection info.
    /// <seealso cref="VeraConnectionInfoLocal"/>
    /// <seealso cref="VeraConnectionInfoCloudUi7"/>
    /// <seealso cref="VeraConnectionInfoCloudOld"/>
    /// </summary>
    public abstract class VeraConnectionInfo
    {
        /// <summary>
        /// Http client to connect to the Vera controller.
        /// Each connection info should make its own configurations to the HttpClient.
        /// </summary>
        public HttpClient HttpClient { get; protected init; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a local or remote connection.
        /// </summary>
        /// <value>
        /// <c>true</c> if this is a local connection; otherwise, <c>false</c>.
        /// </value>
        public bool IsLocalConnection { get; init; }

        /// <summary>
        /// Gets the URL to connect to the Vera controller.
        /// </summary>
        protected abstract string GetUrl();
        
        public override string ToString()
        {
            return GetUrl();
        }
    }
}
