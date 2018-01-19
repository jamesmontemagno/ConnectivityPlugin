## Connectivity Plugin for Xamarin and Windows

Simple cross platform plugin to check connection status of mobile device, gather connection type, bandwidths, and more.

## Documentation
Get started by reading through the [Connectivity Plugin documentation](https://jamesmontemagno.github.io/ConnectivityPlugin/).

## NuGet
* Available on NuGet: [Xam.Plugin.Connectivity](http://www.nuget.org/packages/Xam.Plugin.Connectivity) [![NuGet](https://img.shields.io/nuget/v/Xam.Plugin.Connectivity.svg?label=NuGet)](https://www.nuget.org/packages/Xam.Plugin.Connectivity/)

## Build: 
* [![Build status](https://ci.appveyor.com/api/projects/status/k6l4x6ovp5ysfbar?svg=true)](https://ci.appveyor.com/project/JamesMontemagno/connectivityplugin)
* CI NuGet Feed: https://ci.appveyor.com/nuget/connectivityplugin

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

### Created By: [@JamesMontemagno](http://twitter.com/jamesmontemagno)
* Twitter: [@JamesMontemagno](http://twitter.com/jamesmontemagno)
* Blog: [MotzCod.es](http://motzcod.es), [Micro Blog](http://motz.micro.blog)
* Podcasts: [Merge Conflict](http://mergeconflict.fm), [Coffeehouse Blunders](http://blunders.fm), [The Xamarin Podcast](http://xamarinpodcast.com)
* Video: [The Xamarin Show on Channel 9](http://xamarinshow.com), [YouTube Channel](https://www.youtube.com/jamesmontemagno) 
# Contribution

Thanks you for your interest in contributing to Settings plugin! In this section we'll outline what you need to know about contributing and how to get started.

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
