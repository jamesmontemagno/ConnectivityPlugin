using Xamarin.Forms;
using Plugin.Connectivity;
using System.Linq;
using System;

namespace ConnectivityTest
{
	public partial class ConnectivityTestPage : ContentPage
	{
		

		public ConnectivityTestPage()
		{
			InitializeComponent();
			connectivityButton.Clicked += async (sender, args) =>
			{
				
				try
				{
					connected.Text = CrossConnectivity.Current.IsConnected ? "Connected" : "No Connection";
					bandwidths.Text = "Bandwidths: ";
					foreach (var band in CrossConnectivity.Current.Bandwidths)
					{
						bandwidths.Text += band.ToString() + ", ";
					}
					connectionTypes.Text = "ConnectionTypes:  ";
					foreach (var band in CrossConnectivity.Current.ConnectionTypes)
					{
						connectionTypes.Text += band.ToString() + ", ";
					}

				}
				catch (Exception ex)
				{
					await DisplayAlert("Uh oh", "Something went wrong, but don't worry we captured for analysis! Thanks.", "OK");
				}

				try
				{
					canReach1.Text = await CrossConnectivity.Current.IsReachable(host.Text) ? "Reachable" : "Not reachable";

				}
				catch (Exception ex)
				{
					canReach1.Text = ex.Message;
				}

				try
				{
					canReach2.Text = await CrossConnectivity.Current.IsRemoteReachable(new Uri(host2.Text +  ":" + port.Text), TimeSpan.FromSeconds(5)) ? "Reachable" : "Not reachable";

				}
				catch (Exception ex)
				{
					canReach2.Text = ex.Message;
				}

				try
				{
					canReach3.Text = await CrossConnectivity.Current.IsRemoteReachable(host3.Text, TimeSpan.FromSeconds(5)) ? "Reachable" : "Not reachable";

				}
				catch (Exception ex)
				{
					canReach3.Text = ex.Message;
				}

				try
				{
					canReach4.Text = await CrossConnectivity.Current.IsReachable(host4.Text, TimeSpan.FromSeconds(5)) ? "Reachable" : "Not reachable";

				}
				catch (Exception ex)
				{
					canReach4.Text = ex.Message;
				}
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			CrossConnectivity.Current.ConnectivityChanged += CrossConnectivity_Current_ConnectivityChanged;
			CrossConnectivity.Current.ConnectivityTypeChanged += Current_ConnectivityTypeChanged;
		}

		private async void Current_ConnectivityTypeChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityTypeChangedEventArgs e)
		{
			var connectionTypes = "ConnectionTypes:  ";
			foreach (var band in e.ConnectionTypes)
			{
				connectionTypes += band.ToString() + ", ";
			}

			await DisplayAlert("Connectivity Types Changed", "Types: " + connectionTypes, "OK");

		}

		async void CrossConnectivity_Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs args)
		{
			await DisplayAlert("Connectivity Changed", "IsConnected: " + args.IsConnected.ToString(), "OK");

		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			CrossConnectivity.Current.ConnectivityChanged -= CrossConnectivity_Current_ConnectivityChanged;
			CrossConnectivity.Current.ConnectivityTypeChanged -= Current_ConnectivityTypeChanged;
		}
	}
}
