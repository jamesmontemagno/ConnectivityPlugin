using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.Connectivity.Abstractions;
using Windows.Networking.Connectivity;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Networking;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using System.Threading;

namespace Plugin.Connectivity
{
    /// <summary>
    /// Connectivity Implementation for Windows
    /// </summary>
    public class ConnectivityImplementation : BaseConnectivity
    {
        bool isConnected;
        /// <summary>
        /// Default constructor
        /// </summary>
        public ConnectivityImplementation()
        {
            isConnected = IsConnected;
            NetworkInformation.NetworkStatusChanged += NetworkStatusChanged;
        }

        async void NetworkStatusChanged(object sender)
        {
            var previous = isConnected;
            var newConnected = IsConnected;

            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

            if (dispatcher != null)
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (previous != newConnected)
                        OnConnectivityChanged(new ConnectivityChangedEventArgs { IsConnected = newConnected });

                    OnConnectivityTypeChanged(new ConnectivityTypeChangedEventArgs { IsConnected = newConnected, ConnectionTypes = this.ConnectionTypes });
                });
            }
            else
            {
                if (previous != newConnected)
                    OnConnectivityChanged(new ConnectivityChangedEventArgs { IsConnected = newConnected });

                OnConnectivityTypeChanged(new ConnectivityTypeChangedEventArgs { IsConnected = newConnected, ConnectionTypes = this.ConnectionTypes });
            }
        }

        /// <summary>
        /// Gets if there is an active internet connection
        /// </summary>
        public override bool IsConnected
        {
            get
            {
                var profile = NetworkInformation.GetInternetConnectionProfile();
                if (profile == null)
                    isConnected = false;
                else
                {
                    var level = profile.GetNetworkConnectivityLevel();
                    isConnected = level != NetworkConnectivityLevel.None && level != NetworkConnectivityLevel.LocalAccess;
                }

                return isConnected;
            }
        }


		/// <summary>
		/// Tests if a host name is pingable
		/// </summary>
		/// <param name="host">The host name can either be a machine name, such as "java.sun.com", or a textual representation of its IP address (127.0.0.1)</param>
		/// <param name="timeout">Timeout</param>
		/// <returns></returns>
		public override async Task<bool> IsReachable(string host, TimeSpan timeout)
		{
			if (string.IsNullOrWhiteSpace(host))
				throw new ArgumentNullException(nameof(host));

			try
            {
                var serverHost = new HostName(host);
                using (var client = new StreamSocket())
                {
                    var cancellationTokenSource = new CancellationTokenSource();
                    cancellationTokenSource.CancelAfter(timeout);

                    await client.ConnectAsync(serverHost, "http").AsTask(cancellationTokenSource.Token);
                    return true;
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to reach: " + host + " Error: " + ex);
                return false;
            }
        }

		/// <summary>
		/// Tests if a remote uri is reachable
		/// </summary>
		/// <param name="uri">Full valid Uri to check for reachability</param>
		/// <param name="timeout">Timeout</param>
		public override async Task<bool> IsRemoteReachable(Uri uri, TimeSpan timeout)
		{
            
            try
            {
                using (var tcpClient = new StreamSocket())
                {
                    var cancellationTokenSource = new CancellationTokenSource();
                    cancellationTokenSource.CancelAfter(timeout);

                    await tcpClient.ConnectAsync(
                        new HostName(uri.Host),
                        uri.Port.ToString(),
                        SocketProtectionLevel.PlainSocket).AsTask(cancellationTokenSource.Token);

                    var localIp = tcpClient.Information.LocalAddress.DisplayName;
                    var remoteIp = tcpClient.Information.RemoteAddress.DisplayName;

                    tcpClient.Dispose();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to reach: {uri} Error: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Gets the list of all active connection types.
        /// </summary>
        public override IEnumerable<ConnectionType> ConnectionTypes
        {
            get
            {
                var networkInterfaceList = NetworkInformation.GetConnectionProfiles();
                foreach (var networkInterfaceInfo in networkInterfaceList.Where(networkInterfaceInfo => networkInterfaceInfo.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None))
                {
                    var type = ConnectionType.Other;

                    if (networkInterfaceInfo.NetworkAdapter != null)
                    {

                        switch (networkInterfaceInfo.NetworkAdapter.IanaInterfaceType)
                        {
                            case 6:
                                type = ConnectionType.Desktop;
                                break;
                            case 71:
                                type = ConnectionType.WiFi;
                                break;
                            case 243:
                            case 244:
                                type = ConnectionType.Cellular;
                                break;
                        }
                    }

                    yield return type;
                }
            }
        }

        /// <summary>
        /// Retrieves a list of available bandwidths for the platform.
        /// Only active connections.
        /// </summary>
        public override IEnumerable<UInt64> Bandwidths
        {
            get
            {
                var networkInterfaceList = NetworkInformation.GetConnectionProfiles();
                foreach (var networkInterfaceInfo in networkInterfaceList.Where(networkInterfaceInfo => networkInterfaceInfo.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None))
                {
                    UInt64 speed = 0;

                    if (networkInterfaceInfo.NetworkAdapter != null)
                    {
                        speed = (UInt64)networkInterfaceInfo.NetworkAdapter.OutboundMaxBitsPerSecond;
                    }

                    yield return speed;
                }
            }
        }
        private bool disposed = false;
        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        public override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    NetworkInformation.NetworkStatusChanged -= NetworkStatusChanged;
                }

                disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
