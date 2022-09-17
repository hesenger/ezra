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
        writer.Write("HTTP/1.1 200 OK\r");
    }
}
