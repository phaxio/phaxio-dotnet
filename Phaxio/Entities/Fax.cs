using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Entities
{
    public class Fax
    {
        private PhaxioClient client;

        public string Id { get; set; }

        internal Fax (PhaxioClient client)
        {
            this.client = client;
        }

        public void Send(string toNumber, FileInfo file, FaxOptions options = null)
        {
            Send(new List<string> { toNumber }, new List<FileInfo> { file }, options);
        }

        public void Send(IEnumerable<string> toNumbers, FileInfo file, FaxOptions options = null)
        {
            Send(toNumbers, new List<FileInfo> { file }, options);
        }


        public void Send(string toNumber, IEnumerable<FileInfo> files, FaxOptions options = null)
        {
            Send(new List<string> { toNumber }, files, options);
        }

        public void Send(IEnumerable<string> toNumbers, IEnumerable<FileInfo> files, FaxOptions options = null)
        {
            Id = client.SendFax(toNumbers, files, options);
        }

        public Result Resend()
        {
            if (Id == null)
            {
                throw new InvalidOperationException("The Id has not been set.");
            }

            return client.ResendFax(Id);
        }

        public Result Cancel()
        {
            if (Id == null)
            {
                throw new InvalidOperationException("The Id has not been set.");
            }

            return client.CancelFax(Id);
        }

        public Result Delete()
        {
            if (Id == null)
            {
                throw new InvalidOperationException("The Id has not been set.");
            }

            return client.DeleteFax(Id);
        }

        public Result Delete(bool filesOnly = false)
        {
            if (Id == null)
            {
                throw new InvalidOperationException("The Id has not been set.");
            }

            return client.DeleteFax(Id, filesOnly);
        }

        public byte[] DownloadFile(string fileType = null)
        {
            if (Id == null)
            {
                throw new InvalidOperationException("The Id has not been set.");
            }

            return client.DownloadFax(Id, fileType);
        }
    }
}
