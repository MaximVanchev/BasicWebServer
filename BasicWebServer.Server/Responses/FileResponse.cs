using BasicWebServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Responses
{
    public class FileResponse : Response
    {
        public FileResponse(string fileName)
            : base(StatusCode.OK)
        {
            FileName = fileName;
            Headers.Add(Header.ContentType, ContentType.File);
        }

        public string FileName { get; init; }

        public override string ToString()
        {
            if (File.Exists(FileName))
            {
                Body = string.Empty;

                FileContent = File.ReadAllBytes(FileName);

                var fileBytesCount = new FileInfo(FileName).Length;

                Headers.Add(Header.ContentLength, fileBytesCount.ToString());

                Headers.Add(Header.ContentDisposition, $"attachment; filename=\"{FileName}\"");
            }

            return base.ToString();
        }
    }
}
