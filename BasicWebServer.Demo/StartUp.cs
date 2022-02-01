using BasicWebServer.Demo.Controllers;
using BasicWebServer.Server;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;
using BasicWebServer.Server.Responses.ContentResponses;
using BasicWebServer.Server.Routing;
using System.Text;
using System.Web;

await ServerStart();

static async Task ServerStart() => await new HttpServer(routes => routes
    .MapGet<HomeController>("/", c => c.Index())
    .MapGet<HomeController>("/HTML", c => c.Html())
    .MapGet<HomeController>("/Redirect", c => c.Redirect())
    .MapPost<HomeController>("/HTML", c => c.HtmlFormPost())
    .MapGet<HomeController>("/Content" , c => c.Content())
    .MapPost<HomeController>("/Content" , c => c.DownloadContent())
    .MapGet<HomeController>("/Cookies" , c => c.Cookies())
    .MapGet<HomeController>("/Session" , c => c.Session())
    .MapGet<UserController>("/Login", c => c.Login())
    .MapPost<UserController>("/Login", c => c.LoginInUser())
    .MapGet<UserController>("/Logout", c => c.Logout())
    .MapGet<UserController>("/UserProfile", c => c.GetUserData())
    ).Start();


