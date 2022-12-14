using System;
using System.IO;
using Ezra.Processing;
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

    public static readonly string[][] MethodCases = new[]
    {
        new[] { "GET", "404 Not Found" },
        new[] { "INVALID-METHOD", "400 Bad Request" },
    };

    [Test]
    [TestCaseSource(nameof(MethodCases))]
    public void ShouldValidateMethod(string method, string expectedResponse)
    {
        var request = CreateStream($"{method} / HTTP/1.1");

        var response = new MemoryStream();
        var processor = new RequestProcessor(new());
        processor.Process(request, response);

        response.Position = 0;
        var reader = new StreamReader(response);
        var responseText = reader.ReadLine();
        Assert.That(responseText, Contains.Substring(expectedResponse));
    }

    [Test]
    public void ShouldDelegateRequestMappedHandler()
    {
        var handlerSpy = new Mock<IRequestHandler>();
        handlerSpy
            .Setup(h => h.Handle(It.IsAny<IRequest>(), It.IsAny<IResponse>()))
            .Callback<IRequest, IResponse>(
                (request, response) =>
                {
                    response.Code = 200;
                    response.Reason = "OK";
                    response.Write("Hello World");
                }
            );

        var request = CreateStream($"GET /test HTTP/1.1");
        var response = new MemoryStream();
        var processor = new RequestProcessor(
            new() { { "/test", handlerSpy.Object } }
        );
        processor.Process(request, response);

        handlerSpy.Verify(
            h => h.Handle(It.IsAny<IRequest>(), It.IsAny<IResponse>()),
            Times.Once
        );

        response.Position = 0;
        var reader = new StreamReader(response);
        var responseText = reader.ReadLine();
        var body = reader.ReadToEnd();
        Assert.That(responseText, Contains.Substring("200 OK"));
        Assert.That(body.Trim(), Is.EqualTo("Hello World"));
    }

    [Test]
    public void ShouldCatchErrorFromHandler()
    {
        var handlerSpy = new Mock<IRequestHandler>();
        handlerSpy
            .Setup(h => h.Handle(It.IsAny<IRequest>(), It.IsAny<IResponse>()))
            .Callback<IRequest, IResponse>(
                (request, response) =>
                    throw new InvalidOperationException("Something went wrong")
            );

        var request = CreateStream($"GET /test HTTP/1.1");
        var response = new MemoryStream();
        var processor = new RequestProcessor(
            new() { { "/test", handlerSpy.Object } }
        );
        processor.Process(request, response);

        handlerSpy.Verify(
            h => h.Handle(It.IsAny<IRequest>(), It.IsAny<IResponse>()),
            Times.Once
        );

        response.Position = 0;
        var reader = new StreamReader(response);
        var responseText = reader.ReadLine();
        Assert.That(
            responseText,
            Contains.Substring("500 Internal Server Error")
        );
    }
}
