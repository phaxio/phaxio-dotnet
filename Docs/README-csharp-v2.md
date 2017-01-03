# Usage examples - C&#35;

## Basics

The PhaxioClient class is the entry point for any Phaxio operation.

    var phaxio = new Phaxio(key, secret);

### PagedResult

Several methods return a PagedResult object that has a field Data with the list,
as well as Page indicating the current page, Total which indicates how many items are available,
and PerPage which indicates how many items are retrieved per page.
Those methods that return paged results also accept two parameters ```perPage```,
which controls how many items show per page and ```page``` which controls which page to fetch.
    
### Getting your account status

    var account = phaxio.GetAccountStatus();
    Console.WriteLine(string.Format("Balance: {0}", account.Balance));

## Faxes

### Sending a fax

At the heart of the Phaxio API is the ability to send a fax:

    var pdf = new FileInfo("form1234.pdf");
    var faxRequest = new FaxRequest { ToNumber = "8088675309", File = pdf };
    var faxId = phaxio.SendFax(faxRequest);

The faxId can be used to reference your fax later. Well, now, wasn't that simple?

You can customize how this sends by passing in a FaxOptions object:

    var faxRequest = new FaxRequest { ToNumber = "8088675309", File = pdf, CallerId = "2125552368" };
    var faxId = phaxio.SendFax(faxRequest);
    
If you have more than one file, you can pass in a list and Phaxio will concatenate them into one fax:

    var pdf1 = new FileInfo("form1234.pdf");
    var pdf2 = new FileInfo("form4321.pdf");
    var faxRequest = new FaxRequest
    {
        ToNumber = "8088675309",
        Files = new List<FileInfo> { pdf1, pdf2 },
        CallerId = "2125552368"
    };
    phaxio.SendFax(faxRequest);

