## 4.0.0
* [All] New Implementations of IsReachable and IsRemoteReachable
* IsReachable: Checks if host is reachable.
* IsRemoteReachable: Now only takes in a valid Uri with or without a port to check connectivity to a service.
* IsRemoteReachable: Must have a valid Uri scheme.

### 3.0.2
* [Android] Bandwidth as bits per second instead of mbps
* [Android] No longer show connection types that aren't available
* [Android] Only return active connection types
* [iOS] Fix for connection type always returning WiFi even when on cellular
* [Windows] Improve timeout when checking connections

### 3.0.1
* WPF/.NET 4.5 Support
* macOS Support
* tvOS Support
* Move to .NET STandard 
* Removal of Windows Phone/Store 8/8.1

### 2.3.0
* Add bluetooth connection type
* Add ConnectivityTypeChanged event
* Fixed #3 (Ensure connecitivity manager is never null)
* Fixed #6 (Android returns all connection types)
* iOS 10 optimizations
