using BasicWebServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Responses.ContentResponses
{
    public class HtmlResponse : ContentResponse
    {
        public HtmlResponse(string text)
            :base(text , ContentType.Html)
        {
        }
    }
}
