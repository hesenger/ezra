using System.Text;

namespace Ezra;

public class RequestProcessor
{
    private readonly Dictionary<string, IRequestHandler> _handlers = new();

    public void Process(Stream content, Stream response)
    {
        using var writer = new StreamWriter(
            stream: response,
            encoding: Encoding.UTF8,
            leaveOpen: true
        );
        writer.NewLine = "\r\n";

        try
        {
            var request = new RequestParser().Parse(content);
            var handler = GetHandlerFor(request);
            handler.Handle(request, new MemoryStream());
        }
        catch (HttpException ex)
        {
            writer.WriteLine($"HTTP/1.1 {ex.Code} {ex.Reason}");
        }
        catch
        {
            writer.WriteLine("HTTP/1.1 500 Internal Server Error");
        }
    }

    private IRequestHandler GetHandlerFor(IRequest request)
    {
        return _handlers.TryGetValue(request.Path, out var handler)
            ? handler
            : throw new HttpException(404, "Not Found", "No handler for path");
    }

    public void MapHandler(string path, IRequestHandler handler)
    {
        _handlers.Add(path, handler);
    }
}

public class HttpException : Exception
{
    public HttpException(int code, string reason, string message)
        : base(message)
    {
        Code = code;
        Reason = reason;
    }

    public int Code { get; }
    public string Reason { get; }
}

public class Request : IRequest
{
    public Request(string method, string path)
    {
        Method = method;
        Path = path;
    }

    public string Method { get; }
    public string Path { get; }
}

public interface IRequest
{
    string Method { get; }
    string Path { get; }
}

public interface IRequestHandler
{
    public void Handle(IRequest request, Stream response);
}
