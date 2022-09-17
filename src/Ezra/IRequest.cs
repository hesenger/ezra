namespace Ezra;

public interface IRequest
{
    string Method { get; }
    string Path { get; }
    IDictionary<string, string> Headers { get; }
}
