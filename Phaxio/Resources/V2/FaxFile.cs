using Phaxio.ThinRestClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Resources.V2
{
    public class FaxFile
    {
        public PhaxioClient PhaxioClient { get; set; }
        private int faxId;
        private string thumbnail;
        public FaxFile(int faxId, string thumbnail = null)
        {
            this.faxId = faxId;
            this.thumbnail = thumbnail;
        }

        public FileInfo ToFileInfo(string name)
        {
            File.WriteAllBytes(name, Bytes);

            return new FileInfo(name);
        } 

        public byte[] Bytes
        {
            get
            {
                Action<IRestRequest> requestModifier = req =>
                {
                    if (thumbnail != null)
                    {
                        req.AddParameter("thumbnail", thumbnail);
                    }
                };

                return PhaxioClient.download("faxes/" + faxId + "/file", Method.GET, requestModifier);
            }
        }

        public FaxFile SmallJpeg
        {
            get
            {
                return new FaxFile(faxId, "s");
            }
        }

        public FaxFile LargeJpeg
        {
            get
            {
                return new FaxFile(faxId, "l");
            }
        }

        public FaxFile Pdf
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        ///  Deletes a fax's files
        /// </summary>
        public void Delete()
        {
            PhaxioClient.request<Object>("faxes/" + faxId + "/file", Method.DELETE).ToResult();
        }
    }
}
