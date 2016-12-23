# Usage examples - Visual Basic .NET

## Basics

The PhaxioClient class is the entry point for any Phaxio operation (be sure to use `Imports Phaxio.V2`)

    Dim phaxio As PhaxioV2Client = New PhaxioClient("secret", "key")

### PagedResult

Several methods return a PagedResult object that has a field Data with the list,
as well as Page indicating the current page, Total which indicates how many items are available,
and PerPage which indicates how many items are retrieved per page.
Those methods that return paged results also accept two parameters ```perPage```,
which controls how many items show per page and ```page``` which controls which page to fetch.
    
### Getting your account status

    Dim account As Account = phaxio.GetAccountStatus()
    Console.WriteLine(account.Balance)

## Faxes

### Sending a fax

At the heart of the Phaxio API is the ability to send a fax:

    Dim pdf = New FileInfo("form1234.pdf")
    Dim faxRequest = New FaxRequest With { .ToNumber = "8088675309", .File = pdf };
    var faxId = phaxio.SendFax(faxRequest);

The faxId can be used to reference your fax later. Well, now, wasn't that simple?
You can customize how this sends by passing in a FaxOptions object:

    Dim faxRequest = New FaxRequest With { .ToNumber = "8088675309", .File = pdf, .CallerId = "2125552368" };
    phaxio.SendFax(faxRequest)
    
If you have more than one file, you can pass in a list and Phaxio will concatenate them into one fax:

    Dim pdf1 = New FileInfo("form1234.pdf")
    Dim pdf2 = New FileInfo("form4321.pdf")
    Dim faxRequest = New FaxRequest With
    {
        .ToNumber = "8088675309",
        .Files = New List(Of FileInfo) From {pdf1, pdf2}
    };
    phaxio.SendFax(faxRequest)

