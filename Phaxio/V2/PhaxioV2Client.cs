using Phaxio.ThinRestClient;
using Phaxio.Entities;
using Phaxio.Entities.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Globalization;

namespace Phaxio.V2
{
    /// <summary>
    ///  This is the main class and starting point for interacting with Phaxio using the V2 API.
    /// </summary>
    public class PhaxioV2Client : BasePhaxioClient
    {
        private const string phaxioApiEndpoint = "https://api.phaxio.com/v2/";

        public PhaxioV2Client(string key, string secret)
            : this(key, secret, new RestClient())
        {
        }

        public PhaxioV2Client(string key, string secret, IRestClient restClient)
            : base(key, secret, new RestClient())
        {
            // Initialize the rest client
            client = restClient;
            client.BaseUrl = new Uri(phaxioApiEndpoint);
        }

        /// <summary>
        ///  Sends a fax
        /// </summary>
        /// <param name="fax">The fax request.</param>
        /// <returns>a string representing a fax id.</returns>
        public string SendFax(FaxRequest fax)
        {
            Action<IRestRequest> requestModifier = req =>
            {
                foreach (var file in combine(fax.File, fax.Files))
                {
                    byte[] fileBytes = File.ReadAllBytes(file.DirectoryName + Path.DirectorySeparatorChar + file.Name);

                    req.AddFile("file[]", fileBytes, file.Name, "application/octet");
                }

                foreach (var number in combine(fax.ToNumber, fax.ToNumbers))
                {
                    req.AddParameter("to[]", number);
                }

                foreach (var url in combine(fax.ContentUrl, fax.ContentUrls))
                {
                    req.AddParameter("content_url[]", url);
                }

                // Add the tags
                foreach (var pair in fax.Tags)
                {
                    req.AddParameter("tag[" + pair.Key + "]", pair.Value);
                }

                // Add all the scalar properties
                var props = typeof(FaxRequest).GetProperties();
                foreach (var prop in props)
                {
                    var serializeAs = prop.GetCustomAttributes(false)
                        .OfType<SerializeAsAttribute>()
                        .FirstOrDefault();

                    if (serializeAs != null)
                    {
                        object value = prop.GetValue(fax);

                        if (value != null)
                        {
                            req.AddParameter(serializeAs.Value, value);
                        }
                    }
                }
            };

            var result = request<dynamic>("faxes", Method.POST, true, requestModifier);

            if (!result.Success)
            {
                throw new ApplicationException(result.Message);
            }

            var longId = result.Data["id"];

            return longId.ToString();
        }

        /// <summary>
        ///  Resends a fax
        /// </summary>
        /// <param name="faxId">The id of the fax to cancel.</param>
        /// <returns>A Result object indicating whether the operation was successful.</returns>
        public Result ResendFax(string faxId, string callbackUrl = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                if (callbackUrl != null)
                {
                    req.AddParameter("callback_url", callbackUrl);
                }
            };

            var escapedId = WebUtility.UrlEncode(faxId);
            return request<Object>("faxes/" + escapedId + "/resend", Method.POST, true, addParameters).ToResult();
        }

        /// <summary>
        ///  Cancels a fax
        /// </summary>
        /// <param name="faxId">The id of the fax to cancel.</param>
        /// <returns>A Result object indicating whether the operation was successful.</returns>
        public Result CancelFax(string faxId)
        {
            var escapedId = WebUtility.UrlEncode(faxId);
            return request<Object>("faxes/" + escapedId + "/cancel", Method.POST).ToResult();
        }

        /// <summary>
        ///  Gets the fax info.
        /// </summary>
        /// <param name="faxId">The id of the fax to cancel.</param>
        /// <returns>A FaxInfo object.</returns>
        public FaxInfo GetFaxInfo(string faxId)
        {
            var escapedId = WebUtility.UrlEncode(faxId);
            return request<FaxInfo>("faxes/" + escapedId, Method.GET).Data;
        }

