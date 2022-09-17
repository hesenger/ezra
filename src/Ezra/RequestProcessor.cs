using System.Text;

namespace Ezra;

public class RequestProcessor
{
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
            var request = new RequestParser();
            request.Parse(content);
            writer.WriteLine("HTTP/1.1 200 OK");
        }
        catch
        {
            writer.WriteLine("HTTP/1.1 400 Bad Request");
        }
    }

    public void MapHandler(string path, IRequestHandler handler) { }
}

public interface IRequest
{
    string Method { get; }
    string Path { get; }
    IDictionary<string, string> Headers { get; }
}

public interface IResponse
{
    void Write(string content);
}

public interface IRequestHandler
{
    public void Handle(IRequest request, IResponse response);
}
