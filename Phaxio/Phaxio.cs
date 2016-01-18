using Newtonsoft.Json;
using Phaxio.Entities;
using Phaxio.Entities.Internal;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio
{
    public class Phaxio
    {
        private const string phaxioApiEndpoint = "https://api.phaxio.com/v1/";
        private readonly string key;
        private readonly string secret;
        private IRestClient client;

        public Phaxio (string key, string secret)
            : this(key, secret, new RestClient())
        {

        }

        public Phaxio (string key, string secret, IRestClient restClient)
        {
            this.key = key;
            this.secret = secret;

            // Initialize the rest client
            client = restClient;
            client.BaseUrl = new Uri(phaxioApiEndpoint);
        }

        /// <summary>
        ///  Gets the account for this Phaxio instance.
        /// </summary>
        /// <returns>An Account object</returns>
        public Account GetAccountStatus ()
        {
            return performRequest<Account>("accountStatus", Method.GET).Data;
        }

        /// <summary>
        ///  Returns a dictionary of area codes available for purchasing Phaxio numbers.
        ///  The keys are areacodes, and the values are their city and state.
        /// </summary>
        /// <param name="tollFree">Whether the number should be tollfree.</param>
        /// <param name="state">A two character state or province abbreviation (e.g. IL or YT).
        /// Will only return area codes available for this state.</param>
        /// <returns>A Dictionary&lt;string, CityState&gt; with area codes for keys and CityStates for values</returns>
        public Dictionary<string, CityState> GetAreaCodes (bool? tollFree = null, string state = null)
        {
            Action<IRestRequest> addParameters = req =>
                {
                    if (tollFree != null)
                    {
                        req.AddParameter("is_toll_free", tollFree);
                    }

                    if (state != null)
                    {
                        req.AddParameter("state", state);
                    }
                };

            return performRequest<Dictionary<string, CityState>>("areaCodes", Method.POST, false, addParameters).Data;
        }

        /// <summary>
        ///  Returns a dictionary of supported countries by Phaxio along with pricing information
        /// </summary>
        /// <returns>A Dictionary&lt;string, Pricing&gt; with countries for keys and Pricing for values</returns>
        public Dictionary<string, Pricing> GetSupportedCountries()
        {
            return performRequest<Dictionary<string, Pricing>>("supportedCountries", Method.POST, false, r => { }).Data;
        }

        /// <summary>
        ///  Sends a request to Phaxio to test a callback (web hook).
        /// </summary>
        /// <param name="file">The file to send to the callback.</param>
        /// <param name="fromNumber">The phone number of the simulated sender.</param>
        /// <param name="toNumber">The phone number that is receiving the fax.</param>
        /// <returns>A Result object indicating whether the operation was successful.</returns>
        public Result TestRecieveCallback(FileInfo file, string fromNumber = null, string toNumber = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                byte[] fileBytes = File.ReadAllBytes(file.DirectoryName + Path.DirectorySeparatorChar + file.Name);

                req.AddFile("filename", fileBytes, file.Name, "application/octet");

                if (fromNumber != null)
                {
                    req.AddParameter("from_number", fromNumber);
                }

                if (toNumber != null)
                {
                    req.AddParameter("to_number", toNumber);
                }
            };

            return performRequest<Object>("testReceive", Method.POST, true, addParameters).ToResult();
        }

        /// <summary>
        ///  Cancels a fax
        /// </summary>
        /// <param name="faxId">The id of the fax to cancel.</param>
        /// <returns>A Result object indicating whether the operation was successful.</returns>
        public Result CancelFax(string faxId)
        {
            Action<IRestRequest> addParameters = req =>
            {
                req.AddParameter("id", faxId);
            };

            return performRequest<Object>("faxCancel", Method.GET, true, addParameters).ToResult();
        }

        /// <summary>
        ///  Resends a fax
        /// </summary>
        /// <param name="faxId">The id of the fax to resend.</param>
        /// <returns>A Result object indicating whether the operation was successful.</returns>
        public Result ResendFax(string faxId)
        {
            Action<IRestRequest> addParameters = req =>
            {
                req.AddParameter("id", faxId);
            };

            return performRequest<Object>("resendFax", Method.GET, true, addParameters).ToResult();
        }

        /// <summary>
        ///  Deletes a fax
        /// </summary>
        /// <param name="faxId">The id of the fax to delete.</param>
        /// <param name="filesOnly">A boolean indicating whether to only delete the files.</param>
        /// <returns>A Result object indicating whether the operation was successful.</returns>
        public Result DeleteFax(string faxId, bool filesOnly = false)
        {
            Action<IRestRequest> addParameters = req =>
            {
                req.AddParameter("id", faxId);
                req.AddParameter("files_only", filesOnly);
            };

            return performRequest<Object>("deleteFax", Method.GET, true, addParameters).ToResult();
        }

        /// <summary>
        ///  Provisions a new fax number
        /// </summary>
        /// <param name="areaCode">The area code to provsion the number in.</param>
        /// <param name="callbackUrl">The URL that Phaxio will post to when a fax is recieved at this number.</param>
        /// <returns>A PhoneNumber object representing the new number.</returns>
        public PhoneNumber ProvisionNumber(string areaCode, string callbackUrl = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                req.AddParameter("area_code", areaCode);

                if (callbackUrl != null)
                {
                    req.AddParameter("callback_url", callbackUrl);
                }
            };

            return performRequest<PhoneNumber>("provisionNumber", Method.GET, true, addParameters).Data;
        }
        
        /// <summary>
        ///  Lists all of your numbers
        /// </summary>
        /// <param name="areaCode">The area code to filter by.</param>
        /// <param name="number">The number to search for.</param>
        /// <returns>A List of PhoneNumber objects.</returns>
        public List<PhoneNumber> ListNumbers (string areaCode = null, string number = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                if (areaCode != null)
                {
                    req.AddParameter("area_code", areaCode);
                }
                
                if (number != null)
                {
                    req.AddParameter("number", number);
                }
            };

            return performRequest<List<PhoneNumber>>("numberList", Method.GET, true, addParameters).Data;
        }

        /// <summary>
        ///  Releases a number
        /// </summary>
        /// <param name="number">The number to release.</param>
        /// <returns>A Result object indicating whether the operation was successful.</returns>
        public Result ReleaseNumber(string number)
        {
            Action<IRestRequest> addParameters = req =>
            {
                req.AddParameter("number", number);
            };

            return performRequest<Object>("releaseNumber", Method.GET, true, addParameters).ToResult();
        }

        /// <summary>
        ///  Creates a PhaxCode and returns a URL to the barcode image.
        /// </summary>
        /// <param name="metadata">Metadata to associate with this code.</param>
        /// <returns>a URI to the barcode image.</returns>
        public Uri CreatePhaxCode(string metadata = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                if (metadata != null)
                {
                    req.AddParameter("metadata", metadata);
                }
            };

            return performRequest<Url>("createPhaxCode", Method.GET, true, addParameters).Data.Address;
        }

        /// <summary>
        ///  Creates a PhaxCode and returns a byte array representing the image.
        /// </summary>
        /// <param name="metadata">Metadata to associate with this code.</param>
        /// <returns>a byte array of barcode image.</returns>
        public byte[] DownloadPhaxCodePng(string metadata = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                req.AddParameter("redirect", true);

                if (metadata != null)
                {
                    req.AddParameter("metadata", metadata);
                }
            };

            return performDownloadRequest("createPhaxCode", Method.GET, addParameters);
        }

        /// <summary>
        ///  Attaches a PhaxCode to the supplied File.
        /// </summary>
        /// <param name="x">The x-coordinate (in PDF points) of where the PhaxCode should be drawn.
        /// x=0 is the left most point of the page.</param>
        /// <param name="y">The y-coordinate (in PDF points) of where the PhaxCode should be drawn.
        /// y=0 is the bottom most point of the page.</param>
        /// <param name="metadata">The metadata of the PhaxCode you'd like to use.
        /// If you leave this blank, the default account code will be used.</param>
        /// <param name="pageNumber">The page number to attach the code to.</param>
        /// <returns>a byte array of PDF with the code.</returns>
        public byte[] AttachPhaxCodeToPdf(float x, float y, FileInfo pdf, string metadata = null, int? pageNumber = null)
        {
            Action<IRestRequest> requestModifier = req =>
            {
                byte[] fileBytes = File.ReadAllBytes(pdf.DirectoryName + Path.DirectorySeparatorChar + pdf.Name);

                req.AddFile("filename", fileBytes, pdf.Name, "application/pdf");

                req.AddParameter("x", x);
                req.AddParameter("y", y);

                if (metadata != null)
                {
                    req.AddParameter("metadata", metadata);
                }

                if (pageNumber != null)
                {
                    req.AddParameter("page_number", pageNumber);
                }
            };

            return performDownloadRequest("attachPhaxCodeToPdf", Method.POST, requestModifier);
        }

        /// <summary>
        ///  Attaches a PhaxCode to the supplied File and streams the result to the supplied stream
        /// </summary>
        /// <param name="x">The x-coordinate (in PDF points) of where the PhaxCode should be drawn.
        /// x=0 is the left most point of the page.</param>
        /// <param name="y">The y-coordinate (in PDF points) of where the PhaxCode should be drawn.
        /// y=0 is the bottom most point of the page.</param>
        /// <param name="destination">The stream you want the new PDF to be written to.</param>
        /// <param name="metadata">The metadata of the PhaxCode you'd like to use.
        /// If you leave this blank, the default account code will be used.</param>
        /// <param name="pageNumber">The page number to attach the code to.</param>
        /// <returns>a byte array of PDF with the code.</returns>
        public void AttachPhaxCodeToPdf(float x, float y, FileInfo pdf, Stream destination, string metadata = null, int? pageNumber = null)
        {
            Action<IRestRequest> requestModifier = req =>
            {
                req.ResponseWriter = (responseStream) => responseStream.CopyTo(destination);

                byte[] fileBytes = File.ReadAllBytes(pdf.DirectoryName + Path.DirectorySeparatorChar + pdf.Name);

                req.AddFile("filename", fileBytes, pdf.Name, "application/pdf");

                req.AddParameter("x", x);
                req.AddParameter("y", y);

                if (metadata != null)
                {
                    req.AddParameter("metadata", metadata);
                }

                if (pageNumber != null)
                {
                    req.AddParameter("page_number", pageNumber);
                }
            };

            performStreamRequest("attachPhaxCodeToPdf", Method.POST, requestModifier);
        }

        /// <summary>
        ///  Sends a fax
        /// </summary>
        /// <param name="toNumber">The number to send the fax to</param>
        /// <param name="file">The file to send. Supoorts doc, docx, pdf, tif, jpg, odt, txt, html and png</param>
        /// <param name="options">Additional fax options.</param>
        /// <param name="metadata">The metadata of the PhaxCode you'd like to use.
        /// If you leave this blank, the default account code will be used.</param>
        /// <param name="pageNumber">The page number to attach the code to.</param>
        /// <returns>a string representing a fax id.</returns>
        public string Send(string toNumber, FileInfo file, FaxOptions options = null)
        {
            return Send(new List<string> { toNumber }, new List<FileInfo> { file }, options);
        }

        /// <summary>
        ///  Sends a fax
        /// </summary>
        /// <param name="toNumbers">The numbers to send the fax to</param>
        /// <param name="file">The file to send. Supoorts doc, docx, pdf, tif, jpg, odt, txt, html and png</param>
        /// <param name="options">Additional fax options.</param>
        /// <param name="metadata">The metadata of the PhaxCode you'd like to use.
        /// If you leave this blank, the default account code will be used.</param>
        /// <param name="pageNumber">The page number to attach the code to.</param>
        /// <returns>a string representing a fax id.</returns>
        public string Send(IEnumerable<string> toNumbers, FileInfo file, FaxOptions options = null)
        {
            return Send(toNumbers, new List<FileInfo> { file }, options);
        }

        /// <summary>
        ///  Sends a fax
        /// </summary>
        /// <param name="toNumber">The number to send the fax to</param>
        /// <param name="files">The files to send. Supoorts doc, docx, pdf, tif, jpg, odt, txt, html and png</param>
        /// <param name="options">Additional fax options.</param>
        /// <param name="metadata">The metadata of the PhaxCode you'd like to use.
        /// If you leave this blank, the default account code will be used.</param>
        /// <param name="pageNumber">The page number to attach the code to.</param>
        /// <returns>a string representing a fax id.</returns>
        public string Send(string toNumber, IEnumerable<FileInfo> files, FaxOptions options = null)
        {
            return Send(new List<string> { toNumber }, files, options);
        }

        /// <summary>
        ///  Sends a fax
        /// </summary>
        /// <param name="toNumbers">The numbers to send the fax to</param>
        /// <param name="files">The files to send. Supoorts doc, docx, pdf, tif, jpg, odt, txt, html and png</param>
        /// <param name="options">Additional fax options.</param>
        /// <param name="metadata">The metadata of the PhaxCode you'd like to use.
        /// If you leave this blank, the default account code will be used.</param>
        /// <param name="pageNumber">The page number to attach the code to.</param>
        /// <returns>a string representing a fax id.</returns>
        public string Send(IEnumerable<string> toNumbers, IEnumerable<FileInfo> files, FaxOptions options = null)
        {
            Action<IRestRequest> requestModifier = req =>
            {
                foreach (var file in files)
                {
                    byte[] fileBytes = File.ReadAllBytes(file.DirectoryName + Path.DirectorySeparatorChar + file.Name);

                    req.AddFile("filename[]", fileBytes, file.Name, "application/octet");
                }

                foreach (var number in toNumbers)
                {
                    req.AddParameter("to[]", number);
                }
                
                if (options != null)
                {
                    // Add all the scalar properties
                    var props = typeof(FaxOptions).GetProperties();
                    foreach (var prop in props)
                    {
                        var serializeAs = prop.GetCustomAttributes(false)
                            .OfType<SerializeAsAttribute>()
                            .FirstOrDefault();

                        if (serializeAs != null)
                        {
                            object value = prop.GetValue(options, null);

                            if (value != null)
                            {
                                req.AddParameter(serializeAs.Value, value);
                            }
                        }
                    }

                    // Add the tags
                    foreach (var pair in options.Tags)
                    {
                        req.AddParameter("tag[" + pair.Key + "]", pair.Value);
                    }
                }
            };

            var longId = performRequest<dynamic>("send", Method.POST, true, requestModifier).Data["faxId"];

            return longId.ToString();
        }

        /// <summary>
        ///  Downloads a fax
        /// </summary>
        /// <param name="faxId">The id of the fax to download.</param>
        /// <param name="fileType">The file type of the download. Specify "s" for a small JPEG,
        /// "l" for a large JPEG, or "p" for PDF. If you don't specify this, it will be a PDF</param>
        /// <returns>A byte array representing the fax in the specified format.</returns>
        public byte[] DownloadFax(string faxId, string fileType = null)
        {
            Action<IRestRequest> requestModifier = req =>
            {
                req.AddParameter("id", faxId);

                if (fileType != null)
                {
                    req.AddParameter("type", fileType);
                }
            };

            return performDownloadRequest("faxFile", Method.GET, requestModifier);
        }

        /// <summary>
        ///  Downloads a hosted document
        /// </summary>
        /// <param name="faxId">The id of the fax to download.</param>
        /// <param name="metadata">Custom metadata to be associated with the PhaxCode that
        /// will be attached to the hosted document. If not present, the basic PhaxCode
        /// for your account will be used.</param>
        /// <returns>A byte array represent the hosted document PDF.</returns>
        [Obsolete("GetHostedDocument is deprecated, please use DownloadFax instead.")]
        public byte[] GetHostedDocument(string name, string metadata = null)
        {
            Action<IRestRequest> requestModifier = req =>
            {
                req.AddParameter("name", name);

                if (metadata != null)
                {
                    req.AddParameter("metadata", metadata);
                }
            };

            return performDownloadRequest("getHostedDocument", Method.GET, requestModifier);
        }

        private byte[] readAllBytes (Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private void performStreamRequest(string resource, Method method, Action<IRestRequest> requestModifier)
        {
            var request = new RestRequest();

            // Run any custom modifications
            requestModifier(request);

            request.AddParameter(PhaxioConstants.KEY_NAME, key);
            request.AddParameter(PhaxioConstants.SECRET_NAME, secret);

            request.Method = method;
            request.Resource = resource;

            var response = client.Execute(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner exception.";
                var phaxioException = new ApplicationException(message, response.ErrorException);
                throw phaxioException;
            }

            if (response.ContentType == "application/json")
            {
                var json = new JsonDeserializer();

                var phaxioResponse = json.Deserialize<Response<Object>>(response);

                throw new ApplicationException(phaxioResponse.Message);
            }
        }

        // TODO: Figure out how to combine performRequests methods
        private byte[] performDownloadRequest(string resource, Method method, Action<IRestRequest> requestModifier)
        {
            var request = new RestRequest();

            // Run any custom modifications
            requestModifier(request);

            request.AddParameter(PhaxioConstants.KEY_NAME, key);
            request.AddParameter(PhaxioConstants.SECRET_NAME, secret);

            request.Method = method;
            request.Resource = resource;

            var response = client.Execute(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner exception.";
                var phaxioException = new ApplicationException(message, response.ErrorException);
                throw phaxioException;
            }

            if (response.ContentType == "application/json")
            {
                var json = new JsonDeserializer();

                var phaxioResponse = json.Deserialize<Response<Object>>(response);

                throw new ApplicationException(phaxioResponse.Message);
            }

            return response.RawBytes;
        }

        private Response<T> performRequest<T>(string resource, Method method)
        {
            return performRequest<T>(resource, method, true, r => { });
        }

        private Response<T> performRequest<T>(string resource, Method method, bool auth, Action<IRestRequest> requestModifier)
        {
            var request = new RestRequest();

            if (auth)
            {
                request.AddParameter(PhaxioConstants.KEY_NAME, key);
                request.AddParameter(PhaxioConstants.SECRET_NAME, secret);
            }

            // Run any custom modifications
            requestModifier(request);

            request.Method = method;
            request.Resource = resource;

            var response = client.Execute<Response<T>>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner exception.";
                var phaxioException = new ApplicationException(message, response.ErrorException);
                throw phaxioException;
            }

            // If T is Object, it means that the method will return a
            // bool indicating success or failure.
            if (typeof(T) != typeof(Object) && !response.Data.Success)
            {
                throw new ApplicationException(response.Data.Message);
            }

            return response.Data;
        }
    }
}