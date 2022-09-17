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
}
