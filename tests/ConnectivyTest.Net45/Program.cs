using System;
using System.Threading.Tasks;
using Plugin.Connectivity;

namespace ConnectivyTest.Net45
{
    class Program
    {
        // Simple test console to ensure that .net45 CrossConnectivity works.
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var isConnected = CrossConnectivity.Current.IsConnected;

                var isReachable = await CrossConnectivity.Current.IsReachable("http://www.github.com");
                var isHostReachable = await CrossConnectivity.Current.IsRemoteReachable("http://www.github.com");

                Console.WriteLine($"IsConnected: {isConnected}");
                Console.WriteLine($"Is github Reachable: {isReachable}");
                Console.WriteLine($"Is remote port 80 on github reachable: {isHostReachable}");

                CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;

                Console.WriteLine("press enter to close.");

                Console.ReadLine();
            }).Wait();
            
        }

        private static void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            Console.WriteLine($"IsConnected {e.IsConnected}");
        }
    }
}
