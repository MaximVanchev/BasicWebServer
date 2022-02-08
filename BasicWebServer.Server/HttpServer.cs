using BasicWebServer.Server.Common;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Routing;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BasicWebServer.Server
{
    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener serverListener;
        private readonly RoutingTable routingTable;

        public HttpServer(string ipAddress , int port, Action<IRoutingTable> routingTableConfiguration)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;   

            serverListener = new TcpListener(this.ipAddress , port);

            routingTableConfiguration(routingTable = new RoutingTable());

            ServiceCollection = new ServiceCollection();
        }

        public HttpServer(int port, Action<IRoutingTable> routingTableConfiguration)
            : this("127.0.0.1" , port , routingTableConfiguration)
        {      
        }

        public HttpServer(Action<IRoutingTable> routingTableConfiguration)
            : this(8080 , routingTableConfiguration)
        {
        }

        public readonly IServiceCollection ServiceCollection;

        public async Task Start()
        {
            serverListener.Start();

            Console.WriteLine("Slusham");

            while (true)
            {
                var connection = await serverListener.AcceptTcpClientAsync();

                _ = Task.Run(async () =>
                {
                    var networkStream = connection.GetStream();

                    var requestText = await ReadRequest(networkStream);

                    Console.WriteLine(requestText);

                    var request = Request.Parse(requestText , ServiceCollection);

                    var response = routingTable.MatchRequest(request);

                    AddSession(request, response);

                    await WhrieResponse(networkStream, response);

                    connection.Close();
                });
            }
        }

        private static void AddSession(Request request, Response response)
        {
            var sessionExists = request.Session.ContainsKey(Session.SessionCurrentDateKey);

            if (!sessionExists)
            {
                request.Session[Session.SessionCurrentDateKey] = DateTime.Now.ToString();

                response.Cookies.Add(Session.SessionCookieName ,request.Session.Id);
            }
        }

        private static async Task WhrieResponse(NetworkStream networkStream, Response response)
        {
            var responseBytes = Encoding.UTF8.GetBytes(response.ToString());

            if (response.FileContent != null)
            {
                responseBytes = responseBytes.Concat(response.FileContent).ToArray();
            }

            await networkStream.WriteAsync(responseBytes);
        }

        private static async Task<string> ReadRequest(NetworkStream networkStream)
        {
            var bufferLength = 1024;
            var buffer = new byte[bufferLength];

            var totalBytes = 0;

            var requestBuilder = new StringBuilder();

            do
            {
                var bytesRead = await networkStream.ReadAsync(buffer, 0, bufferLength);

                totalBytes += bytesRead;

                if (totalBytes > 10 * bufferLength)
                {
                    throw new InvalidOperationException("Request is too large.");
                }

                requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0 , bytesRead));

            } while (networkStream.DataAvailable);

            return requestBuilder.ToString();
        }
    }
}