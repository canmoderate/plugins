using cm.data.common;

namespace cm.data.interfaces
{
    public interface IHttpClient
    {
        void SetCredentials(string accessToken, string clientId, string secret);
        string Send(HttpMethod method, string url, string data = null);
    }
}