If you have a bunch of faxes going to one number, you might want to check out [batching](https://www.phaxio.com/docs/api/send/batching/).
You first specify a batch delay in the FaxOptions. Then, you send as many faxes as you'd like to the number in question, 
and when you're finished and the batch delay is expired, Phaxio will send them all as one long fax. Here's what a
batching FaxOptions would look like:
    
    Dim faxRequest1 = New FaxRequest With
    {
        .ToNumber = "8088675309",
        .File = pdf1
        .BatchDelaySeconds = 30
    };
    
    Dim faxRequest2 = New FaxRequest With
    {
        .ToNumber = "8088675309",
        .File = pdf2
        .BatchDelaySeconds = 30
    };
    fax1.SendFax(faxRequest1)
    fax2.SendFax(faxRequest2)

The machine at 808-867-5309 will see pdf1 and pdf2 as one long fax.

### Querying sent faxes

To see your sent faxes after you've sent it, call ListFaxes:

    Dim faxes = phaxio.ListFaxes("1234");
    
This returns a PagedResult of FaxInfo objects. You can also add filters:

    Dim faxes = phaxio.ListFaxes("1234", createdBefore:=DateTime.Now);

### Downloading a fax

To retrieve a fax after you've sent it, call DownloadFax:

    Dim file = phaxio.DownloadFax("1234")
    
File is a byte array representing your fax in PDF form that you can write to disk or store in a database.
You can also specify which format you'd like:

    Dim file = phaxio.DownloadFax("1234", thumbnail:="s")
    
Specify "s" for a small JPEG, "l" for a large JPEG, or "p" for PDF. If you don't specify this, it will be a PDF.

### Resending a fax

You can resend a fax:

    Dim result as Result = phaxio.ResendFax("1234")
    If Not result.Success Then
        Console.WriteLine("Error: " + result.Message)
    End If

It returns a Result object saying whether the operation was successful or not. If not, the Result object has a
field called Message with the error message.

### Cancelling a fax

You can cancel a fax:

    Dim result = phaxio.CancelFax("1234")

It returns a Result object saying whether the operation was successful or not. If the fax has already sent, you cannot cancel
it and this operation will return a failure.

### Deleting a fax's files

You can delete a fax:

    Dim result = phaxio.DeleteFaxFiles("1234")

It returns a Result object saying whether the operation was successful or not.

### Deleting a fax

You can delete a fax:

    Dim result = phaxio.DeleteFax("1234")

It returns a Result object saying whether the operation was successful or not.

## Numbers

### Getting area codes 

If you want to know what area codes are available for purchase, you can call this method:

    Dim areaCodes As PagedResult(Of AreaCode) = phaxio.ListAreaCodes()
    
This returns a PagedResult of AreaCode objects. You can also optionally request tollfree numbers:

    Dim areaCodes As Dictionary(Of AreaCode) = phaxio.ListAreaCodes(tollFree:=true)

You can specify the state (and you must specify either the country code or the country if you do):

    Dim areaCodes As Dictionary(Of AreaCode) = phaxio.ListAreaCodes(state:="MA", country:="US")

Or both:

    Dim areaCodes As Dictionary(Of AreaCode) = phaxio.ListAreaCodes(tollFree:=true, state:="MA", country:="US")

### Provisioning a number

You can ask Phaxio to get you a new number (you must specify an area and country code):

    Dim newNumber = phaxio.ProvisionNumber("808", "1")

The call returns a PhoneNumberV2 object representing your new number.

You can also specify a callback URL that will be called when a fax is received
at the new number (this will override the default callback URL).

    Dim newNumber = phaxio.ProvisionNumber("808", "1", "https://example.com/callback")
    
### Listing your numbers

To get a list of your numbers, you can run this method:

    Dim numbers = phaxio.ListAccountNumbers()

which will return a `List(Of PhoneNumber)` with all of your numbers.

You can specify an area code to search in:

    Dim numbers = phaxio.ListNumbers("808")

or you can search for a specific number:
    
    Dim numbers = phaxio.ListNumbers(number:="8088675309")
    
### Release number

You can a release number (give it back to Phaxio):

    Dim success As Result = phaxio.ReleaseNumber("8088675309")

It returns a Result saying whether the operation was successful or not.
    
## PhaxCodes

## Creating a PhaxCode

Creating a PhaxCode is simple:

    Dim codeId = phaxio.GeneratePhaxCode()
    
The variable codeId is a identifier so you can reference it later.

You can also attach metadata to the code so you can reference it later:

    Dim codeId = phaxio.GeneratePhaxCode("code-for-form1234")

To download the PNG of this newly generated code:

    Dim codeBytes = phaxio.DownloadPhaxCode(codeId)

To get the properties of the newly generated code:

    Dim codeBytes = phaxio.GetPhaxCode(codeId)
    
You can also get the image directly:

    Dim codeBytes = phaxio.GeneratePhaxCodeAndDownload();
    
    My.Computer.FileSystem.WriteAllBytes(@"C:\temp\phaxCode.png", code)
    
This returns a byte array representing the barcode. You can attach metadata to the code, same as above:

    Dim codeBytes = phaxio.GeneratePhaxCodeAndDownload("{'key':'value'}")

## Misc

### Getting supported countries

If you want to know what countries are supported by Phaxio, you can call this method:

    Dim supportedCountries As List(Of Country) = phaxio.ListSupportedCountries()
    
This returns a List of Country objects that have pricing and the services available.
 
### Testing callbacks (web hooks)

So you've written a callback or a webhook you'd like tested. It's simple to have Phaxio send you a fax:

    Dim testFax = New FileInfo("test-fax.pdf")
    Dim result = phaxio.TestRecieveCallback(testFax)
    
This returns a Result indicating success. This will call your default account callback. If you've specified a callback
for an individual number and you'd like to test that callback, you can specify it with the toNumber parameter:

    Dim success = phaxio.TestRecieveCallback(testFax, toNumber:="8088675309")

You can also fake who the fax is from:

    Dim result = phaxio.TestRecieveCallback(testFax, fromNumber:="2125552368")

&copy; 2016-2017 Phaxio