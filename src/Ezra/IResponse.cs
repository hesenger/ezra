namespace Ezra;

public interface IResponse
{
    int Code { get; set; }
    string? Reason { get; set; }
    void Write(string content);
}
