namespace Ezra;

public interface IRequestHandler
{
    public void Handle(IRequest request, Stream response);
}
