using BasicWebServer.Server.Controllers;
using BasicWebServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Routing
{
    public static class RoutingTableExtension
    {
        public static IRoutingTable MapGet<TController>(
            this IRoutingTable routingTable,
            string path,
            Func<TController, Response> controllerFunction)
            where TController : Controller
        => routingTable.Map(Method.Get, path, request => controllerFunction(CreateController<TController>(request)));
        
        public static IRoutingTable MapPost<TController>(
            this IRoutingTable routingTable,
            string path,
            Func<TController, Response> controllerFunction)
            where TController : Controller
        => routingTable.Map(Method.Post, path, request => controllerFunction(CreateController<TController>(request)));

        private static TController CreateController<TController>(Request request)
        => (TController)Activator.CreateInstance(typeof(TController) , new[] { request });
    }
}
