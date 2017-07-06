## Checking Connectivity
There are a few properties that can be used to easily check connection information using the plugin.

### Check Connectivity
`IsConnected`: The easiest and most common use case of simply chcking if there is interent access:

```csharp
/// <summary>
/// Gets if there is an active internet connection
/// </summary>
bool IsConnected { get; }
```

Example:
```csharp
public async Task<string> MakeWebRequest()
{
    if(!CrossConnectivity.Current.IsConnected)
    {
      //You are offline, notify the user
      return null;
    }

    //Make web request here
}
```

### Check Type of Connection
Easily check what type of internet connection is currently active.

```csharp
/// <summary>
/// Gets the list of all active connection types.
/// </summary>
IEnumerable<ConnectionType> ConnectionTypes { get; }
```

Example:
```csharp
public async Task<string> MakeWebRequestWifiOnly()
{
    var wifi = Plugin.Connectivity.Abstractions.ConnectionType.WiFi;
    var connectionTypes = CrossConnectivity.Current.ConnectionTypes;
    if(!connectionTypes.Contains(wifi))
    {
      //You do not have wifi
      return null;
    }

    //Make web request here
}
```

### Speed of Connection

You can query all bandwidths of the active connections in Bits Per Second.

```csharp
/// <summary>
/// Retrieves a list of available bandwidths for the platform.
/// Only active connections.
/// </summary>
IEnumerable<UInt64> Bandwidths { get; }
```

Example:
```csharp
public async Task<string> MakeWebRequestOneMeg()
{
    var optimalSpeed = 1000000; //1Mbps
    var speeds = CrossConnectivity.Current.Bandwidths;

    //If on iOS or none were returned
    if(speeds.Length == 0)
      return null;

    if(!connectionTypes.Any(speed => speed > optimalSpeed))
    {
      //You do not have wifi
      return null;
    }

    //Make web request here
}
```

**Platform Tweaks**:
* Apple Platforms: Bandwidths are not supported and will always return an empty list.
* Android: In releases earlier than 3.0.2 this was returned as Mbps.
* Android: Only returns bandwidth of WiFi connections. For all others you can check the 

<= Back to [Table of Contents](README.md)

