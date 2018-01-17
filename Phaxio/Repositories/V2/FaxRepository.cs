using Phaxio.Entities;
using Phaxio.Resources.V2;
using Phaxio.Helpers;
using Phaxio.ThinRestClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Phaxio.Repositories.V2
{
    using Fax = Resources.V2.Fax;

    public class FaxRepository
    {
        private PhaxioClient context;

        public FaxRepository(PhaxioClient context)
        {
            this.context = context;
        }

        /// <summary>
        ///  Sends a fax
        /// </summary>
        /// <param name="options">A dictionary representing all the parameters for the fax. See the Phaxio documentation for the exact parameter names.</param>
        /// <returns>a string representing a fax id.</returns>
        public Fax Create(Dictionary<string, object> options)
        {
            Action<IRestRequest> requestModifier = req =>
            {
                foreach (var option in options)
                {
                    req.AddParameter(option.Key, option.Value);
                }
            };

            return context.request<Fax>("faxes", Method.POST, true, requestModifier).Data;
        }

        /// <summary>
        ///  Sends a fax
        /// </summary>
        /// <param name="to">The number (with country code) to send the fax to. Ignored if ToNumbers is set.</param>
        /// <param name="toNumbers">The numbers (with country code) to send the fax to.</param>
        /// <param name="contentUrl">A URL to be rendered and sent as the fax content. Ignored if ContentUrls is set.</param>
        /// <param name="contentUrls">A list of URLs to be rendered and sent as the fax content.</param>
        /// <param name="file">The file to send. Supports doc, docx, pdf, tif, jpg, odt, txt, html and png.</param>
        /// <param name="files">The files to send. Supports doc, docx, pdf, tif, jpg, odt, txt, html and png.</param>
        /// <param name="headerText">Text that will be displayed at the top of each page of the fax. 50 characters maximum. Default header text is "-".</param>
        /// <param name="batchDelay">Enables batching and specifies the amount of time, in seconds, before the batch is fired. Maximum delay is 3600 (1 hour).</param>
        /// <param name="batchCollisionAvoidance">When BatchDelaySeconds is set, fax will be blocked until the receiving machine is no longer busy.</param>
        /// <param name="callbackUrl">You can specify a callback url that will override the one you have defined globally for your account.</param>
        /// <param name="cancelTimeout">
        /// A number of minutes after which the fax will be canceled if it hasn't yet completed. Must be between 3 and 60.
        /// Additionally, for faxes with a BatchDelaySeconds, the CancelTimeoutAfter must be at least 3 minutes after the batch_delay.
        /// </param>
        /// <param name="tags">Tags for your fax. You may specify a maximum of 10.</param>
        /// <param name="callerId">A Phaxio phone number you would like to use for the caller id, in E.164 format (+[country code][number])</param>
        /// <param name="testFail">When using a test API key, this will simulate a sending failure at Phaxio.</param>
        /// <param name="byteArrays">The bytes representing files to send. You must also specify the filenames in the same order in the fileNames parameter. Supports doc, docx, pdf, tif, jpg, odt, txt, html and png.</param>
        /// <param name="fileNames">The filenames of the files represented in the byteArrays parameter.</param>
        /// <returns>a Fax object.</returns>
        public Fax Create(string to = null,
            IEnumerable<string> toNumbers = null,
            string contentUrl = null,
            IEnumerable<string> contentUrls = null,
            FileInfo file = null,
            IEnumerable<FileInfo> files = null,
            string headerText = null,
            int? batchDelay = null,
            bool? batchCollisionAvoidance = null,
            string callbackUrl = null,
            int? cancelTimeout = null,
            Dictionary<string, string> tags = null,
            string callerId = null,
            string testFail = null,
            IEnumerable<byte[]> byteArrays = null,
            IEnumerable<string> fileNames = null
        )
        {
            Action<IRestRequest> requestModifier = req =>
            {
                if (to != null)
                {
                    req.AddParameter("to", to);
                }

                if (toNumbers != null)
                {
                    foreach (var number in toNumbers)
                    {
                        req.AddParameter("to[]", number);
                    }
                }

                if (contentUrl != null)
                {
                    req.AddParameter("content_url", contentUrl);
                }

                if (contentUrls != null)
                {
                    foreach (var url in contentUrls)
                    {
                        req.AddParameter("content_url[]", url);
                    }
                }

                if (file != null)
                {
                    byte[] fileBytes = File.ReadAllBytes(file.DirectoryName + Path.DirectorySeparatorChar + file.Name);

                    req.AddFile("file[]", fileBytes, file.Name, "application/octet");
                }

                if (files != null)
                {
                    foreach (var fileInfo in files)
                    {
                        byte[] fileBytes = File.ReadAllBytes(fileInfo.DirectoryName + Path.DirectorySeparatorChar + fileInfo.Name);

                        req.AddFile("file[]", fileBytes, fileInfo.Name, "application/octet");
                    }
                }

                if (headerText != null)
                {
                    req.AddParameter("header_text", headerText);
                }

                if (batchDelay != null)
                {
                    req.AddParameter("batch_delay", batchDelay);
                }

                if (batchCollisionAvoidance != null)
                {
                    req.AddParameter("batch_collision_avoidance", batchCollisionAvoidance);
                }

                if (callbackUrl != null)
                {
                    req.AddParameter("callback_url", callbackUrl);
                }

                if (cancelTimeout != null)
                {
                    req.AddParameter("cancel_timeout", cancelTimeout);
                }

                if (tags != null)
                {
                    foreach (var tag in tags)
                    {
                        req.AddParameter("tag[" + tag.Key + "]", tag.Value);
                    }
                }

                if (callerId != null)
                {
                    req.AddParameter("caller_id", callerId);
                }

                if (testFail != null)
                {
                    req.AddParameter("test_fail", testFail);
                }

                if (byteArrays != null)
                {
                    if (fileNames == null || byteArrays.Count() != fileNames.Count())
                    {
                        throw new ArgumentException("You must specify one filename in the fileNames parameter for every array in byteArrays.");
                    }

                    var byteEnum = byteArrays.GetEnumerator();
                    var fileNameEnum = fileNames.GetEnumerator();    
                    
                    while(byteEnum.MoveNext() && fileNameEnum.MoveNext()) {
                        req.AddFile("file[]", byteEnum.Current, fileNameEnum.Current, "application/octet");
                    }
                }
            };

            return context.request<Fax>("faxes", Method.POST, true, requestModifier).Data;
        }

        /// <summary>
        ///  Gets the fax info.
        /// </summary>
        /// <param name="faxId">The id of the fax to retrieve.</param>
        public Fax Retrieve(int id)
        {
            return context.request<Fax>("faxes/" + id, Method.GET).Data;
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
        /// <returns>A IEnumerable&lt;Fax&gt;</returns>
        public IEnumerable<Fax> List(DateTime? createdBefore = null,
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
                    req.AddParameter("created_before", createdBefore.Value.Rfc3339());
                }

                if (createdAfter != null)
                {
                    req.AddParameter("created_after", createdAfter.Value.Rfc3339());
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

                    req.AddParameter("tag[" + tagName + "]", tagValue);
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

            return context.pagedRequest<Fax>("faxes", Method.GET, true, addParameters);
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

            return context.request<Object>("faxes", Method.POST, true, addParameters).ToResult();
        }
    }
}
