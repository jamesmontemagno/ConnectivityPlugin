## Ping a Host

On occassion you may want to check not only if the device has internet, but also if an end point is available as well.


#### Ping Remote Server
The most common use case is to ping a remote server or remote IP address to see if it up and running.

```csharp
/// <summary>
/// Tests if a remote host name is reachable (no http:// or www.)
/// </summary>
/// <param name="host">Host name can be a remote IP or URL of website</param>
/// <param name="port">Port to attempt to check is reachable.</param>
/// <param name="msTimeout">Timeout in milliseconds.</param>
/// <returns></returns>
Task<bool> IsRemoteReachable(string host, int port = 80, int msTimeout = 5000);
```

Example:
```csharp
public async Task<bool> IsBlogReachableAndRunning()
{
    var connectivity = CrossConnectivity.Current;
    if(!connectivity.IsConnected)
      return false;

    var reachable = await connectivity.IsRemoteReachable("motzcod.es");

    return reachable;
}

```

## Ping Internal Host
On rarer occassions a local machine or local IP on the network may need to be pinged.

```csharp
/// <summary>
/// Tests if a host name is pingable
/// </summary>
/// <param name="host">The host name can either be a machine name, such as "java.sun.com", or a textual representation of its IP address (127.0.0.1)</param>
/// <param name="msTimeout">Timeout in milliseconds</param>
/// <returns></returns>
Task<bool> IsReachable(string host, int msTimeout = 5000);
```

<= Back to [Table of Contents](README.md)