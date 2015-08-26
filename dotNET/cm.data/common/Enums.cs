
namespace cm.data.common
{
    public enum PolicyType
    {
        unknown,
        ip,
        text,
        html,
        email,
        bandwidth
    }

    public enum PolicyResult
    {
        none,
        success,
        fail
    }

    public enum HttpMethod
    {
        GET,
        POST
    }
}
