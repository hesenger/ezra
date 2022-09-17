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
    public static readonly string[] ValidMethods = new string[]
    {
        "GET",
        "HEAD",
        "POST",
        "PUT",
        "DELETE",
        "CONNECT",
        "OPTIONS",
        "TRACE",
    };

    private readonly Stream _content;

    public RequestParser(Stream content)
    {
        _content = content;
    }

    public string? Method { get; private set; }

    public void Parse()
    {
        using var reader = new StreamReader(_content);
        var startLine = reader.ReadLine();
        var startLineParts = startLine!.Split(' ');
        if (startLineParts.Length != 3)
        {
            throw new ArgumentException("Invalid request");
        }

        Method = startLineParts[0];
        if (!ValidMethods.Contains(Method))
        {
            throw new ArgumentException("Invalid method");
        }
    }
}
