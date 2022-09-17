using System.IO;
using Moq;
using NUnit.Framework;

namespace Ezra.Tests;

public class RequestProcessorTests
{
    private static Stream CreateStream(params string[] content)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream) { NewLine = "\r\n" };

        foreach (var line in content)
        {
            writer.WriteLine(line);
        }

        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    public static readonly string[][] GetMethodCases = new[]
    {
        new[] { "GET", "404 Not Found" },
        new[] { "INVALID-METHOD", "400 Bad Request" },
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
        var processor = new RequestProcessor();
        processor.Process(request, response);

        response.Position = 0;
        var reader = new StreamReader(response);
        var responseText = reader.ReadLine();
        Assert.That(responseText, Contains.Substring(expectedResponse));
    }

    [Test]
    public void ShouldDelegateRequestMappedHandler()
    {
        var request = CreateStream($"GET /test HTTP/1.1");

        var response = new MemoryStream();
        var processor = new RequestProcessor();

        var handlerSpy = new Mock<IRequestHandler>();
        processor.MapHandler("/test", handlerSpy.Object);
        processor.Process(request, response);

        handlerSpy.Verify(
            h => h.Handle(It.IsAny<IRequest>(), It.IsAny<Stream>()),
            Times.Once
        );
    }
}
