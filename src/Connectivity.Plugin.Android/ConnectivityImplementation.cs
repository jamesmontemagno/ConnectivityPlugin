using Plugin.Connectivity.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.App;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Plugin.Connectivity
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ConnectivityImplementation : BaseConnectivity
    {
        private ConnectivityChangeBroadcastReceiver receiver;
        /// <summary>
        /// Default constructor
        /// </summary>
        public ConnectivityImplementation()
        {
            ConnectivityChangeBroadcastReceiver.ConnectionChanged = OnConnectivityChanged;
            ConnectivityChangeBroadcastReceiver.ConnectionTypeChanged = OnConnectivityTypeChanged;
            receiver = new ConnectivityChangeBroadcastReceiver();
            Application.Context.RegisterReceiver(receiver, new IntentFilter(ConnectivityManager.ConnectivityAction));
        }
        private ConnectivityManager connectivityManager;
        private WifiManager wifiManager;

        ConnectivityManager ConnectivityManager
        {
            get
            {
                if (connectivityManager == null || connectivityManager.Handle == IntPtr.Zero)
                    connectivityManager = (ConnectivityManager)(Application.Context.GetSystemService(Context.ConnectivityService));

                return connectivityManager;
            }
        }

        WifiManager WifiManager
        {
            get
            {
                if(wifiManager == null || wifiManager.Handle == IntPtr.Zero)
                    wifiManager = (WifiManager)(Application.Context.GetSystemService(Context.WifiService));

                return wifiManager;
            }
        }

        public static bool GetIsConnected(ConnectivityManager manager)
        {
            try
            {

                //When on API 21+ need to use getAllNetworks, else fall base to GetAllNetworkInfo
                //https://developer.android.com/reference/android/net/ConnectivityManager.html#getAllNetworks()
                if ((int)Android.OS.Build.VERSION.SdkInt >= 21)
                {
                    foreach (var network in manager.GetAllNetworks())
                    {
                        try
                        {
                            var info = manager.GetNetworkInfo(network);

                            if (info == null || !info.IsAvailable || info.Subtype.HasFlag(ConnectivityType.Dummy))
                                continue;

                            if (info.IsConnected)
                                return true;
                        }
                        catch
                        {
                            //there is a possibility, but don't worry
                        }
                    }
                }
                else
                {
                    foreach (var info in manager.GetAllNetworkInfo())
                    {
                        if (info == null || !info.IsAvailable || info.Subtype.HasFlag(ConnectivityType.Dummy))
                            continue;
                        
                        if (info.IsConnected)
                            return true;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to get connected state - do you have ACCESS_NETWORK_STATE permission? - error: {0}", e);
                return false;
            }
        }

        /// <summary>
        /// Gets if there is an active internet connection
        /// </summary>
        public override bool IsConnected => GetIsConnected(ConnectivityManager);


		/// <summary>
		/// Tests if a host name is pingable
		/// </summary>
		/// <param name="host">The host name can either be a machine name, such as "java.sun.com", or a textual representation of its IP address (127.0.0.1)</param>
		/// <param name="timespan">Timeout in milliseconds</param>
		/// <returns></returns>
		public override Task<bool> IsReachable(string host, TimeSpan timespan)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException(nameof(host));

            return Task.Run(() =>
            {
                bool reachable;
                try
                {
					var inetAddress = Java.Net.InetAddress.GetByName(host);

					//Is reachable only works with IP Addresses, so let's validate first.
					if(IPAddress.TryParse(host, out IPAddress address))
					{
						reachable = inetAddress.IsReachable((int)timespan.TotalMilliseconds);
					}
					else
					{
						//GetByName actually resolves out the host name, so it must be 
						reachable = true;
					}
                }
                catch (Java.Net.UnknownHostException ex)
                {
                    Debug.WriteLine("Unable to reach: " + host + " Error: " + ex);
                    reachable = false;
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine("Unable to reach: " + host + " Error: " + ex2);
                    reachable = false;
                }
                return reachable;
            });

        }

		/// <summary>
		/// Tests if a remote uri is reachable
		/// </summary>
		/// <param name="uri">Full valid Uri to check for reachability</param>
		/// <param name="timeout">Timeout in milliseconds.</param>
		/// <returns></returns>
		public override Task<bool> IsRemoteReachable(System.Uri uri, TimeSpan timeout)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            return Task.Run(() =>
            {
                try
                {
                    using (var clientDone = new ManualResetEvent(false))
                    {
                        var reachable = false;

                        var hostEntry = new DnsEndPoint(uri.Host, uri.Port);

                        using (var socket = new Socket(System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp))
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
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unable to reach: {uri} Error: {ex}");
                    return false;
                }
            });
        }

        /// <summary>
        /// Gets the list of all active connection types.
        /// </summary>
        public override IEnumerable<ConnectionType> ConnectionTypes
        {
            get
            {
                return GetConnectionTypes(ConnectivityManager);
            }
        }

        public static IEnumerable<ConnectionType> GetConnectionTypes(ConnectivityManager manager)
        {
            //When on API 21+ need to use getAllNetworks, else fall base to GetAllNetworkInfo
            //https://developer.android.com/reference/android/net/ConnectivityManager.html#getAllNetworks()
            if ((int)Android.OS.Build.VERSION.SdkInt >= 21)
            {
                foreach (var network in manager.GetAllNetworks())
                {
                    NetworkInfo info = null;
                    try
                    {
                        info = manager.GetNetworkInfo(network);
                    }
                    catch
                    {
                        //there is a possibility, but don't worry about it
                    }

                    if (info == null || !info.IsAvailable)
                        continue;

                    yield return GetConnectionType(info.Type);
                }
            }
            else
            {
                foreach (var info in manager.GetAllNetworkInfo())
                {
                    if (info == null || !info.IsAvailable)
                        continue;

                    yield return GetConnectionType(info.Type);
                }
            }

        }

        public static ConnectionType GetConnectionType(ConnectivityType connectivityType)
        {
            switch (connectivityType)
            {
                case ConnectivityType.Ethernet:
                    return ConnectionType.Desktop;
                case ConnectivityType.Wimax:
                    return ConnectionType.Wimax;
                case ConnectivityType.Wifi:
                    return ConnectionType.WiFi;
                case ConnectivityType.Bluetooth:
                    return ConnectionType.Bluetooth;
                case ConnectivityType.Mobile:
                case ConnectivityType.MobileDun:
                case ConnectivityType.MobileHipri:
                case ConnectivityType.MobileMms:
                    return ConnectionType.Cellular;
                case ConnectivityType.Dummy:
                    return ConnectionType.Other;
                default:
                    return ConnectionType.Other;
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
                try
                {
                    if (ConnectionTypes.Contains(ConnectionType.WiFi))
                        return new[] { (UInt64)WifiManager.ConnectionInfo.LinkSpeed * 1000000 };
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Unable to get connected state - do you have ACCESS_WIFI_STATE permission? - error: {0}", e);
                }

                return new UInt64[] { };
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



                    if (receiver != null)
                        Application.Context.UnregisterReceiver(receiver);

                    ConnectivityChangeBroadcastReceiver.ConnectionChanged = null;
                    if (wifiManager != null)
                    {
                        wifiManager.Dispose();
                        wifiManager = null;
                    }

                    if (connectivityManager != null)
                    {
                        connectivityManager.Dispose();
                        connectivityManager = null;
                    }

                }

                disposed = true;
            }

            base.Dispose(disposing);
        }


    }
}
