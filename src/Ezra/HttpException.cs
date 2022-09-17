namespace Ezra;

public class HttpException : Exception
{
    public HttpException(int code, string reason, string message)
        : base(message)
    {
        Code = code;
        Reason = reason;
    }

    public int Code { get; }
    public string Reason { get; }
}
