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

            Console.WriteLine($"IsConnected: {isConnected}");
            Console.WriteLine("press enter to close.");

            Console.ReadLine();
        }
    }
}
