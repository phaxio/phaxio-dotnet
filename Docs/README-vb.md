# Usage examples - Visual Basic .NET

## Basics

The PhaxioClient class is the entry point for any Phaxio operation (be sure to use `Imports Phaxio`)

    Dim phaxio As PhaxioClient = New PhaxioClient("secret", "key")

### Getting your account status

    Dim account As Account = phaxio.GetAccountStatus()
    Console.WriteLine(account.Balance)

## Faxes

### Sending a fax

At the heart of the Phaxio API is the ability to send a fax:

    Dim pdf = New FileInfo("form1234.pdf")
    Dim fax = phaxio.CreateFax()
    fax.Send("8088675309", pdf)

The Fax object now has its Id field set that you can use to reference your fax later. Well, now, wasn't that simple?

You can customize how this sends by passing in a FaxOptions object:

    Dim options = New FaxOptions With {.CallerId = "2125552368"}
    fax.Send("8088675309", pdf, options)
    
If you have more than one file, you can pass in a list and Phaxio will concatenate them into one fax:

    Dim pdf1 = New FileInfo("form1234.pdf")
    Dim pdf2 = New FileInfo("form4321.pdf")
    fax.Send("8088675309", New List(Of FileInfo) From {pdf1, pdf2})

If you have a bunch of faxes going to one number, you might want to check out [batching](https://www.phaxio.com/docs/api/send/batching/).
You first specify a batch delay in the FaxOptions. Then, you send as many faxes as you'd like to the number in question, 
and when you're finished and the batch delay is expired, Phaxio will send them all as one long fax. Here's what a
batching FaxOptions would look like:
    
    Dim options = New FaxOptions With { .IsBatch = true, .BatchDelaySeconds = 30 }
    fax1.Send("8088675309", pdf1, options)
    fax2.Send("8088675309", pdf2, options)

The machine at 808-867-5309 will see pdf1 and pdf2 as one long fax.

### Get a fax later on

If you need to operate on a fax after you've sent it and you no longer have the original Fax object,
you can create a new Fax object by its id, and then you can resend it, delete it, etc.

    Dim fax = phaxio.CreateFax("1234")
    
This does not work:

    Dim fax = phaxio.CreateFax()
    fax.Download();
    
### Downloading a fax

To retrieve a fax after you've sent it, call DownloadFax:

    Dim file = fax.Download()
    
File is a byte array representing your fax in PDF form that you can write to disk or store in a database.
You can also specify which format you'd like:

    Dim file = fax.Download(fileType:="s")
    
Specify "s" for a small JPEG, "l" for a large JPEG, or "p" for PDF. If you don't specify this, it will be a PDF.

### Resending a fax

You can resend a fax:

    Dim result as Result = fax.Resend()
    If Not result.Success Then
        Console.WriteLine("Error: " + result.Message)
    End If

It returns a Result object saying whether the operation was successful or not. If not, the Result object has a
field called Message with the error message.

### Cancelling a fax

You can cancel a fax:

    Dim result = fax.Cancel("123")

It returns a Result object saying whether the operation was successful or not. If the fax has already sent, you cannot cancel
it and this operation will return a failure.

### Deleting a fax

You can delete a fax:

    Dim result = phaxio.Delete()

It returns a Result object saying whether the operation was successful or not.
You can also specify whether to only delete the files (default is false):

    Dim result = phaxio.Delete(true)

## Numbers

### Getting area codes 

If you want to know what area codes are available for purchase, you can call this method:

    Dim areaCodes As Dictionary(Of String, CityState) = phaxio.ListAreaCodes()
    
This returns a Dictionary with the area codes as keys, and a CityState object that has the city and state of
the area code. You can also optionally request tollfree numbers:

    Dim areaCodes As Dictionary(Of String, CityState) = phaxio.ListAreaCodes(tollFree:=true)

You can specifiy the state:

    Dim areaCodes As Dictionary(Of String, CityState) = phaxio.ListAreaCodes(state:="MA")

Or both:

    Dim areaCodes As Dictionary(Of String, CityState) = phaxio.ListAreaCodes(tollFree:=true, state:="MA")

### Provisioning a number

You can ask Phaxio to get you a new number (you must specify an area code):

    Dim newNumber = phaxio.ProvisionNumber("808")

The call returns a PhoneNumber object representing your new number.

You can also specify a callback URL that will be called when a fax is recieved
at the new number (this will override the default callback URL).

    Dim newNumber = phaxio.ProvisionNumber("808", "https://example.com/callback")
    
### Listing your numbers

To get a list of your numbers, you can run this method:

    Dim numbers = phaxio.ListNumbers()

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

    Dim code = phaxio.CreatePhaxCode()
    
Code is a Uri object where you can download the barcode.

You can also attach metadata to the code so you can reference it later:

    Dim code = phaxio.CreatePhaxCode("code-for-form1234")

You can also get the image directly:

    Dim code = phaxio.DownloadPhaxCodePng()
    
    My.Computer.FileSystem.WriteAllBytes("C:\temp\phaxCode.png", code, False)
    
This returns a byte array representing the barcode. You can attach metadata to the code, same as above:

    Dim code = phaxio.DownloadPhaxCodePng("{'key':'value'}")

## Attaching a PhaxCode to a PDF

If you have a PDF you'd like to attach a PhaxCode to, it's easy:

    Dim pdf = New FileInfo("form1234.pdf")
    Dim pdfBytes = phaxio.AttachPhaxCodeToPdf(10, 10, pdf)
    
The first two parameters are the x,y coordinates in PDF points. This will use the default PhaxCode for your account.
You can also specify a PhaxCode to use by passing in its metadata:

    Dim pdfBytes = phaxio.AttachPhaxCodeToPdf(10, 10, pdf, metadata:="code-for-form1234")

If you'd like it to attach it on a specific page:
    
    Dim pdfBytes = phaxio.AttachPhaxCodeToPdf(10, 10, pdf, pageNumber:=3)

## Misc

### Getting supported countries

If you want to know what countries are supported by Phaxio, you can call this method:

    Dim supportedCountries As Dictionary(Of string, Pricing) = phaxio.ListSupportedCountries()
    
This returns a Dictionary with the country names as keys, and a Pricing object that has the price per page.
 
### Testing callbacks (web hooks)

So you've written a callback or a webhook you'd like tested. It's simple to have Phaxio send you a fax:

    Dim testFax = New FileInfo("test-fax.pdf")
    Dim result = phaxio.TestRecieveCallback(testFax)
    
This returns a Result indicating success. This will call your default account callback. If you've specified a callback
for an individual number and you'd like to test that callback, you can specify it with the toNumber parameter:

    Dim success = phaxio.TestRecieveCallback(testFax, toNumber:="8088675309")

You can also fake who the fax is from:

    Dim result = phaxio.TestRecieveCallback(testFax, fromNumber:="2125552368")

### Download a hosted document (deprecated)

Although it's deprecated, you can still get a hosted document:

    Dim file = phaxio.GetHostedDocument("form1234.pdf")
    
File is a byte array representing your document in PDF form that you can write to disk or store in a database.
You can also specify the metadata of the PhaxCode you'd like:

    Dim file = phaxio.GetHostedDocument("1234", metadata:="key")

&copy; 2016 Noel Herrick