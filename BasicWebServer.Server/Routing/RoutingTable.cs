using BasicWebServer.Server.Common;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Routing
{
    public class RoutingTable : IRoutingTable
    {
        private readonly Dictionary<Method, Dictionary<string, Func<Request , Response>>> routes;

        public RoutingTable()
        {
            routes = new Dictionary<Method, Dictionary<string, Func<Request, Response>>>()
            {
                [Method.Get] = new(StringComparer.InvariantCultureIgnoreCase),
                [Method.Post] = new(StringComparer.InvariantCultureIgnoreCase),
                [Method.Put] = new(StringComparer.InvariantCultureIgnoreCase),
                [Method.Delete] = new(StringComparer.InvariantCultureIgnoreCase)
            };
        }
        public IRoutingTable Map(Method method, string path, Func<Request, Response> responseFunction)
        {
            Guard.AgainstNull(path, nameof(path));
            Guard.AgainstNull(responseFunction, nameof(responseFunction));

            switch (method)
            {
                case Method.Get:
                    MapGet(path, responseFunction);
                    break;
                case Method.Post:
                    MapPost(path, responseFunction);
                    break;
                case Method.Delete:
                case Method.Put:
                default:
                    throw new InvalidOperationException($"Method '{nameof(method)}' is not supported.");
            }

            return this;
        }
            

        private IRoutingTable MapGet(string path, Func<Request, Response> responseFunction)
        {
            routes[Method.Get][path] = responseFunction;

            return this;
        }

        private IRoutingTable MapPost(string path, Func<Request, Response> responseFunction)
        {
            routes[Method.Post][path] = responseFunction;

            return this;
        }

        public Response MatchRequest(Request request)
        {
            var requestMethod = request.Method;
            var requestUrl = request.Url;

            if (!routes.ContainsKey(requestMethod) || ! routes[requestMethod].ContainsKey(requestUrl))
            {
                return new NotFoundResponse();
            }

            var responseFunk = routes[requestMethod][requestUrl];

            return responseFunk(request);
        }
    }
}
