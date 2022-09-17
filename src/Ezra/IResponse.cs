namespace Ezra;

public interface IResponse
{
    int Code { get; set; }
    string? Reason { get; set; }
    void Write(string content);
}

public class Response : IResponse
{
    public Response(Stream stream) { }

    public int Code { get; set; }
    public string? Reason { get; set; }

    public void Write(string content) { }
}
