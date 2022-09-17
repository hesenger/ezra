using System.Globalization;
using System.Net.Sockets;
using Ezra.Processing;

namespace Ezra;

public class EzraServer
{
    private readonly Dictionary<string, IRequestHandler> _handlers = new();

    public EzraServer MapHandler(string path, IRequestHandler handler)
    {
        _handlers.Add(path, handler);
        return this;
    }

    public void Start(string[] commandLineArgs)
    {
        var port =
            commandLineArgs.Length > 0
                ? int.Parse(commandLineArgs[0], CultureInfo.InvariantCulture)
                : 8080;

        Start(port);
    }

    public void Start(int port = 8080)
    {
        var cancellation = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, eventArgs) => cancellation.Cancel();

        var listener = TcpListener.Create(port);
        listener.Start();

        Console.WriteLine($"Ezra server started");
        Console.WriteLine($"Listening on port {port}");

        while (!cancellation.Token.IsCancellationRequested)
        {
            var client = listener.AcceptTcpClient();
            HandleRequest(client);
        }

        listener.Stop();
    }

    private void HandleRequest(TcpClient client)
    {
        var stream = client.GetStream();
        var response = new MemoryStream();
        var processor = new RequestProcessor(_handlers);
        processor.Process(stream, response);
        response.Position = 0;
        stream.Write(response.ToArray());
        stream.Flush();
        client.Close();
    }
}
