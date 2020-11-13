## Connectivity Plugin for Xamarin and Windows

Simple cross platform plugin to check connection status of mobile device, gather connection type, bandwidths, and more.

## Documentation
Get started by reading through the [Connectivity Plugin documentation](https://jamesmontemagno.github.io/ConnectivityPlugin/).

## NuGet
* Available on NuGet: [Xam.Plugin.Connectivity](http://www.nuget.org/packages/Xam.Plugin.Connectivity) [![NuGet](https://img.shields.io/nuget/v/Xam.Plugin.Connectivity.svg?label=NuGet)](https://www.nuget.org/packages/Xam.Plugin.Connectivity/)

## Build: 
* [![Build status](https://ci.appveyor.com/api/projects/status/k6l4x6ovp5ysfbar?svg=true)](https://ci.appveyor.com/project/JamesMontemagno/connectivityplugin)
* CI NuGet Feed: https://ci.appveyor.com/nuget/connectivityplugin

### The Future: [Xamarin.Essentials](https://docs.microsoft.com/xamarin/essentials/index?WT.mc_id=friends-0000-jamont)

I have been working on Plugins for Xamarin for a long time now. Through the years I have always wanted to create a single, optimized, and official package from the Xamarin team at Microsoft that could easily be consumed by any application. The time is now with [Xamarin.Essentials](https://docs.microsoft.com/xamarin/essentials/index?WT.mc_id=friends-0000-jamont), which offers over 50 cross-platform native APIs in a single optimized package. I worked on this new library with an amazing team of developers and I highly highly highly recommend you check it out.

I will continue to work and maintain my Plugins, but I do recommend you checkout Xamarin.Essentials to see if it is a great fit your app as it has been for all of mine!

## Platform Support

|Platform|Version|
| ------------------- | :------------------: |
|Xamarin.iOS|iOS 6+|
|tvOS - Apple TV|All|
|Xamarin.Android|API 10+|
|Windows 10 UWP|10+|
|Xamarin.Mac|All|
|.NET 4.5/WPF|All|
|.NET Core|2.0+|
|Tizen|4.0+|

### Created By: [@JamesMontemagno](http://twitter.com/jamesmontemagno)
* Twitter: [@JamesMontemagno](http://twitter.com/jamesmontemagno)
* Blog: [MotzCod.es](http://motzcod.es), [Micro Blog](http://motz.micro.blog)
* Podcasts: [Merge Conflict](http://mergeconflict.fm), [Coffeehouse Blunders](http://blunders.fm), [The Xamarin Podcast](http://xamarinpodcast.com)
* Video: [The Xamarin Show on Channel 9](http://xamarinshow.com), [YouTube Channel](https://www.youtube.com/jamesmontemagno) 
# Contribution

Thank you for your interest in contributing to the Connectivity plugin! In this section we'll outline what you need to know about contributing and how to get started.

### Bug Fixes
Please browse open issues, if you're looking to fix something, it's possible that someone already reported it. Additionally you select any `up-for-grabs` items

### Pull requests
Please fill out the pull request template when you send one.
Run tests to make sure your changes don't break any unit tests. Follow these instructions to run tests - 

**iOS**
- Navigate to _tests/Connectivity.Tests.iOS_
- Execute `make run-simulator-tests`

**Android**

Execute `./build.sh --target RunDroidTests` from the project root
## License
The MIT License (MIT) see [License file](LICENSE)

### Want To Support This Project?
All I have ever asked is to be active by submitting bugs, features, and sending those pull requests down! Want to go further? Make sure to subscribe to my weekly development podcast [Merge Conflict](http://mergeconflict.fm), where I talk all about awesome Xamarin goodies and you can optionally support the show by becoming a [supporter on Patreon](https://www.patreon.com/mergeconflictfm).
