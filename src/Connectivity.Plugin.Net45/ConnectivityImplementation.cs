using Plugin.Connectivity.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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

        public ConnectivityImplementation()
        {
            NetworkChange.NetworkAddressChanged += NetworkChangeOnNetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;

            var adapters = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .ToArray();

            ResetConnections(adapters);

            isConnected = adapters.Any(p =>
                {
                    var connetionType = ToConnectionTypes(p);

                    switch (connetionType)
                    {
                        case ConnectionType.Cellular:
                        case ConnectionType.Bluetooth:
                        case ConnectionType.WiFi:
                        case ConnectionType.Desktop:
                        case ConnectionType.Wimax:
                            return p.OperationalStatus == OperationalStatus.Up;
                        case ConnectionType.Other:
                            return false;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
        }

        void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            isConnected = e.IsAvailable;
            OnConnectivityChanged(new ConnectivityChangedEventArgs() { IsConnected = isConnected });
        }

        void NetworkChangeOnNetworkAddressChanged(object sender, EventArgs eventArgs)
        {
            var adapters = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .ToArray();

            ResetConnections(adapters);
        }

        void ResetConnections(NetworkInterface[] adapters)
        {
            connectionTypes.Clear();
            bandwidths.Clear();
            foreach (var networkInterface in adapters)
            {
                connectionTypes.Add(ToConnectionTypes(networkInterface));

                bandwidths.Add((ulong) networkInterface.Speed);
            }

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
        public override Task<bool> IsReachable(string host, int msTimeout = 5000) => Task.FromResult(isConnected);

        /// <summary>
        /// IsReachable
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="msTimeout"></param>
        /// <returns></returns>
        public override Task<bool> IsRemoteReachable(string host, int port = 80, int msTimeout = 5000) => Task.FromResult(isConnected);

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
