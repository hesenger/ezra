using System.IO;
using NUnit.Framework;

namespace Ezra.Tests;

public class RequestHandlerTests
{
    private static Stream CreateStream(string content)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    [Test]
    public void ShouldReturn200OK()
    {
        var request = CreateStream("GET / HTTP/1.1\r\n");

        var response = new MemoryStream();
        var handler = new RequestHandler();
        handler.HandleRequest(request, response);

        response.Position = 0;
        var reader = new StreamReader(response);
        var responseText = reader.ReadToEnd();
        Assert.AreEqual("HTTP/1.1 200 OK\r\n", responseText);
    }

    [Test]
    public void ShouldReturn400WhenRequestIsInvalid()
    {
        var request = CreateStream("Invalid content");

        var response = new MemoryStream();
        var handler = new RequestHandler();
        handler.HandleRequest(request, response);

        response.Position = 0;
        var reader = new StreamReader(response);
        var responseText = reader.ReadToEnd();
        Assert.AreEqual("HTTP/1.1 400 Bad Request\r\n", responseText);
    }
}
