using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Plugin.Connectivity;

namespace ConnectivityTest.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{

			var test = CrossConnectivity.Current.IsConnected;
			global::Xamarin.Forms.Forms.Init();

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}
