using Plugin.Connectivity.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;


namespace Plugin.Connectivity
{
    /// <summary>
    /// Implementation for Connectivity
    /// </summary>
    [Foundation.Preserve(AllMembers = true)]
    public class ConnectivityImplementation : BaseConnectivity
	{
		Task initialTask = null;
		/// <summary>
		/// Default constructor
		/// </summary>
		public ConnectivityImplementation()
		{
			//start an update on the background.
			initialTask = Task.Run(() => { UpdateConnected(false);});
			Reachability.ReachabilityChanged += ReachabilityChanged;
		}

		async void ReachabilityChanged(object sender, EventArgs e)
		{
			//Add in artifical delay so the connection status has time to change
			//else it will return true no matter what.
			await Task.Delay(100);
			UpdateConnected();
		}


		private bool isConnected;
		private NetworkStatus previousInternetStatus = NetworkStatus.NotReachable;
		private void UpdateConnected(bool triggerChange = true)
		{
			var remoteHostStatus = Reachability.RemoteHostStatus();
			var internetStatus = Reachability.InternetConnectionStatus();

			var previouslyConnected = isConnected;
			isConnected = (internetStatus == NetworkStatus.ReachableViaCarrierDataNetwork ||
							internetStatus == NetworkStatus.ReachableViaWiFiNetwork) ||
						  (remoteHostStatus == NetworkStatus.ReachableViaCarrierDataNetwork ||
							remoteHostStatus == NetworkStatus.ReachableViaWiFiNetwork);

			if (triggerChange)
			{
				if (previouslyConnected != isConnected || previousInternetStatus != internetStatus)
					OnConnectivityChanged(new ConnectivityChangedEventArgs { IsConnected = isConnected });

				var connectionTypes = this.ConnectionTypes.ToArray();
				OnConnectivityTypeChanged(new ConnectivityTypeChangedEventArgs { IsConnected = isConnected, ConnectionTypes = connectionTypes });
			}
			previousInternetStatus = internetStatus;
		}


		/// <summary>
		/// Gets if there is an active internet connection
		/// </summary>
		public override bool IsConnected 
		{
			get 
			{
                if (initialTask?.IsCompleted ?? true)
                {
                    UpdateConnected(false);
                    return isConnected;
                }

				//await for the initial run to complete
				initialTask.Wait();
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

			var cancellationTokenSource = new CancellationTokenSource();
			cancellationTokenSource.CancelAfter(timeout);
			try
			{
				return await Task.Run(() =>
				{
					try
					{
						return Reachability.IsHostReachable(host);
					}
					catch (Exception ex)
					{
						Debug.WriteLine($"IsReachable Error: {ex.Message}");
					}
					return false;
				}, cancellationTokenSource.Token);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"IsReachable Error: {ex.Message}");
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
					using (var clientDone = new ManualResetEvent(false))
					{
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
				var statuses = Reachability.GetActiveConnectionType();
                foreach (var status in statuses)
                {
                    switch (status)
                    {
                        case NetworkStatus.ReachableViaCarrierDataNetwork:
                            yield return ConnectionType.Cellular;
                            break;
                        case NetworkStatus.ReachableViaWiFiNetwork:
                            yield return ConnectionType.WiFi;
                            break;
                        default:
                            yield return ConnectionType.Other;
                            break;
                    }
                }
			}
		}
		/// <summary>
		/// Not supported on iOS
		/// </summary>
		public override IEnumerable<UInt64> Bandwidths => new UInt64[] { };

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
					Reachability.ReachabilityChanged -= ReachabilityChanged;
					Reachability.Dispose();
				}

				disposed = true;
			}

			base.Dispose(disposing);
		}

	}
}
