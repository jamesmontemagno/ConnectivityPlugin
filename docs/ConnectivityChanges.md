## Detecting Connectivity Changes
Often you may need to notify your user or respond based on network changes. You can do this by subscribing several different events.

#### Changes in Connectivity
When any network connectivity is gained, changed, or loss you can register for an event to fire:
```csharp
/// <summary>
/// Event handler when connection changes
/// </summary>
event ConnectivityChangedEventHandler ConnectivityChanged; 
```

You will get a ConnectivityChangeEventArgs with the status if you are connected or not:
```csharp
public class ConnectivityChangedEventArgs : EventArgs
{
  public bool IsConnected { get; set; }
}

public delegate void ConnectivityChangedEventHandler(object sender, ConnectivityChangedEventArgs e);
```

```csharp
CrossConnectivity.Current.ConnectivityChanged += async (sender, args) =>
  {
      Debug.WriteLine($"Connectivity changed to {args.IsConnected}");
  };
```


### Changes in Connectivity Type
When any network connectivity type is changed this event is triggered. Often it also is accompanied by a `ConnectivityChanged` event.

```csharp
/// <summary>
/// Event handler when connection type changes
/// </summary>
event ConnectivityTypeChangedEventHandler ConnectivityTypeChanged;
```

When this occurs an event will be triggered with EventArgs that have the most recent information:

```csharp
public class ConnectivityTypeChangedEventArgs : EventArgs
{
    public bool IsConnected { get; set; }
    public IEnumerable<ConnectionType> ConnectionTypes { get; set; }
}
public delegate void ConnectivityTypeChangedEventHandler(object sender, ConnectivityTypeChangedEventArgs e);
```

Example:
```csharp
CrossConnectivity.Current.ConnectivityTypeChanged += async (sender, args) =>
  {
      Debug.WriteLine($"Connectivity changed to {args.IsConnected}");
      foreach(var t in args.ConnectionTypes)
        Debug.WriteLine($"Connection Type {t}");
  };
```

<= Back to [Table of Contents](README.md)