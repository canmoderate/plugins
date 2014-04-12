
namespace CanModerateNet.Data
{
    public enum PolicyType
    {
        unknown,
        ip,
        text,
        html,
        email
    }

    public enum PolicyResult
    {
        unknown,
        success,
        fail
    }
}
