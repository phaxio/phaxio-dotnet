# Phaxio (ALPHA) (NO LICENSE)

Phaxio is the only cloud based fax API designed for developers. This is the .NET client library for Phaxio.

## Getting started

First, [sign up](https://www.phaxio.com/signup) if you haven't already.

Then, go to [api settings](https://www.phaxio.com/apiSettings) and get your key and your secret.

The Phaxio class is the entry point for any Phaxio operation. 

    var phaxio = new Phaxio(key, secret);

## Getting your account status

    var account = phaxio.GetAccountStatus();
    Console.WriteLine(string.Format("Balance: {0}", account.Balance));

## Getting area codes 

If you want to know what area codes are available for purchase, you can call this method:

    Dictionary<string, CityState> areaCodes = phaxio.GetAreaCodes();
    
This returns a Dictionary with the area codes as keys, and a CityState object that has the city and state of
the area code. You can also optionally request tollfree numbers:

    Dictionary<string, CityState> areaCodes = phaxio.GetAreaCodes(tollFree:true);

You can specifiy the state:

    Dictionary<string, CityState> areaCodes = phaxio.GetAreaCodes(state:"MA");

Or both:

    Dictionary<string, CityState> areaCodes = phaxio.GetAreaCodes(tollFree:true, state:"MA");
    

&copy; 2016 Noel Herrick