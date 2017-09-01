using Plugin.Connectivity.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Plugin.Connectivity
{
    /// <summary>
    /// Connectivity Implementation
    /// </summary>
    public class ConnectivityImplementation : BaseConnectivity
    {
        private readonly IList<ConnectionType> connectionTypes = new List<ConnectionType>();
        private readonly IList<ulong> bandwidths = new List<ulong>();
        private bool isConnected;

		/// <summary>
		/// Default constructor
		/// </summary>
        public ConnectivityImplementation()
        {
            NetworkChange.NetworkAddressChanged += NetworkChangeOnNetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
            
            ResetConnections();
        }

        IEnumerable<NetworkInterface> Adapters => NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .ToArray();

        void UpdateIsConnected()
        {
            isConnected = NetworkInterface.GetIsNetworkAvailable();
        }

        void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                UpdateIsConnected();
            }
            else
            {
                isConnected = false;
            }
            
            OnConnectivityChanged(new ConnectivityChangedEventArgs() {IsConnected = isConnected});
        }

        void NetworkChangeOnNetworkAddressChanged(object sender, EventArgs eventArgs)
        {
            ResetConnections();
        }

        void ResetConnections()
        {
            connectionTypes.Clear();
            bandwidths.Clear();
            foreach (var networkInterface in Adapters)
            {
                connectionTypes.Add(ToConnectionTypes(networkInterface));

                bandwidths.Add((ulong) networkInterface.Speed);
            }

            UpdateIsConnected();

            OnConnectivityTypeChanged(new ConnectivityTypeChangedEventArgs
            {
                ConnectionTypes = ConnectionTypes,
                IsConnected = IsConnected
            });
        }

        ConnectionType ToConnectionTypes(NetworkInterface networkInterface)
        {
            switch (networkInterface.NetworkInterfaceType)
            {
                case NetworkInterfaceType.TokenRing:
                case NetworkInterfaceType.Ethernet:
                case NetworkInterfaceType.Ethernet3Megabit:
                case NetworkInterfaceType.IPOverAtm:
                case NetworkInterfaceType.FastEthernetT:
                case NetworkInterfaceType.GigabitEthernet:
                case NetworkInterfaceType.FastEthernetFx:
                case NetworkInterfaceType.GenericModem:
                    return ConnectionType.Desktop;

                case NetworkInterfaceType.Ppp:
                case NetworkInterfaceType.Fddi:
                case NetworkInterfaceType.Isdn:
                case NetworkInterfaceType.BasicIsdn:
                case NetworkInterfaceType.PrimaryIsdn:
                case NetworkInterfaceType.Tunnel:
                case NetworkInterfaceType.VeryHighSpeedDsl:
                case NetworkInterfaceType.AsymmetricDsl:
                case NetworkInterfaceType.RateAdaptDsl:
                case NetworkInterfaceType.SymmetricDsl:
                case NetworkInterfaceType.Loopback:
                case NetworkInterfaceType.Slip:
                case NetworkInterfaceType.Atm:
                case NetworkInterfaceType.MultiRateSymmetricDsl:
                case NetworkInterfaceType.HighPerformanceSerialBus:
                case NetworkInterfaceType.Unknown:
                    return ConnectionType.Other;

                case NetworkInterfaceType.Wireless80211:
                    return ConnectionType.WiFi;

                case NetworkInterfaceType.Wman:
                    return ConnectionType.Wimax;

                case NetworkInterfaceType.Wwanpp2:
                case NetworkInterfaceType.Wwanpp:
                    return ConnectionType.Cellular;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Bandwidths
        /// </summary>
        public override IEnumerable<ulong> Bandwidths => bandwidths;

        /// <summary>
        /// Connection types
        /// </summary>
        public override IEnumerable<ConnectionType> ConnectionTypes => connectionTypes;

        /// <summary>
        /// Is Connected
        /// </summary>
        public override bool IsConnected => isConnected;

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

			var pingRequest = new Ping();
            try
            {              
                var pingReply = await pingRequest.SendPingAsync(host, (int)timeout.TotalMilliseconds);

                if (pingReply != null && pingReply.Status == IPStatus.Success)
                {
                    return true;
                }
            }
            catch
            {
                // Suppressing catch here, if any exception is returned by Ping, consider it as Not reachable.
            }

            return false;
        }

		/// <summary>
		/// Tests if a remote uri is reachable
		/// </summary>
		/// <param name="uri">Full valid Uri to check for reachability</param>
		/// <param name="timeout">Timeout</param>
		public override Task<bool> IsRemoteReachable(Uri uri, TimeSpan timeout)
		{
			if (uri == null)
				throw new ArgumentNullException(nameof(uri));

			return Task.Run(() =>
			{
				try
				{
					var clientDone = new ManualResetEvent(false);
					
					var reachable = false;

					var hostEntry = new DnsEndPoint(uri.Host, uri.Port);

					using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp))
					{
						var socketEventArg = new SocketAsyncEventArgs { RemoteEndPoint = hostEntry };

						void handler(object sender, SocketAsyncEventArgs e)
						{
							reachable = e.SocketError == SocketError.Success;
							socketEventArg.Completed -= handler;
							clientDone.Set();
						};

						socketEventArg.Completed += handler;

						clientDone.Reset();

						socket.ConnectAsync(socketEventArg);

						clientDone.WaitOne(timeout);

						return reachable;
					}
					
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"Unable to reach: {uri} Error: {ex}");
					return false;
				}
			});
		}

		public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                NetworkChange.NetworkAddressChanged -= NetworkChangeOnNetworkAddressChanged;
                NetworkChange.NetworkAvailabilityChanged -= NetworkChange_NetworkAvailabilityChanged;
            }
        }
    }
}
