using Plugin.Connectivity.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Plugin.Connectivity
{
    /// <summary>
    /// Connectivity Implementation
    /// </summary>
    public class ConnectivityImplementation : BaseConnectivity
    {
        readonly IList<ConnectionType> connectionTypes = new List<ConnectionType>();
        readonly IList<ulong> bandwidths = new List<ulong>();
        bool isConnected;


        /// <summary>
        /// Initializes a new instance of the <see cref="T:Plugin.Connectivity.ConnectivityImplementation"/> class.
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
        /// Is Reachable
        /// </summary>
        /// <param name="host"></param>
        /// <param name="msTimeout"></param>
        /// <returns></returns>
        public override async Task<bool> IsReachable(string host, int msTimeout = 5000)
        {
            var pingRequest = new Ping();
            try
            {
                var trimmedHost = host.Replace("http://", string.Empty).
                  Replace("https://", string.Empty).
                  TrimEnd('/');


                var pingReply = await pingRequest.SendPingAsync(trimmedHost, msTimeout);

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
        /// IsReachable
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="msTimeout"></param>
        /// <returns></returns>
        public override async Task<bool> IsRemoteReachable(string host, int port = 80, int msTimeout = 5000)
        {
            var trimmedHost = host.Replace("http://www.", string.Empty).
              Replace("http://", string.Empty).
              Replace("https://www.", string.Empty).
              Replace("https://", string.Empty).
              TrimEnd('/');
            try
            {
                using (var client = new TcpClient())
                {
                    client.ReceiveTimeout = msTimeout;
                    await client.ConnectAsync(trimmedHost, port);
                }

                return true;
            }
            catch
            {
                return false;
            }
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
