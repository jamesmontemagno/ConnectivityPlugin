Connectivity Readme

Ensure the NuGet is installed in PCL and Platform specific projects.

[Release Notes - 2.3.0]
* New ConnectivityTypeChanged event
* Fix for Ad-hoc wifi, see readme on GitHub
* Additional bug fixes and optimizations

See: https://github.com/jamesmontemagno/ConnectivityPlugin/blob/master/CHANGELOG.md for full list

**IMPORTANT**
Android:
The following persmissions are automatically added for you:
ACCESS_NETWORK_STATE & ACCESS_WIFI_STATE

iOS:
Bandwidths are not supported and will always return an empty list.

Windows 8.1 & Windows Phone 8.1 RT:
RT apps can not perform loopback, so you can not use IsReachable to query the states of a local IP.

Permissions to think about:
The Private Networks (Client & Server) capability is represented by the Capability name = "privateNetworkClientServer" tag in the app manifest. 
The Internet (Client & Server) capability is represented by the Capability name = "internetClientServer" tag in the app manifest.
