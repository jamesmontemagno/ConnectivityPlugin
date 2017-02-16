using System;
using Plugin.Connectivity;

namespace ConnectivyTest.Net45
{
    class Program
    {
        // Simple test console to ensure that .net45 CrossConnectivity works.
        static void Main(string[] args)
        {
            var isConnected = CrossConnectivity.Current.IsConnected;

            var isReachable = CrossConnectivity.Current.IsReachable("http://www.github.com").Result;
            var isHostReachable = CrossConnectivity.Current.IsRemoteReachable("http://www.github.com").Result;

            Console.WriteLine($"IsConnected: {isConnected}");
            Console.WriteLine($"Is github Reachable: {isReachable}");
            Console.WriteLine($"Is remote port 80 on github reachable: {isHostReachable}");
            Console.WriteLine("press enter to close.");

            Console.ReadLine();
        }
    }
}
