using System.IO;
using NUnit.Framework;

namespace Ezra.Tests;

public class RequestHandlerTests
{
    [Test]
    public void ShouldReturnSuccessEmptyResponse()
    {
        var request = new MemoryStream();
        new StreamWriter(request).Write("GET / HTTP/1.1");

        var response = new MemoryStream();
        var handler = new RequestHandler();
        handler.HandleRequest(request, response);

        response.Position = 0;
        var reader = new StreamReader(response);
        var responseText = reader.ReadToEnd();
        Assert.AreEqual("HTTP/1.1 200 OK\r", responseText);
    }
}
