using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.HTTP
{
    public class Response
    {
        public Response(StatusCode statusCode)
        {
            StatusCode = statusCode;
            Headers.Add(Header.Server, "My Web Server");
            Headers.Add(Header.Date, $"{DateTime.UtcNow:r}");
        }

        public StatusCode StatusCode { get; init; }

        public HeaderCollection Headers { get; } = new HeaderCollection();

        public string Body { get; set; }

        public byte[] FileContent { get; set; }

        public CookieCollection Cookies { get; } = new CookieCollection();

        public override string ToString()
        {
            var result = new StringBuilder();

            result.AppendLine($"HTTP/1.1 {(int)StatusCode} {StatusCode}");

            foreach (var header in Headers)
            {
                result.AppendLine(header.ToString());
            }

            foreach (var cookie in Cookies)
            {
                result.AppendLine($"{Header.SetCookie}: {cookie}");
            }

            result.AppendLine();

            if (!string.IsNullOrEmpty(Body))
            {
                result.Append(Body);
            }

            return result.ToString();
        }
    }
}
