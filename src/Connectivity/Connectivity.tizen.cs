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
using Tizen.Network.Connection;
using Tizen.System;

namespace Plugin.Connectivity
{
	/// <summary>
	/// Connectivity Implementation
	/// </summary>
	public class ConnectivityImplementation : BaseConnectivity
	{
		private static ConnectionProfile _connectionProfile;
		private static WiFiProfile _wiFiProfile;

		static bool isWiFiSupported = false;
		static bool isEthernetSupported = false;
		static bool isCellularSupported = false;
		static bool isBluetoothSupported = false;

		public ConnectivityImplementation()
		{
			Init();

			ConnectionManager.ConnectionTypeChanged += (s, e) =>
			{
				GetIsConnected();

				OnConnectivityChanged(new ConnectivityChangedEventArgs { IsConnected = isConnected });

				var connectionTypes = this.ConnectionTypes.ToArray();
				OnConnectivityTypeChanged(new ConnectivityTypeChangedEventArgs { IsConnected = isConnected, ConnectionTypes = connectionTypes });
			};
		}

		public void Init()
		{
			_connectionProfile = null;
			_wiFiProfile = null;

			Information.TryGetValue("http://tizen.org/feature/network.wifi", out isWiFiSupported);
			Information.TryGetValue("http://tizen.org/feature/network.ethernet", out isEthernetSupported);
			Information.TryGetValue("http://tizen.org/feature/network.telephony", out isCellularSupported);
			Information.TryGetValue("http://tizen.org/feature/network.bluetooth", out isBluetoothSupported);
		}

		public static async void SetUpMaxSpeed()
		{
			var list = await ConnectionProfileManager.GetProfileListAsync(ProfileListType.Registered);
			foreach (var item in list)
			{
				_connectionProfile = item;
				break;
			}
			_wiFiProfile = (WiFiProfile)_connectionProfile;
		}

		/// <summary>
		/// Retrieves a list of available bandwidths for the platform.
		/// Only active connections.
		/// </summary>
		public static IEnumerable<UInt64> GetBandWidths()
		{
			SetUpMaxSpeed();

			int _maxSpeed = 0;
			if (_wiFiProfile != null) 
			{
				if (ConnectionManager.WiFiState == ConnectionState.Connected)
				{
					try
					{
						_maxSpeed = _wiFiProfile.MaxSpeed;
					}
					catch (Exception e)
					{
						Debug.WriteLine("Unable to get connected state - error: {0}", e);
					}
				}
			}

			return new UInt64[] { (UInt64)_maxSpeed };
		}

		/// <summary>
		/// Bandwidths
		/// </summary>
		public override IEnumerable<UInt64> Bandwidths => GetBandWidths();

		/// <summary>
		/// Connection types
		/// </summary>
		public override IEnumerable<Abstractions.ConnectionType> ConnectionTypes
		{
			get
			{
				yield return isWiFiSupported ? Abstractions.ConnectionType.WiFi : Abstractions.ConnectionType.Other;
				yield return isCellularSupported ? Abstractions.ConnectionType.Cellular : Abstractions.ConnectionType.Other;
				yield return isBluetoothSupported ? Abstractions.ConnectionType.Bluetooth : Abstractions.ConnectionType.Other;
				yield return isEthernetSupported ? Abstractions.ConnectionType.Desktop : Abstractions.ConnectionType.Other;
			}
		}

		private bool isConnected;
		public bool GetIsConnected()
		{
			switch (ConnectionManager.CurrentConnection.Type)
			{
				case Tizen.Network.Connection.ConnectionType.WiFi:
					if (ConnectionState.Connected == ConnectionManager.CurrentConnection.State)
					{
						isConnected = true;
					}
					break;
				case Tizen.Network.Connection.ConnectionType.Cellular:
					if (ConnectionState.Connected == ConnectionManager.CurrentConnection.State)
					{
						isConnected = true;
					}
					break;
				default:
					isConnected = false;
					break;
			}
			return isConnected;
		}

		/// <summary>
		/// Is Connected
		/// </summary>
		public override bool IsConnected => GetIsConnected();


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
				catch (Exception ex)
				{
					Debug.WriteLine($"Unable to reach: {uri} Error: {ex}");
					return false;
				}
			});
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
					if (_wiFiProfile != null)
						_wiFiProfile.Dispose();

					if (_connectionProfile != null)
						_connectionProfile.Dispose();
				}

				disposed = true;
			}
			base.Dispose(disposing);
        }

		
	}
}
