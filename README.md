# Phaxio (ALPHA) (NO LICENSE)

Phaxio is the only cloud based fax API designed for developers. This is the .NET client library for Phaxio.

## Getting started

First, [sign up](https://www.phaxio.com/signup) if you haven't already.

Then, go to [api settings](https://www.phaxio.com/apiSettings) and get your key and your secret.

If you're a C# developer, [see here](Docs/README-csharp.md) for basic usage.

## Errors

### Rate limiting

The Phaxio API is rate limited. If you make too many requests too quickly, you might receive this error.
Check the exception message, wait a second, and then try your request again.

## Writing callbacks (webhooks)

[Read this](Docs/README-callbacks.md)

&copy; 2016 Noel Herrick