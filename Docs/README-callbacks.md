# Writing callbacks (webhooks)

Phaxio can send you a message via callback (or webhook) every time you send or recieve a fax.
Writing a callback is the way you get notified you have a new fax sent to one of your numbers.
Using Web API, it's simple to write a callback. Checkout the Phaxio.Examples.ReceiveCallback projects for
a full implementation.

## Example code
```C#
public Task<HttpResponseMessage> Post()
{
// Check to make sure we're getting the expected format
// Phaxio will send the callback as a multipart
HttpRequestMessage request = this.Request;
if (!request.Content.IsMimeMultipartContent())
{
    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
}

// We'll need a place to store the fax uploads
// In this case, we'll store it in /App_Data/uploads/
string root = HttpContext.Current.Server.MapPath("~/App_Data/uploads");
var provider = new MultipartFormDataStreamProvider(root);

// Now we're going to request that the provider process
// the request and when it's finished, call back the
// lambda below
var task = request.Content.ReadAsMultipartAsync(provider).
    ContinueWith<HttpResponseMessage>(o =>
    {
	// Right here, the provider has read all the data
	// We get the file that Phaxio sent us
	var file = provider.FileData.First();

	// Here we get a new Fax object from the key/values
	// that Phaxio passed us
	var receipt = getFaxFromFormData(provider.FormData);

	// We're storing the fax in a memory cache
	// but you might want to store this in a DB
	ObjectCache cache = MemoryCache.Default;
	var faxList = cache["Callbacks"] as List<FaxReceipt>;
	faxList.Add(receipt);

	// Respond to Phaxio's servers
	return new HttpResponseMessage()
	{
	    Content = new StringContent("Callback received.")
	};
    }
);

return task;
}```

Let's take a look at the `getFaxFromFormData` method:

```C#
private FaxReceipt getFaxFromFormData(NameValueCollection formData)
{
return new FaxReceipt
{
    Direction = formData["direction"],

    // The "fax" field is a JSON document, so we'll let
    // Json.NET make a dynamic object we can use later
    Fax = JsonConvert.DeserializeObject(formData["fax"]),
    IsTest = formData["is_test"] == "true" ? true : false,
    Success = formData["success"] == "true" ? true : false
};
}
```

Finally, here's the `FaxReceipt` class:

```C#
public class FaxReceipt
{
// Indicates outgoing or incoming (a send or a recieve)
public string Direction { get; set; }

// A JSON document representing a fax
public dynamic Fax { get; set; }

// Whether the callback was a test
public bool IsTest { get; set; }

// Whether the fax was successful or not
public bool Success { get; set; }
}
```
## Testing your callback

To test a send callback, just send a fax. To trigger a recieve callback, use the TestRecieveCallback method:

```C#
var testFax = new FileInfo("test-fax.pdf");
var success = phaxio.TestRecieveCallback(testFax);
```