        /// <summary>
        ///  Downloads a fax
        /// </summary>
        /// <param name="faxId">The id of the fax to download.</param>
        /// <param name="thumbnail">Either 's' (small - 129x167px) or 'l' (large - 300x388px). If specified,
        /// a thumbnail of the requested size will be returned.</param>
        /// <returns>A byte array representing the PDF of the fax, or a JPG if thumbnail was specified.</returns>
        public byte[] DownloadFax(string faxId, string thumbnail = null)
        {
            Action<IRestRequest> requestModifier = req =>
            {
                if (thumbnail != null)
                {
                    req.AddParameter("thumbnail", thumbnail);
                }
            };

            var escapedId = WebUtility.UrlEncode(faxId);

            return download("faxes/" + escapedId + "/file", Method.GET, requestModifier);
        }

        /// <summary>
        ///  Deletes a fax
        /// </summary>
        /// <param name="faxId">The id of the fax to delete.</param>
        /// <returns>A Result object indicating whether the operation was successful.</returns>
        public Result DeleteFax(string faxId)
        {
            var escapedId = WebUtility.UrlEncode(faxId);
            return request<Object>("faxes/" + escapedId, Method.DELETE).ToResult();
        }

        /// <summary>
        ///  Deletes a fax's files
        /// </summary>
        /// <param name="faxId">The id of the fax.</param>
        /// <returns>A Result object indicating whether the operation was successful.</returns>
        public Result DeleteFaxFiles(string faxId)
        {
            var escapedId = WebUtility.UrlEncode(faxId);
            return request<Object>("faxes/" + escapedId + "/file", Method.DELETE).ToResult();
        }

        /// <summary>
        /// Lists faxes with optional filters.
        /// </summary>
        /// <param name="createdBefore">The end of the range. Defaults to now.</param>
        /// <param name="createdAfter">The beginning of the range. Defaults to one week ago.</param>
        /// <param name="direction">Either 'sent' or 'received'.</param>
        /// <param name="status">The status to filter by.</param>
        /// <param name="phoneNumber">A phone number to filter by.</param>
        /// <param name="tagName">A tag name to filter by (you must specify a tag value as well).</param>
        /// <param name="tagValue">A tag value to filter by (you must specify a tag name as well).</param>
        /// <param name="perPage">The maximum number of results to return per call or "page" (1000 max).</param>
        /// <param name="page">The page number to return for the request. 1-based.</param>
        /// <returns>A PagedResult&lt;FaxInfo&gt;</returns>
        public PagedResult<FaxInfo> ListFaxes(DateTime? createdBefore = null,
            DateTime? createdAfter = null,
            string direction = null,
            string status = null,
            string phoneNumber = null,
            string tagName = null,
            string tagValue = null,
            int? perPage = null,
            int? page = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                if (createdBefore != null)
                {
                    req.AddParameter("created_before", rfc3339(createdBefore.Value));
                }

                if (createdAfter != null)
                {
                    req.AddParameter("created_after", rfc3339(createdAfter.Value));
                }
                
                if (direction != null)
                {
                    req.AddParameter("direction", direction);
                }

                if (status != null)
                {
                    req.AddParameter("status", status);
                }

                if (phoneNumber != null)
                {
                    req.AddParameter("phone_number", phoneNumber);
                }

                if (tagName != null)
                {
                    if (tagValue == null)
                    {
                        throw new ArgumentException("You must supply a tag value if you specify a tag name.");
                    }

                    req.AddParameter("tag["+ tagName + "]", tagValue);
                }

                if (perPage != null)
                {
                    req.AddParameter("per_page", perPage);
                }

                if (page != null)
                {
                    req.AddParameter("page", page);
                }
            };

            var response = request<List<FaxInfo>>("faxes", Method.GET, true, addParameters);

