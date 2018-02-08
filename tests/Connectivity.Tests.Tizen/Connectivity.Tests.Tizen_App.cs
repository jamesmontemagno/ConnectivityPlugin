using Tizen.Applications;
using ElmSharp;
using Plugin.Connectivity;
using System.Diagnostics;
using System;

namespace Connectivity.Tests.Tizen
{
	class App : CoreUIApplication
	{
		private ToastMessage toast;

		protected override void OnCreate()
		{
			base.OnCreate();
			Initialize();
		}

		void Initialize()
		{
			toast = new ToastMessage();

			Window window = new Window("ConnectivityTest")
			{
				AvailableRotations = DisplayRotation.Degree_0 | DisplayRotation.Degree_180 | DisplayRotation.Degree_270 | DisplayRotation.Degree_90
			};
			window.BackButtonPressed += (s, e) =>
			{
				Exit();
			};
			window.Show();

			var box = new Box(window)
			{
				AlignmentX = -1,
				AlignmentY = -1,
				WeightX = 1,
				WeightY = 1,
			};
			box.Show();

			var bg = new Background(window)
			{
				Color = Color.White
			};
			bg.SetContent(box);

			var conformant = new Conformant(window);
			conformant.Show();
			conformant.SetContent(bg);

			var IsConnected = new Button(window)
			{
				Text = "IsConnected",
				AlignmentX = -1,
				AlignmentY = -1,
				WeightX = 1,
			};
			IsConnected.Clicked += IsConnected_ClickedAsync;
			IsConnected.Show();
			box.PackEnd(IsConnected);

			var ConnectionTypes = new Button(window)
			{
				Text = "ConnectionTypes",
				AlignmentX = -1,
				AlignmentY = -1,
				WeightX = 1,
			};
			ConnectionTypes.Clicked += ConnectionTypes_ClickedAsync;
			ConnectionTypes.Show();
			box.PackEnd(ConnectionTypes);

			var CanReachRemote = new Button(window)
			{
				Text = "CanReachRemote",
				AlignmentX = -1,
				AlignmentY = -1,
				WeightX = 1,
			};
			CanReachRemote.Clicked += CanReachRemote_ClickedAsync;
			CanReachRemote.Show();
			box.PackEnd(CanReachRemote);
		}

		static void Main(string[] args)
		{
			Elementary.Initialize();
			Elementary.ThemeOverlay();
			App app = new App();
			app.Run(args);
		}

		private void PostToastMessage(string message)
		{
			toast.Message = message;
			toast.Post();
			Debug.WriteLine(message);
		}

		private void IsConnected_ClickedAsync(object sender, System.EventArgs e)
		{
			try
			{
				PostToastMessage("IsConnected : " + CrossConnectivity.Current.IsConnected);
			}
			catch (Exception ex)
			{
				PostToastMessage(ex.Message);
			}
		}

		private void ConnectionTypes_ClickedAsync(object sender, System.EventArgs e)
		{
			try
			{
				var types = CrossConnectivity.Current.ConnectionTypes;
				foreach(var type in types)
					PostToastMessage("ConnectionTypes : " + type);
			}
			catch (Exception ex)
			{
				PostToastMessage(ex.Message);
			}
		}

		private async void CanReachRemote_ClickedAsync(object sender, System.EventArgs e)
		{
			try
			{
				PostToastMessage("IsRemoteReachable : " + await CrossConnectivity.Current.IsRemoteReachable("http://www.github.com"));
			}
			catch (Exception ex)
			{
				PostToastMessage(ex.Message);
			}
		}
	}
}
