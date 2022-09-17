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

    public static readonly string[][] GetMethodCases = new[]
    {
        new[] { "GET", "200 OK" },
        new[] { "HEAD", "200 OK" },
        new[] { "POST", "200 OK" },
        new[] { "PUT", "200 OK" },
        new[] { "DELETE", "200 OK" },
        new[] { "CONNECT", "200 OK" },
        new[] { "OPTIONS", "200 OK" },
        new[] { "TRACE", "200 OK" },
        new[] { "LOL", "400 Bad Request" },
        new[] { "4HEAD", "400 Bad Request" },
    };

    [Test]
    [TestCaseSource(nameof(GetMethodCases))]
    public void ShouldReturn200ValidMethodsOnly(
        string method,
        string expectedResponse
    )
    {
        var request = CreateStream($"{method} / HTTP/1.1");

        var response = new MemoryStream();
        var handler = new RequestHandler();
        handler.HandleRequest(request, response);

        response.Position = 0;
        var reader = new StreamReader(response);
        var responseText = reader.ReadToEnd();
        Assert.That(responseText, Contains.Substring(expectedResponse));
    }

    [Test]
    public void ShouldReturn400UnsupportedHttp()
    {
        var request = CreateStream("GET / HTTP/1.2");

        var response = new MemoryStream();
        var handler = new RequestHandler();
        handler.HandleRequest(request, response);

        response.Position = 0;
        var reader = new StreamReader(response);
        var responseText = reader.ReadToEnd();
        Assert.AreEqual("HTTP/1.1 400 Bad Request\r\n", responseText);
    }
}
