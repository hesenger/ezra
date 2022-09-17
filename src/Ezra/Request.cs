namespace Ezra;

public class Request : IRequest
{
    public Request(string method, string path)
    {
        Method = method;
        Path = path;
    }

    public string Method { get; }
    public string Path { get; }
}
