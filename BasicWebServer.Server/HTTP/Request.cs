using BasicWebServer.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BasicWebServer.Server.HTTP
{
    public class Request
    {
        private static Dictionary<string, Session> Sessions = new();
        public Method Method { get; private set; }

        public string Url { get; private set; }

        public HeaderCollection Headers { get; private set; }

        public string Body { get; private set; }

        public IReadOnlyDictionary<string, string> Form { get; private set; }

        public CookieCollection Cookies { get; private set; }

        public Session Session { get; private set; }

        public static IServiceCollection ServiceCollection { get; private set; }

        public static Request Parse(string request , IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;

            var lines = request.Split("\r\n");

            var startLine = lines.First().Split(" ");

            var method = ParseMethod(startLine[0]);

            var url = startLine[1];

            var headers = ParseHeadres(lines.Skip(1));

            var cookies = ParseCookies(headers);

            var bodyLines = lines.Skip(headers.Count() + 2).ToArray();

            var body = string.Join("\r\n", bodyLines);

            var form = ParseForm(headers, body);

            var session = GetSession(cookies);

            return new Request
            {
                Method = method,
                Url = url,
                Headers = headers,
                Cookies = cookies,
                Body = body,
                Session = session,
                Form = form
            };
        }

        private static Session GetSession(CookieCollection cookies)
        {
            var sessionId = cookies.Contains(Session.SessionCookieName)
                ? cookies[Session.SessionCookieName]
                : Guid.NewGuid().ToString();

            if (!Sessions.ContainsKey(sessionId))
            {
                Sessions[sessionId] = new Session(sessionId);
            }

            return Sessions[sessionId];
        }

        private static CookieCollection ParseCookies(HeaderCollection headers)
        {
            var cookies = new CookieCollection();

            if (headers.Contains(Header.Cookie))
            {
                var cookieHeader = headers[Header.Cookie];

                var allCookies = cookieHeader.Split(";");

                foreach (var cookieText in allCookies)
                {
                    var cookieParts = cookieText.Split("=");

                    var cookieName = cookieParts[0].Trim();

                    var cookieValue = cookieParts[1].Trim();

                    cookies.Add(cookieName, cookieValue);
                }
            }

            return cookies;
        }

        private static Dictionary<string , string> ParseFormData(string bodyLines)
        {
            return HttpUtility.UrlDecode(bodyLines)
                .Split('&')
                .Select(x => x.Split('='))
                .Where(x => x.Length == 2)
                .ToDictionary(
                    x => x[0],
                    x => x[1],
                    StringComparer.InvariantCultureIgnoreCase);
        }

        private static Dictionary<string , string> ParseForm(HeaderCollection headers, string body)
        {
            var forms = new Dictionary<string , string>();

            if (headers.Contains(Header.ContentType) && headers[Header.ContentType] == ContentType.FormUrlEncoded)
            {
                var parsedResult = ParseFormData(body);

                foreach (var (name , value) in parsedResult)
                {
                    forms.Add(name, value);
                }
            }

            return forms;
        }

        private static HeaderCollection ParseHeadres(IEnumerable<string> headerLines)
        {
            var headers = new HeaderCollection();

            foreach (var line in headerLines)
            {
                if (line == string.Empty)
                {
                    break;
                }

                var headerParts = line.Split(":", 2);

                if (headerParts.Length != 2)
                {
                    throw new InvalidOperationException("Request is not valid.");
                }

                var headerName = headerParts[0];

                var headerValue = headerParts[1].Trim();

                headers.Add(headerName, headerValue);
            }

            return headers;
        }

        private static Method ParseMethod(string method)
        {
            try
            {
                return (Method)Enum.Parse(typeof(Method), method , true);
            }
            catch (Exception)
            {

                throw new InvalidOperationException($"Method '{method}' is not supported");
            }
        }
    }
}
