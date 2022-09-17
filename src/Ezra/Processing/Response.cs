using System.Text;

namespace Ezra.Processing;

public class Response : IResponse
{
    private readonly StringBuilder _content = new();

    public int Code { get; set; } = 200;
    public string? Reason { get; set; } = "OK";

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
