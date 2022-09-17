using System.Text;

namespace Ezra;

public class Response : IResponse
{
    private readonly StringBuilder _content = new();

    public Response() { }

    public int Code { get; set; }
    public string? Reason { get; set; }

    public void Write(string content)
    {
        _content.Append(content);
    }

    public void Serialize(StreamWriter writer)
    {
        writer.WriteLine($"HTTP/1.1 {Code} {Reason}");
        writer.WriteLine();
        writer.WriteLine(_content.ToString());
    }
}
