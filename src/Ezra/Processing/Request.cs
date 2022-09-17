namespace Ezra.Processing;

public class Request : IRequest
{
    public Request(
        string method,
        string path,
        IDictionary<string, string> headers
    )
    {
        Method = method;
        Path = path;
        Headers = headers;
    }

    public string Method { get; }
    public string Path { get; }
    public IDictionary<string, string> Headers { get; }
}
