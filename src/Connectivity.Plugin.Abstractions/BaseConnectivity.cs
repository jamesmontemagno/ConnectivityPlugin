using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.Connectivity.Abstractions
{
	/// <summary>
	/// Base class for all connectivity classes
	/// </summary>
	public abstract class BaseConnectivity : IConnectivity, IDisposable
	{
		/// <summary>
		/// Gets if there is an active internet connection
		/// </summary>
		public abstract bool IsConnected
		{
			get;
		}

		/// <summary>
		/// Tests if a host name is pingable
		/// </summary>
		/// <param name="host">The host name can either be a machine name, such as "java.sun.com", or a textual representation of its IP address (127.0.0.1)</param>
		/// <param name="msTimeout">Timeout</param>
		/// <returns></returns>
		public Task<bool> IsReachable(string host, int msTimeout = 5000) =>
			IsReachable(host, TimeSpan.FromMilliseconds(msTimeout));

		/// <summary>
		/// Tests if a host name is pingable
		/// </summary>
		/// <param name="host">The host name can either be a machine name, such as "java.sun.com", or a textual representation of its IP address (127.0.0.1)</param>
		/// <param name="timeout">Timeout</param>
		/// <returns></returns>
		public abstract Task<bool> IsReachable(string host, TimeSpan timeout);

		/// <summary>
		/// <summary>
		/// Tests if a remote uri is reachable
		/// </summary>
		/// <param name="uri">Full valid Uri to check for reachability</param>
		/// <param name="msTimeout">Timeout in milliseconds</param>
		/// <returns></returns>
		[Obsolete("This method is now obsolete and replaced with IsRemoteReachable that take in a full Uri. This method now ignores the port and acts like a full Uri.")]
		public Task<bool> IsRemoteReachable(string uri, int port = 80, int msTimeout = 5000) =>
			IsRemoteReachable(uri, TimeSpan.FromMilliseconds(msTimeout));

		/// <summary>
		/// Tests if a remote uri is reachable
		/// </summary>
		/// <param name="uri">Full valid Uri to check for reachability</param>
		/// <param name="timeout">Timeout</param>
		/// <returns></returns>
		public Task<bool> IsRemoteReachable(string uri, TimeSpan timeout)
		{
			if (uri == null)
				throw new ArgumentNullException(nameof(uri));

			Uri newUri = null;

			try
			{
				newUri = new Uri(uri, UriKind.Absolute);

			}
			catch (Exception ex)
			{
				throw new ArgumentException("You must pass in a corretly formated url.", nameof(uri), ex);
			}

			return IsRemoteReachable(newUri, timeout);
		}

		/// <summary>
		/// Tests if a remote uri is reachable
		/// </summary>
		/// <param name="uri">Full valid Uri to check for reachability</param>
		/// <param name="timeout">Timeout.</param>
		/// <returns></returns>
		public abstract Task<bool> IsRemoteReachable(Uri uri, TimeSpan timeout);

		/// <summary>
		/// Gets the list of all active connection types.
		/// </summary>
		public abstract IEnumerable<ConnectionType> ConnectionTypes
        {
            get;
        }

        /// <summary>
        /// Retrieves a list of available bandwidths for the platform.
        /// Only active connections.
        /// </summary>
        public abstract IEnumerable<ulong> Bandwidths
        {
            get;
        }

        /// <summary>
        /// When connectivity changes
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnConnectivityChanged(ConnectivityChangedEventArgs e) =>
            ConnectivityChanged?.Invoke(this, e);

        /// <summary>
        /// When connectivity type changes
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnConnectivityTypeChanged(ConnectivityTypeChangedEventArgs e) =>
            ConnectivityTypeChanged?.Invoke(this, e);

        /// <summary>
        /// Connectivity event
        /// </summary>
        public event ConnectivityChangedEventHandler ConnectivityChanged;

        /// <summary>
        /// Connectivity type changed event
        /// </summary>
        public event ConnectivityTypeChangedEventHandler ConnectivityTypeChanged;


        /// <summary>
        /// Dispose of class and parent classes
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose up
        /// </summary>
        ~BaseConnectivity()
        {
            Dispose(false);
        }
        private bool disposed = false;
        /// <summary>
        /// Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose only
                }

                disposed = true;
            }
        }
	}
}
