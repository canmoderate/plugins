
namespace CanModerateNet.Data
{
    public interface ICanModerateService
    {
        void SetUserCredentials(string accessToken, string clientId, string secret);
        ValidationResults CheckContent(ModerationContent content);
    }
}
