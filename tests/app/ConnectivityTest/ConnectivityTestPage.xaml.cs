using Xamarin.Forms;
using Plugin.Connectivity;
using System.Linq;

namespace ConnectivityTest
{
	public partial class ConnectivityTestPage : ContentPage
	{
		

		public ConnectivityTestPage()
		{
			InitializeComponent();

		}

		protected override async void OnAppearing()
		{
			base.OnAppearing(); 
			await DisplayAlert("Is Connected", CrossConnectivity.Current.IsConnected ? "YES" : "NO", "OK");

		}

		void HandleStart_Clicked(object sender, System.EventArgs e)
		{
			//CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
			CrossConnectivity.Current.ConnectivityTypeChanged += Current_ConnectivityTypeChanged;
		}

		void HandleStop_Clicked(object sender, System.EventArgs e)
		{
			//CrossConnectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
			CrossConnectivity.Current.ConnectivityTypeChanged -= Current_ConnectivityTypeChanged;

		}

		async void HandleIsConnected_Clicked(object sender, System.EventArgs e)
		{
			await DisplayAlert("Is Connected", CrossConnectivity.Current.IsConnected ? "YES" : "NO", "OK");
		}

		void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				await DisplayAlert("Is Connected", e.IsConnected ? "YES" : "NO", "OK");

			});
		}

		void Current_ConnectivityTypeChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityTypeChangedEventArgs e)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				var stuff = string.Empty;
				foreach (var i in e.ConnectionTypes)
					stuff += "/n" + i.ToString();
				
				await DisplayAlert("Is Connected", (e.IsConnected ? "YES" : "NO") + stuff, "OK");

			});
		}

        async void Types_Clicked(object sender, System.EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var stuff = string.Empty;
                foreach (var i in CrossConnectivity.Current.ConnectionTypes)
                    stuff += "/n" + i.ToString();

                await DisplayAlert("Is Connected", (CrossConnectivity.Current.IsConnected ? "YES" : "NO") + stuff, "OK");

            });
        }
    }
}
