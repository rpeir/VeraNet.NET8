// -----------------------------------------------------------------------
// <copyright file="VeraConnectionInfo.cs" company="Sebastien.warin.Fr">
//  Copyright 2012 - Sebastien.warin.fr
// </copyright>
// <author>Sebastien Warin, Rodrigo Peireso</author>
// -----------------------------------------------------------------------

using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace VeraNet
{
    /// <summary>
    /// Represents connection information to Vera Controller.
    /// This class is abstract and should be inherited to create a specific connection info.
    /// <seealso cref="VeraConnectionLocal"/>
    /// <seealso cref="VeraConnectionCloudUi7"/>
    /// <seealso cref="VeraConnectionCloudOld"/>
    /// </summary>
    [ComVisible(true)]
    [Guid("2A0FFBAA-28B8-4D7C-8719-2869B3A5E75C")]
    public abstract class VeraConnection
    {
        /// <summary>
        /// Http client to connect to the Vera controller.
        /// Each connection info should make its own configurations to the HttpClient.
        /// </summary>
        public HttpClient HttpClient { protected get; init; } = new();

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
        
        /// <summary>
        /// Gets a response from the Vera controller.
        /// </summary>
        /// <param name="uri">The URI to get the response from.</param>
        /// <returns>The response.</returns>
        public virtual async Task<HttpResponseMessage> GetWebResponse(string uri)
        {
            return await HttpClient.GetAsync(uri);
        }
    }
}
