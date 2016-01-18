# Usage examples - C#

## Basics

The Phaxio class is the entry point for any Phaxio operation. 

    var phaxio = new Phaxio(key, secret);

### Getting your account status

    var account = phaxio.GetAccountStatus();
    Console.WriteLine(string.Format("Balance: {0}", account.Balance));

## Faxes

### Sending a fax

At the heart of the Phaxio API is the ability to send a fax:

    var pdf = new FileInfo("form1234.pdf");
    var faxId = phaxio.SendFax("8088675309", pdf);

This returns a string id that you can use to reference your fax later. Well, now, wasn't that simple?

You can customize how this sends by passing in a FaxOptions object:

    var options = new FaxOptions { CallerId = "2125552368" };
    var faxId = phaxio.SendFax("8088675309", pdf, options);
    
If you have more than one file, you can pass in a list and Phaxio will concatenate them into one fax:

    var pdf1 = new FileInfo("form1234.pdf");
    var pdf2 = new FileInfo("form4321.pdf");
    var faxId = phaxio.SendFax("8088675309", new List<FileInfo> { pdf1, pdf2 });

If you have a bunch of faxes going to one number, you might want to check out [batching](https://www.phaxio.com/docs/api/send/batching/).
You first specify a batch delay in the FaxOptions. Then, you send as many faxes as you'd like to the number in question, 
and when you're finished and the batch delay is expired, Phaxio will send them all as one long fax. Here's what a
batching FaxOptions would look like:
    
    var options = new FaxOptions { IsBatch = true, BatchDelaySeconds = 30 };
    var fax1Id = phaxio.SendFax("8088675309", pdf1);
    var fax2Id = phaxio.SendFax("8088675309", pdf2);

The machine at 808-867-5309 will see pdf1 and pdf2 as one long fax.
    
### Downloading a fax

To retrieve a fax after you've sent it, call DownloadFax with its id:

    var file = phaxio.DownloadFax("1234");
    
File is a byte array representing your fax in PDF form that you can write to disk or store in a database.
You can also specify which format you'd like:

    var file = phaxio.DownloadFax("1234", fileType:"s");
    
Specify "s" for a small JPEG, "l" for a large JPEG, or "p" for PDF. If you don't specify this, it will be a PDF.

### Resending a fax

You can resend a fax by id:

    Result result = phaxio.ResendFax(123);
    if (result.Success)
    {
        Console.WriteLine("Yes!");
    }
    else
    {
        Console.WriteLine("Error: " + result.Message);
    }

It returns a Result object saying whether the operation was successful or not. If not, the Result object has a
field called Message with the error message:

### Cancelling a fax

You can cancel a fax by id:

    var result = phaxio.CancelFax(123);

It returns a Result object saying whether the operation was successful or not. If the fax has already sent, you cannot cancel
it and this operation will return a failure.

### Deleting a fax

You can delete a fax by id:

    var result = phaxio.DeleteFax(123);

It returns a Result object saying whether the operation was successful or not.
You can also specify whether to only delete the files (default is false):

    var result = phaxio.DeleteFax(123, true);

## Numbers

### Getting area codes 

If you want to know what area codes are available for purchase, you can call this method:

    Dictionary<string, CityState> areaCodes = phaxio.ListAreaCodes();
    
This returns a Dictionary with the area codes as keys, and a CityState object that has the city and state of
the area code. You can also optionally request tollfree numbers:

    Dictionary<string, CityState> areaCodes = phaxio.ListAreaCodes(tollFree:true);

You can specifiy the state:

    Dictionary<string, CityState> areaCodes = phaxio.ListAreaCodes(state:"MA");

Or both:

    Dictionary<string, CityState> areaCodes = phaxio.ListAreaCodes(tollFree:true, state:"MA");

### Provisioning a number

You can ask Phaxio to get you a new number (you must specify an area code:

    var newNumber = phaxio.ProvisionNumber("808");

The call returns a PhoneNumber object representing your new number.

You can also specify a callback URL that will be called when a fax is recieved that overrides the default callback URL.

    var newNumber = phaxio.ProvisionNumber("808", "https://example.com/callback");
    
### Listing your numbers

To get a list of your numbers, you can run this method:

    var numbers = phaxio.ListNumbers();

which will return a List with all of your numbers.

You can specify an area code to search in:

    var numbers = phaxio.ListNumbers("808");

or you can search for a specific number:
    
    var numbers = phaxio.ListNumbers(number: "8088675309");
    
### Release number

You can a number:

    Result success = phaxio.ReleaseNumber("8088675309");

It returns a Result saying whether the operation was successful or not.
    
## PhaxCodes

## Creating a PhaxCode

Creating a PhaxCode is simple:

    var code = phaxio.CreatePhaxCode();
    
Code is a Url object where you can download the barcode.

You can also attach metadata to the code so you can reference it later:

    var code = phaxio.CreatePhaxCode("code-for-form1234");

You can also get the image directly:

    var code = phaxio.DownloadPhaxCodePng();
    
    File.WriteAllBytes(@"C:\temp\phaxCode.png", code);
    
This returns a byte array representing the barcode. You can attach metadata to the code, same as above:

    var code = phaxio.DownloadPhaxCodePng("{'key':'value'}");

## Attaching a PhaxCode to a PDF

If you have a PDF you'd like to attach a PhaxCode to, it's easy:

    var pdf = new FileInfo("form1234.pdf");
    var pdfBytes = phaxio.AttachPhaxCodeToPdf(10, 10, pdf);
    
The first two parameters are the x,y coordinates in PDF points. This will use the default PhaxCode for your account.
You can also specify a PhaxCode to use by passing in its metadata:

    var pdfBytes = phaxio.AttachPhaxCodeToPdf(10, 10, pdf, metadata:"code-for-form1234");

If you'd like it to attach it on a specific page:
    
    var pdfBytes = phaxio.AttachPhaxCodeToPdf(10, 10, pdf, pageNumber:3);

## Misc

### Getting supported countries

If you want to know what countries are supported by Phaxio, you can call this method:

    Dictionary<string, Pricing> supportedCountries = phaxio.ListSupportedCountries();
    
This returns a Dictionary with the country names as keys, and a Pricing object that has the price per page.
 
### Testing callbacks (web hooks)

So you've written a callback or a webhook you'd like tested. It's simple to have Phaxio send you a fax:

    var testFax = new FileInfo("test-fax.pdf");
    var success = phaxio.TestRecieveCallback(testFax);
    
This returns a bool indicating success. This will call your default account callback. If you've specified a callback
for an individual number and you'd like to test that callback, you can specify it with the toNumber parameter:

    var success = phaxio.TestRecieveCallback(testFax, toNumber:"8088675309");

You can also fake who the fax is from:

    var success = phaxio.TestRecieveCallback(testFax, fromNumber:"2125552368");

### Download a hosted document (deprecated)

Although it's deprecated, you can still get a hosted document:

    var file = phaxio.GetHostedDocument("form1234.pdf");
    
File is a byte array representing your document in PDF form that you can write to disk or store in a database.
You can also specify the metadata of the PhaxCode you'd like:

    var file = phaxio.GetHostedDocument("1234", metadata:"key");

&copy; 2016 Noel Herrick