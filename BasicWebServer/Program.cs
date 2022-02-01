using System.Net;
using System.Net.Sockets;
using System.Text;

var ipAdress = IPAddress.Parse("127.0.0.1");

var port = 8080;

var serverListener = new TcpListener(ipAdress, port);

serverListener.Start();

Console.WriteLine("Slusham anal grozen");

while (true)
{
    var connection = serverListener.AcceptTcpClient();

    var networkStream = connection.GetStream();

    var message = "Qj mi kura dildo mazno";

    var messageLength = Encoding.UTF8.GetBytes(message);

    var responce = $@"HTTP/1.1 200 OK
Content-Type text/plain; charset=UTF-8
Content-Length: {messageLength}

{message}";

    var responseBytes = Encoding.UTF8.GetBytes(responce);

    networkStream.Write(responseBytes);

    connection.Close(); 
}