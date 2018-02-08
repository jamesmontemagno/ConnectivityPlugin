using System;
using System.Reflection;
using Android.App;
using Android.Runtime;
using Xamarin.Android.NUnitLite;

namespace Connectivity.Tests.Droid
{
    [Instrumentation(Name = "app.tests.TestInstrumentation")]
    public class TestInstrumentation : TestSuiteInstrumentation
    {
        protected TestInstrumentation(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        protected override void AddTests()
        {
            AddTest(Assembly.GetExecutingAssembly());
        }
    }
}
