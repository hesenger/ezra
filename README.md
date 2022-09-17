# Ezra

Ezra is a HTTP web server build based on orinal protocol specification,
using TDD from the scratch. It was built completely using managed C# code.

Ezra is a fully recreational project made for learning and not intended
to be used in production.

## Usage

```csharp
using Ezra;

public static class Program
{
    public static void Main(string[] args)
    {
        new EzraServer()
            .MapHandler("/hello", new EchoHandler("hello"))
            .MapHandler("/ok", new EchoHandler("ok"))
            .Start(args);
    }
}

public class EchoHandler : IRequestHandler
{
    private readonly string _message;

    public EchoHandler(string message)
    {
        _message = message;
    }

    public void Handle(IRequest request, IResponse response)
    {
        System.Console.WriteLine($"{request.Method} {request.Path}");
        response.Write(_message);
    }
}
```

## Development

The project was developed using NET 6 and VSCode in a Mac M1, so
running it in a Windows/Visual Studio environment might require
some tweaks.

## Resources

- HTTP resources - https://developer.mozilla.org/en-US/docs/Web/HTTP/Resources_and_specifications
- Message format (line ending, sections etc) - https://datatracker.ietf.org/doc/html/rfc7230#section-3
