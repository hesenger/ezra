namespace Ezra;

public interface IRequest
{
    string Method { get; }
    string Path { get; }
}
