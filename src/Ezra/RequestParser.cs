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

    public IRequest Parse(Stream content)
    {
        using var reader = new StreamReader(
            content,
            encoding: Encoding.UTF8,
            leaveOpen: true
        );
        string[] startLineParts = ParseStartLine(reader);
        var method = ParseMethod(startLineParts);
        var path = ParsePath(startLineParts);
        return new Request(method, path);
    }

    private string ParsePath(string[] startLineParts)
    {
        return startLineParts[1];
    }

    private string ParseMethod(string[] startLineParts)
    {
        var method = startLineParts[0];
        return ValidMethods.Contains(method)
            ? method
            : throw new HttpException(400, "Bad Request", "Invalid method");
    }

    private string[] ParseStartLine(StreamReader reader)
    {
        var startLine = reader.ReadLine();
        var startLineParts = startLine!.Split(' ');
        return startLineParts.Length == 3
            ? startLineParts
            : throw new HttpException(400, "Bad Request", "Invalid start line");
    }
}
