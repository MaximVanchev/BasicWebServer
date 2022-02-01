﻿using BasicWebServer.Server.Controllers;
using BasicWebServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BasicWebServer.Demo.Controllers
{
    public class HomeController : Controller
    {
        private const string DownloadForm = @"<form action = '/Content' method='POST'>
<input type = 'submit' value ='Download Sites Content' />
</form>";

        private const string HtmlForm = @"<form action ='/HTML' method='POST'>
    Name: <input type='text' name='Name'/>
    Age: <input type='number' name ='Age'/>
    <input type='submit' value ='Save' />
</form>";

        private const string FileName = @"content.txt";

        private const string Site = "https://www.pornhub.com/view_video.php?viewkey=ph5a7ba32a1893c";
        public HomeController(Request request)
            : base(request)
        { }

        public Response Index() => Text("Hello from the server!");

        public Response Redirect() => Redirect(Site);

        public Response Html() => Html(HtmlForm);

        public Response HtmlFormPost()
        {
            var formData = string.Empty;

            foreach (var (key, value) in Request.Form)
            {
                formData += $"{key} - {value}";
                formData += Environment.NewLine;
            }

            return Text(formData);
        }

        public Response Content() => Html(DownloadForm);

        public Response DownloadContent()
        {
            DownloadSitesAsTextFile(FileName, new string[] { Site })
                .Wait();

            return FileContent(FileName);
        }

        public Response Cookies()
        {
            var requestHasCookies = Request.Cookies.Any(c => c.Name != BasicWebServer.Server.HTTP.Session.SessionCookieName);

            var bodyText = "";

            if (requestHasCookies)
            {
                var cookieText = new StringBuilder();
                cookieText.AppendLine("<h1>Cookies</h1>");

                cookieText
                    .Append("<table border='1'><tr><th>Name</th><th>Value</th></tr>");

                foreach (var cookie in Request.Cookies)
                {
                    cookieText.Append("<tr>");
                    cookieText
                        .Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
                    cookieText
                        .Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>");
                    cookieText.Append("</tr>");
                }
                cookieText.Append("</table>");

                bodyText = cookieText.ToString();
            }

            var cookies = new CookieCollection();

            if (!requestHasCookies)
            {
                cookies.Add("My-Cookie", "My-Value");
                cookies.Add("My-Second-Cookie", "My-Second-Value");
                bodyText = "<h1>Cookies set!</h1>";
            }

            return Html(bodyText, cookies);
        }

        public Response Session()
        {
            var sessionExists = Request.Session.ContainsKey(BasicWebServer.Server.HTTP.Session.SessionCurrentDateKey);

            if (sessionExists)
            {
                var currentDate = Request.Session[BasicWebServer.Server.HTTP.Session.SessionCurrentDateKey];

                return Text($"Stored date: {currentDate}!");
            }

            return Text("Current date stored!");
        }

        private static async Task<string> DownloadSiteContent(string url)
        {
            var httpClient = new HttpClient();

            using (httpClient)
            {
                var response = await httpClient.GetAsync(url);

                var html = await response.Content.ReadAsStringAsync();

                return html.Substring(0, 2000);
            }
        }

        private static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
        {
            List<Task<string>> downloads = new List<Task<string>>();

            foreach (var url in urls)
            {
                downloads.Add(DownloadSiteContent(url));
            }

            var responses = await Task.WhenAll(downloads);

            var responsesString = string.Join(Environment.NewLine + new string('-', 100), responses);

            await File.WriteAllTextAsync(fileName, responsesString);
        }
    }
}
