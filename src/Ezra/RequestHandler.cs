using System.Text;

namespace Ezra;

public class RequestHandler
{
    public void HandleRequest(Stream content, Stream response)
    {
        using var writer = new StreamWriter(
            stream: response,
            encoding: Encoding.UTF8,
            leaveOpen: true
        );
        writer.NewLine = "\r\n";

        try
        {
            var request = new RequestParser(content);
            request.Parse();
            writer.WriteLine("HTTP/1.1 200 OK");
        }
        catch
        {
            writer.WriteLine("HTTP/1.1 400 Bad Request");
        }
    }
}

public class RequestParser
{
    private readonly Stream _content;

    public RequestParser(Stream content)
    {
        _content = content;
    }

    public void Parse()
    {
        using var reader = new StreamReader(_content);
        var startLine = reader.ReadLine();
        var startLineParts = startLine!.Split(' ');
        if (startLineParts.Length != 3)
        {
            throw new ArgumentException("Invalid request");
        }
    }
}