If you have a bunch of faxes going to one number, you might want to check out [batching](https://www.phaxio.com/docs/api/send/batching/).
You first specify a batch delay in the FaxOptions. Then, you send as many faxes as you'd like to the number in question, 
and when you're finished and the batch delay is expired, Phaxio will send them all as one long fax. Here's what a
batching FaxOptions would look like:
    
    var faxRequest1 = new FaxRequest
    {
        ToNumber = "8088675309",
        File = pdf1,
        BatchDelaySeconds = 30
    };
    var faxRequest2 = new FaxRequest
    {
        ToNumber = "8088675309",
        File = pdf2,
        BatchDelaySeconds = 30
    };
    phaxio.SendFax(faxRequest1);
    phaxio.SendFax(faxRequest2);

The machine at 808-867-5309 will see pdf1 and pdf2 as one long fax.

### Using content URLs

You can also specify a URL to pull the content of your fax from:

    var faxRequest = new FaxRequest
    {
        ToNumber = "8088675309",
        ContentUrl = "http://example.com/invoice/1234"
    };
    phaxio.SendFax(faxRequest);

If you have multiple URLs to specify, you can set ContentUrls to an IEnumerable<string> to pass multiple URLs

    var faxRequest = new FaxRequest
    {
        ToNumber = "8088675309",
        ContentUrls = new List<string>
        {
            "http://example.com/invoice/1234",
            "http://example.com/invoice/5678"
        }
    };
    phaxio.SendFax(faxRequest);

### Querying sent faxes

To see your sent faxes after you've sent it, call ListFaxes:

    var faxes = phaxio.ListFaxes("1234");
    
This returns a PagedResult of FaxInfo objects. You can also add filters:

    var faxes = phaxio.ListFaxes("1234", createdBefore: DateTime.Now);

### Downloading a fax

To retrieve a fax after you've sent it, call DownloadFax:

    var file = phaxio.DownloadFax("1234");
    
File is a byte array representing your fax in PDF form that you can write to disk or store in a database.
You can also specify which format you'd like:

    var file = phaxio.DownloadFax("1234", "s");
    
Specify "s" for a small JPEG, "l" for a large JPEG, or "p" for PDF. If you don't specify this, it will be a PDF.

### Resending a fax

You can resend a fax:

    Result result = phaxio.ResendFax("1234");
    if (result.Success)
    {
        Console.WriteLine("Yes!");
    }
    else
    {
        Console.WriteLine("Error: " + result.Message);
    }

It returns a Result object saying whether the operation was successful or not. If not, the Result object has a
field called Message with the error message.

### Cancelling a fax

You can cancel a fax:

    var result = phaxio.CancelFax("1234");

It returns a Result object saying whether the operation was successful or not. If the fax has already sent, you cannot cancel
it and this operation will return a failure.

### Deleting a fax's files

You can delete a fax:

    var result = phaxio.DeleteFaxFiles("1234");

It returns a Result object saying whether the operation was successful or not.

### Deleting a fax

You can delete a fax:

    var result = phaxio.DeleteFax("1234");

It returns a Result object saying whether the operation was successful or not.

## Numbers

### Getting area codes 

If you want to know what area codes are available for purchase, you can call this method:

    PagedResult<AreaCode> areaCodes = phaxio.ListAreaCodes();
    
This returns a PagedResult of AreaCode objects. You can also optionally request tollfree numbers:

    PagedResult<AreaCode> areaCodes = phaxio.ListAreaCodes(tollFree:true);

You can specify the state (and you must specify either the country code or the country if you do):

    PagedResult<AreaCode> areaCodes = phaxio.ListAreaCodes(state:"MA", country:"US");

Or both:

    PagedResult<AreaCode> areaCodes = phaxio.ListAreaCodes(tollFree:true, state:"MA", country:"US");

### Provisioning a number

You can ask Phaxio to get you a new number (you must specify an area code and country code):

    var newNumber = phaxio.ProvisionNumber("808", "1");

The call returns a PhoneNumberV2 object representing your new number.

You can also specify a callback URL that will be called when a fax is recieved
at the new number (this will override the default callback URL).

    var newNumber = phaxio.ProvisionNumber("808", "https://example.com/callback");
    
### Listing your numbers

To get a list of your numbers, you can run this method:

    var numbers = phaxio.ListAccountNumbers();

which will return a List<PhoneNumberV2> with all of your numbers.

You can specify an country code to search in:

    var numbers = phaxio.ListNumbers(countryCode: "1");

You can specify an area code to search in (you must also specify the country code):

    var numbers = phaxio.ListNumbers(areaCode: "808", countryCode: "1");

or you can search for a specific number:
    
    var numbers = phaxio.GetNumberInfo("8088675309");
    
### Release number

You can a release number (give it back to Phaxio):

    Result success = phaxio.ReleaseNumber("8088675309");

It returns a Result saying whether the operation was successful or not.
    
## PhaxCodes

## Creating a PhaxCode

Creating a PhaxCode is simple:

    var codeId = phaxio.GeneratePhaxCode();
    
The variable codeId is a identifier so you can reference it later.

You can also attach metadata to the code so you can reference it later:

    var codeId = phaxio.GeneratePhaxCode("code-for-form1234");

To download the PNG of this newly generated code:

    var codeBytes = phaxio.DownloadPhaxCode(codeId);

To get the properties of the newly generated code:

    var codeBytes = phaxio.GetPhaxCode(codeId);
    
You can also get the image directly:

    var codeBytes = phaxio.GeneratePhaxCodeAndDownload();
    
    File.WriteAllBytes(@"C:\temp\phaxCode.png", code);
    
This returns a byte array representing the barcode. You can attach metadata to the code, same as above:

    var codeBytes = phaxio.GeneratePhaxCodeAndDownload("{'key':'value'}");
    

## Misc

### Getting supported countries

If you want to know what countries are supported by Phaxio, you can call this method:

    PagedResult<Country> supportedCountries = phaxio.ListSupportedCountries();
    
This returns a PagedResult of Country objects that have pricing and the services available.
 
### Testing callbacks (web hooks)

So you've written a callback or a webhook you'd like tested. It's simple to have Phaxio send you a fax:

    var testFax = new FileInfo("test-fax.pdf");
    var success = phaxio.TestRecieveCallback(testFax);
    
This returns a bool indicating success. This will call your default account callback. If you've specified a callback
for an individual number and you'd like to test that callback, you can specify it with the toNumber parameter:

    var success = phaxio.TestRecieveCallback(testFax, toNumber:"8088675309");

You can also fake who the fax is from:

    var success = phaxio.TestRecieveCallback(testFax, fromNumber:"2125552368");

&copy; 2016-2017 Phaxio