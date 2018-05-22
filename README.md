# Phaxio

![AppVeyor build status](https://ci.appveyor.com/api/projects/status/s9gjrnfa42g08mb4?svg=true)

[![Build Status](https://travis-ci.org/phaxio/phaxio-dotnet.svg?branch=master)](https://travis-ci.org/phaxio/phaxio-dotnet)

Phaxio is the only cloud based fax API designed for developers. This is the .NET client library for Phaxio.

## Getting started

First, [sign up](https://console.phaxio.com/signup) if you haven’t already.

Second, go to [api settings](https://console.phaxio.com/api_credentials) and get your key and your secret.

Third, install this client with NuGet:

  Install-Package Phaxio

Use [this guide](Docs/README-csharp.md) if you’re a C# developer or
[this guide](Docs/README-vb.md) if you’re a VB.NET developer.

## Migration from library version 1.0.0 to 2.0.0

This is a complete re-write and starts from scratch in its design. Please see the above documentation about how to use the new library and its calls.

## Migration from API V1 to V2

This library now uses Phaxio API V2, so these methods have been removed and have no equivalent:

- AttachPhaxCodeToPdf was removed
- CreatePhaxCode is now GeneratePhaxCode
- DownloadPhaxCodePng is now DownloadPhaxCode
- GetHostedDocument was removed

## Errors

Operations that connect to Phaxio will throw an exception if an error is encountered.

RateLimitException happens if you have made too many requests per second.
InvalidRequestException is throw if the data sent to Phaxio is not correct
AuthenticationException gets thrown when your credentials are invalid
NotFoundException is throw when you try to retrieve a resource by ID but it isn’t found
ApiConnectionException occurs when there is a network issue
ServerException happens if the server is not working

### Rate limiting

The Phaxio API is rate limited. If you make too many requests too quickly, you might receive this error.
Check the exception message, wait a second, and then try your request again.

## Writing callbacks (webhooks)

Writing a callback to get fax send or receive events is simple. Read this [handy guide](Docs/README-callbacks.md) to get started.

&copy; 2016-2017 Phaxio
Message Input

Message #general

*bold* _italics_ ~strike~ `code` ```preformatted``` >quote
