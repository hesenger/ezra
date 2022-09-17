using System.Net.Sockets;

namespace Ezra.Console;

public static class Program
{
    public static void Main(string[] args)
    {
        var listener = TcpListener.Create(8080);
        listener.Start();
        var client = listener.AcceptTcpClient();
        var stream = client.GetStream();
        var response = new MemoryStream();
        var handler = new RequestHandler();
        handler.HandleRequest(stream, response);
        response.Position = 0;
        stream.Write(response.ToArray());
        client.Close();
        listener.Stop();
    }
}
