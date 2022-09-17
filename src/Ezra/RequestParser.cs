using System.Text;

namespace Ezra;

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
    public string? HttpVersion { get; private set; }

    public void Parse()
    {
        using var reader = new StreamReader(
            _content,
            encoding: Encoding.UTF8,
            leaveOpen: true
        );
        string[] startLineParts = ParseStartLine(reader);
        ParseMethod(startLineParts);
        ParseHttpVersion(startLineParts);
    }

    private void ParseHttpVersion(string[] startLineParts)
    {
        HttpVersion = startLineParts[2];
        if (HttpVersion != "HTTP/1.1")
        {
            throw new ArgumentException("Invalid HTTP version");
        }
    }

    private void ParseMethod(string[] startLineParts)
    {
        Method = startLineParts[0];
        if (!ValidMethods.Contains(Method))
        {
            throw new ArgumentException("Invalid method");
        }
    }

    private string[] ParseStartLine(StreamReader reader)
    {
        var startLine = reader.ReadLine();
        var startLineParts = startLine!.Split(' ');
        if (startLineParts.Length != 3)
        {
            throw new ArgumentException("Invalid request");
        }

        return startLineParts;
    }
}
