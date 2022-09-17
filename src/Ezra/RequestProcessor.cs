using System.Text;

namespace Ezra;

public class RequestProcessor
{
    private readonly Dictionary<string, IRequestHandler> _handlers = new();

    public void Process(Stream content, Stream responseStream)
    {
        using var writer = new StreamWriter(
            stream: responseStream,
            encoding: Encoding.UTF8,
            leaveOpen: true
        );
        writer.NewLine = "\r\n";

        try
        {
            var request = new RequestParser().Parse(content);
            var handler = GetHandler(request);

            var response = new Response();
            handler.Handle(request, response);
            response.Serialize(writer);
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

    private IRequestHandler GetHandler(IRequest request)
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
