namespace Ezra.Console;

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
