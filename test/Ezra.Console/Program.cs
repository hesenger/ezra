using System.Net.Sockets;

namespace Ezra.Console;

public static class Program
{
    public static void Main(string[] args)
    {
        var cancellation = new CancellationTokenSource();
        System.Console.CancelKeyPress += (sender, eventArgs) =>
            cancellation.Cancel();

        var listener = TcpListener.Create(8080);
        listener.Start();
        ThreadPool.QueueUserWorkItem(_ =>
        {
            while (!cancellation.Token.IsCancellationRequested)
            {
                var client = listener.AcceptTcpClient();
                HandleRequest(client);
            }
        });

        while (!cancellation.Token.IsCancellationRequested) { }
        listener.Stop();
    }

    private static void HandleRequest(TcpClient client)
    {
        var stream = client.GetStream();
        var response = new MemoryStream();
        var processor = new RequestProcessor();
        processor.MapHandler("/hello", new HelloHandler());

        processor.Process(stream, response);
        response.Position = 0;
        System.Console.WriteLine(
            new StreamReader(response, leaveOpen: true).ReadToEnd()
        );

        response.Position = 0;
        stream.Write(response.ToArray());
        stream.Flush();
        client.Close();
    }
}

public class HelloHandler : IRequestHandler
{
    public void Handle(IRequest request, IResponse response)
    {
        System.Console.WriteLine($"{request.Method} {request.Path}");
        response.Write("Hello World!");
    }
}
