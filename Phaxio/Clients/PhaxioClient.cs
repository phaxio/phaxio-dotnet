using Phaxio.Entities;
using Phaxio.Entities.Internal;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Phaxio.ThinRestClient;
using Phaxio.Clients.Internal;

namespace Phaxio
{
    /// <summary>
    ///  This is the main class and starting point for interacting with Phaxio.
    /// </summary>
    public class PhaxioClient : BasePhaxioClient
    {
        private const string phaxioApiEndpoint = "https://api.phaxio.com/v1/";

        public PhaxioClient (string key, string secret)
            : this(key, secret, new RestClient())
        {
        }

        public PhaxioClient (string key, string secret, IRestClient restClient)
            : base(key, secret, restClient)
        {
            client.BaseUrl = new Uri(phaxioApiEndpoint);
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

            return download("attachPhaxCodeToPdf", Method.POST, requestModifier);
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

            return request<Object>("faxCancel", Method.GET, true, addParameters).ToResult();
        }

        /// <summary>
        ///  Creates a Fax object
        /// </summary>
        /// <returns>a new fax object.</returns>
        public Fax CreateFax()
        {
            return new Fax(this);
        }

        /// <summary>
        ///  Creates a Fax object
        /// </summary>
        /// <param name="id">The id of the fax you'd like to use.</param>
        /// <returns>a new fax object.</returns>
        public Fax CreateFax(string id)
        {
            return new Fax(this) { Id = id };
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

            return request<Url>("createPhaxCode", Method.GET, true, addParameters).Data.Address;
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

            return request<Object>("deleteFax", Method.GET, true, addParameters).ToResult();
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

            return download("faxFile", Method.GET, requestModifier);
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

            return download("createPhaxCode", Method.GET, addParameters);
        }

        /// <summary>
        ///  Gets the account for this Phaxio instance.
        /// </summary>
        /// <returns>An Account object</returns>
        public Account GetAccountStatus ()
        {
            return request<Account>("accountStatus", Method.GET).Data;
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

            return download("getHostedDocument", Method.GET, requestModifier);
        }

        /// <summary>
        ///  Returns a dictionary of area codes available for purchasing Phaxio numbers.
        ///  The keys are areacodes, and the values are their city and state.
        /// </summary>
        /// <param name="tollFree">Whether the number should be tollfree.</param>
        /// <param name="state">A two character state or province abbreviation (e.g. IL or YT).
        /// Will only return area codes available for this state.</param>
        /// <returns>A Dictionary&lt;string, CityState&gt; with area codes for keys and CityStates for values</returns>
        public Dictionary<string, CityState> ListAreaCodes (bool? tollFree = null, string state = null)
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

            return request<Dictionary<string, CityState>>("areaCodes", Method.POST, false, addParameters).Data;
        }

        /// <summary>
        ///  Lists all of your numbers
        /// </summary>
        /// <param name="areaCode">The area code to filter by.</param>
        /// <param name="number">The number to search for.</param>
        /// <returns>A List of PhoneNumber objects.</returns>
        public List<PhoneNumber> ListNumbers(string areaCode = null, string number = null)
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

            return request<List<PhoneNumber>>("numberList", Method.GET, true, addParameters).Data;
        }

        /// <summary>
        ///  Returns a dictionary of supported countries by Phaxio along with pricing information
        /// </summary>
        /// <returns>A Dictionary&lt;string, Pricing&gt; with countries for keys and Pricing for values</returns>
        public Dictionary<string, Pricing> ListSupportedCountries()
        {
            return request<Dictionary<string, Pricing>>("supportedCountries", Method.POST, false, r => { }).Data;
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

            return request<PhoneNumber>("provisionNumber", Method.GET, true, addParameters).Data;
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

            return request<Object>("releaseNumber", Method.GET, true, addParameters).ToResult();
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

            return request<Object>("resendFax", Method.GET, true, addParameters).ToResult();
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
        public string SendFax(string toNumber, FileInfo file, FaxOptions options = null)
        {
            return SendFax(new List<string> { toNumber }, new List<FileInfo> { file }, options);
        }

        /// <summary>
        ///  Sends a fax
        /// </summary>
        /// <param name="toNumbers">The numbers to send the fax to</param>
        /// <param name="file">The file to send. Supports doc, docx, pdf, tif, jpg, odt, txt, html and png</param>
        /// <param name="options">Additional fax options.</param>
        /// <param name="metadata">The metadata of the PhaxCode you'd like to use.
        /// If you leave this blank, the default account code will be used.</param>
        /// <param name="pageNumber">The page number to attach the code to.</param>
        /// <returns>a string representing a fax id.</returns>
        public string SendFax(IEnumerable<string> toNumbers, FileInfo file, FaxOptions options = null)
        {
            return SendFax(toNumbers, new List<FileInfo> { file }, options);
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
        public string SendFax(string toNumber, IEnumerable<FileInfo> files, FaxOptions options = null)
        {
            return SendFax(new List<string> { toNumber }, files, options);
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
        public string SendFax(IEnumerable<string> toNumbers, IEnumerable<FileInfo> files, FaxOptions options = null)
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
                            object value = prop.GetValue(options);

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

            var result = request<dynamic>("send", Method.POST, true, requestModifier);

            if (!result.Success)
            {
                throw new ApplicationException(result.Message);
            }

            var longId = result.Data["faxId"];

            return longId.ToString();
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

            return request<Object>("testReceive", Method.POST, true, addParameters).ToResult();
        }
    }
}