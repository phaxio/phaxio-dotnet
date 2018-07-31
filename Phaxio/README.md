# Developing this client

## Development notes

You need Visual Studio 2017 (Windows) for the best experience, but you can also run this on Mono/.NET Core on macOS and Linux.

You need the NUnit 3 Runner VS extension to run the tests.

ThinRestClient could be replaced by RestSharp should it get .NET Standard support.

## Requirements 

Currently, this app targets two APIs: .NET Framework 4.5 and .NET Standard 2.0, and it is tested on two different frameworks, .NET 4.5 and .NET Core 2. This means you'll need to have two different tools:

- .NET 4.5 Framework/SDK
  - Windows: comes with Visual Studio
  - Linux/macOS: [mono](https://www.mono-project.com/download/stable/) at 5.12
- [.NET Core 2](https://www.microsoft.com/net/download) SDK at 2.1

## Building

To build, you run these commands:

```sh
nuget restore
msbuild phaxio-dotnet.sln
```

## Testing

See the README under Phaxio.Tests.

## Packaging

Run: 

```sh
msbuild /t:pack /p:Configuration=Release
```

This should create a nuget package in Phaxio/bin/Release that you can then upload at nuget.org.