            return new PagedResult<FaxInfo> {
                Success = response.Success,
                Message = response.Message,
                Data = response.Data,
                PagingInfo = response.PagingInfo
            };
        }

        /// <summary>
        /// Lists supported countries with pricing
        /// </summary>
        /// <param name="perPage">The maximum number of results to return per call or "page" (1000 max).</param>
        /// <param name="page">The page number to return for the request. 1-based.</param>
        /// <returns>A PagedResult&lt;Country&gt;</returns>
        public PagedResult<Country> ListSupportedCountries(int? perPage = null,
            int? page = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                if (perPage != null)
                {
                    req.AddParameter("per_page", perPage);
                }

                if (page != null)
                {
                    req.AddParameter("page", page);
                }
            };

            var response = request<List<Country>>("public/countries", Method.GET, true, addParameters);

            return new PagedResult<Country>
            {
                Success = response.Success,
                Message = response.Message,
                Data = response.Data,
                PagingInfo = response.PagingInfo
            };
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
                req.AddParameter("direction", "received");

                byte[] fileBytes = File.ReadAllBytes(file.DirectoryName + Path.DirectorySeparatorChar + file.Name);

                req.AddFile("file", fileBytes, file.Name, "application/octet");

                if (fromNumber != null)
                {
                    req.AddParameter("from_number", fromNumber);
                }

                if (toNumber != null)
                {
                    req.AddParameter("to_number", toNumber);
                }
            };

            return request<Object>("faxes", Method.POST, true, addParameters).ToResult();
        }

        /// <summary>
        ///  Creates a PhaxCode and returns an identifier for the barcode image.
        /// </summary>
        /// <param name="metadata">Metadata to associate with this code.</param>
        /// <returns>an identifier for the PhaxCode.</returns>
        public string GeneratePhaxCode(string metadata = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                if (metadata != null)
                {
                    req.AddParameter("metadata", metadata);
                }
            };

            return request<dynamic>("phax_codes.json", Method.POST, true, addParameters).Data["identifier"];
        }

        /// <summary>
        ///  Creates a PhaxCode and returns a byte array representing the image.
        /// </summary>
        /// <param name="metadata">Metadata to associate with this code.</param>
        /// <returns>a byte array of barcode image.</returns>
        public byte[] GeneratePhaxCodePng(string metadata = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                if (metadata != null)
                {
                    req.AddParameter("metadata", metadata);
                }
            };

            return download("phax_codes.png", Method.POST, addParameters);
        }

        /// <summary>
        ///  Gets the account for this Phaxio instance.
        /// </summary>
        /// <returns>An Account object</returns>
        public AccountStatus GetAccountStatus()
        {
            return request<AccountStatus>("account/status", Method.GET).Data;
        }

        /// <summary>
        ///  Gets the information for a phone number
        /// </summary>
        /// <returns>An PhoneNumberV2 object</returns>
        public PhoneNumberV2 GetNumberInfo(string phoneNumber)
        {
            return request<PhoneNumberV2>("phone_numbers/" + phoneNumber, Method.GET).Data;
        }

        /// <summary>
        ///  Returns a dictionary of area codes available for purchasing Phaxio numbers.
        ///  The keys are areacodes, and the values are their city and state.
        /// </summary>
        /// <param name="tollFree">Whether the number should be tollfree.</param>
        /// <param name="countryCode">The country code to search (e.g., "1" for US).</param>
        /// <param name="country">The two-letter country abbreviation to search (e.g., "US").</param>
        /// <param name="state">A two character state or province abbreviation (e.g. IL or YT).
        /// Will only return area codes available for this state.</param>
        /// <param name="perPage">The maximum number of results to return per call or "page" (1000 max).</param>
        /// <param name="page">The page number to return for the request. 1-based.</param>
        /// <returns>A PagedResult&lt;AreaCode&gt;</returns>
        public PagedResult<AreaCode> ListAreaCodes(bool? tollFree = null,
            string countryCode = null,
            string country = null,
            string state = null,
            int? perPage = null,
            int? page = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                if (tollFree != null)
                {
                    req.AddParameter("toll_free", tollFree);
                }

                if (countryCode != null)
                {
                    req.AddParameter("country_code", countryCode);
                }

                if (country != null)
                {
                    req.AddParameter("country", country);
                }

                if (state != null)
                {
                    if (country == null && countryCode == null)
                    {
                        throw new ArgumentException("You must supply either a country code or a country if you specify a state.");
                    }

                    req.AddParameter("state", state);
                }

                if (perPage != null)
                {
                    req.AddParameter("per_page", perPage);
                }

                if (page != null)
                {
                    req.AddParameter("page", page);
                }
            };

            var response = request<List<AreaCode>>("public/area_codes", Method.GET, false, addParameters);

            return new PagedResult<AreaCode>
            {
                Success = response.Success,
                Message = response.Message,
                Data = response.Data,
                PagingInfo = response.PagingInfo
            };
        }

        /// <summary>
        ///  Lists the phone numbers provisioned to your account.
        /// </summary>
        /// <param name="countryCode">The country code to search (e.g., "1" for US).</param>
        /// <param name="country">The area code search. If this is specified, you must specify the country code.</param>
        /// <param name="perPage">The maximum number of results to return per call or "page" (1000 max).</param>
        /// <param name="page">The page number to return for the request. 1-based.</param>
        /// <returns>A PagedResult&lt;PhoneNumberV2&gt;</returns>
        public PagedResult<PhoneNumberV2> ListAccountNumbers(string countryCode = null,
            string areaCode = null,
            int? perPage = null,
            int? page = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                if (countryCode != null)
                {
                    req.AddParameter("country_code", countryCode);
                }

                if (areaCode != null)
                {
                    if (countryCode == null)
                    {
                        throw new ArgumentException("You must supply either a country code or a country if you specify a state.");
                    }

                    req.AddParameter("area_code", areaCode);
                }

                if (perPage != null)
                {
                    req.AddParameter("per_page", perPage);
                }

                if (page != null)
                {
                    req.AddParameter("page", page);
                }
            };

            var response = request<List<PhoneNumberV2>>("phone_numbers", Method.GET, true, addParameters);

            return new PagedResult<PhoneNumberV2>
            {
                Success = response.Success,
                Message = response.Message,
                Data = response.Data,
                PagingInfo = response.PagingInfo
            };
        }

        /// <summary>
        ///  Provisions a new fax number
        /// </summary>
        /// <param name="countryCode">The country code to provsion the number in.</param>
        /// <param name="areaCode">The area code to provsion the number in.</param>
        /// <param name="callbackUrl">The URL that Phaxio will post to when a fax is recieved at this number.</param>
        /// <returns>A PhoneNumber object representing the new number.</returns>
        public PhoneNumberV2 ProvisionNumber(string areaCode, string countryCode, string callbackUrl = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                req.AddParameter("country_code", countryCode);
                req.AddParameter("area_code", areaCode);

                if (callbackUrl != null)
                {
                    req.AddParameter("callback_url", callbackUrl);
                }
            };

            return request<PhoneNumberV2>("phone_numbers", Method.POST, true, addParameters).Data;
        }

        /// <summary>
        ///  Releases a number
        /// </summary>
        /// <param name="number">The number to release.</param>
        /// <returns>A Result object indicating whether the operation was successful.</returns>
        public Result ReleaseNumber(string number)
        {
            return request<Object>("phone_numbers/" + number, Method.DELETE).ToResult();
        }

        /// <summary>
        /// Returns the PhaxCode properties.
        /// </summary>
        /// <param name="identifier">The identifier of the PhaxCode. If none is passed in, the default PhaxCode is downloaded.</param>
        /// <returns>a PhaxCode object.</returns>
        public PhaxCode RetrievePhaxCodeProperties(string identifier = null)
        {
            var resource = "phax_code";

            if (identifier != null)
            {
                resource += "s/" + WebUtility.UrlEncode(identifier);
            }

            return request<PhaxCode>(resource + ".json", Method.GET).Data;
        }

        /// <summary>
        /// Returns a byte array representing the PhaxCode.
        /// </summary>
        /// <param name="identifier">The identifier of the PhaxCode. If none is passed in, the default PhaxCode is downloaded.</param>
        /// <returns>a byte array of barcode image.</returns>
        public byte[] RetrievePhaxCodePng(string identifier = null)
        {
            var resource = "phax_code";

            if (identifier != null)
            {
                resource += "s/" + WebUtility.UrlEncode(identifier);
            }

            return download(resource + ".png", Method.GET, req => { });
        }

        private IEnumerable<T> combine<T>(T scalar, IEnumerable<T> array)
        {
            return scalar != null ? array.Concat(new[] { scalar }) : array;
        }

        private string rfc3339(DateTime time)
        {
            return time.ToString("yyyy-MM-dd'T'HH:mm:ssK", DateTimeFormatInfo.InvariantInfo);
        }
    }
}
