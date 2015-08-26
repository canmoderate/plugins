
namespace cm.data.common
{
    public class Constants
    {
        public const string OAuthTokenPrefix = "Bearer";
        public const string WebHeaderKeyAuthorization = "Authorization";
        public const string DefaultGrantType = "refresh_token";
        public const string UrlNewToken = "https://api.canmoderate.com/api/oauth/token";
        public const string UrlVerifyContent = "https://api.canmoderate.com/api/message/validate";
        public const string UrlHumanModerationQueue = "http://api.canmoderate.com/api/message/history?date={0}&limit={1}";
    }
}
