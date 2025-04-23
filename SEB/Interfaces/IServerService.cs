using SEB.Http;

namespace SEB.Interfaces;

public interface IServerService
{
    public void ParseRequest(StreamReader reader, Request request);
    public void ParseHeaders(StreamReader reader, Request request);
    public void ParseBody(StreamReader reader, Request request);
}