## Ping a Host

On occassion you may want to check not only if the device has internet, but also if an end point is available as well.


#### Ping Remote Server
The most common use case is to check to see if a remote uri is reachable. This could include a web server, api call, or host with a specific port.

```csharp
/// <summary>
/// Tests if a remote uri is reachable
/// </summary>
/// <param name="uri">Full valid Uri to check for reachability</param>
/// <param name="timeout">Timeout</param>
/// <returns></returns>
Task<bool> IsRemoteReachable(string uri, TimeSpan timeout);

/// <summary>
/// Tests if a remote uri is reachable
/// </summary>
/// <param name="uri">Full valid Uri to check for reachability</param>
/// <param name="timeout">Timeout.</param>
/// <returns></returns>
Task<bool> IsRemoteReachable(Uri uri, TimeSpan timeout);
```

Example:
```csharp
public async Task<bool> IsBlogReachableAndRunning()
{
    var connectivity = CrossConnectivity.Current;
    if(!connectivity.IsConnected)
      return false;

    var reachable = await connectivity.IsRemoteReachab("montemagno.com/monkeys.json", TimeSpan.FromSecond(5));

    return reachable;
}

```

## Ping Host
On rarer occassions a local machine, hostname, or local IP on the network may need to be pinged.

```csharp
/// <summary>
/// Tests if a host name is reachable
/// </summary>
/// <param name="host">The host name can either be a machine name, such as "java.sun.com", or a textual representation of its IP address (127.0.0.1)</param>
/// <param name="timeout">Timeout</param>
/// <returns></returns>
Task<bool> IsReachable(string host, TimeSpan timespan);
```

<= Back to [Table of Contents](README.md)