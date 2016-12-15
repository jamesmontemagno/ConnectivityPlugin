using System;
using NUnit.Framework;
using Plugin.Connectivity;
using System.Linq;

namespace Connectivity.Tests
{
    [TestFixture]
    public class TestsSample
    {

        [SetUp]
        public void Setup() { }


        [TearDown]
        public void Tear() { }

        [Test]
        public void IsConnected()
        {
            Assert.True(CrossConnectivity.Current.IsConnected, "Emulator was not connected");
        }

        [Test]
        public void ConnectionTypes()
        {
            var connected = CrossConnectivity.Current.IsConnected;
            var types = CrossConnectivity.Current.ConnectionTypes;
            Assert.IsTrue(!types.Contains(Plugin.Connectivity.Abstractions.ConnectionType.Other));

        }
        
        [Test]
        public async void CanReachRemote()
        {
            var canReach = await CrossConnectivity.Current.IsRemoteReachable("google.com");
            Assert.IsTrue(canReach);
        }

        
    }
}