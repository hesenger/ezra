using System.IO;
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
        var handler = new RequestProcessor();
        handler.Process(request, response);

        response.Position = 0;
        var reader = new StreamReader(response);
        var responseText = reader.ReadLine();
        Assert.That(responseText, Contains.Substring(expectedResponse));
    }
}
