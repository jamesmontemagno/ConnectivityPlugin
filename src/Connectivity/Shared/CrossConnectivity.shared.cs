using Plugin.Connectivity.Abstractions;
using System;

namespace Plugin.Connectivity
{
    /// <summary>
    /// Cross platform Connectivity implementations
    /// </summary>
    public class CrossConnectivity
    {
        static Lazy<IConnectivity> implementation = new Lazy<IConnectivity>(() => CreateConnectivity(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

		/// <summary>
		/// Gets if the plugin is supported on the current platform.
		/// </summary>
		public static bool IsSupported => implementation.Value == null ? false : true;

		/// <summary>
		/// Current plugin implementation to use
		/// </summary>
		public static IConnectivity Current
        {
            get
            {
                var ret = implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static IConnectivity CreateConnectivity()
        {
#if NETSTANDARD1_0 || NETSTANDARD2_0
            return null;
#else
			return new ConnectivityImplementation();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
			new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        


        /// <summary>
        /// Dispose of everything 
        /// </summary>
        public static void Dispose()
        {
            if (implementation?.IsValueCreated ?? false)
            {
				implementation.Value.Dispose();

				implementation = new Lazy<IConnectivity>(() => CreateConnectivity(), System.Threading.LazyThreadSafetyMode.PublicationOnly);
            }
        }
    }
}
