using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using CanModerateNet.Data;

namespace CanModerateNet4
{
    public class CanModerateService : ICanModerateService
    {
        public const int MaxRetryCount = 3;
        public const string OAuthTokenPrefix = "Bearer";
        public const string WebHeaderKeyAuthorization = "Authorization";
        public const string DefaultGrantType = "refresh_token";
        public const string UrlNewToken = "https://api.canmoderate.com/api/oauth/token";
        public const string UrlVerifyContent = "https://api.canmoderate.com/api/message/validate";

        private string _accessToken;
        private AccessTokenRequest UserCredentials { get; set; }
        private int RetryCount { get; set; }
        private JavaScriptSerializer JsonSerializer { get; set; }

        public CanModerateService()
        {
            JsonSerializer = new JavaScriptSerializer();
        }

        /// <summary>
        /// Checks content according to configured validation policies
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="clientId"></param>
        /// <param name="secret"></param>
        public CanModerateService(string accessToken, string clientId, string secret)
        {
            UserCredentials = new AccessTokenRequest
            {
                grant_type = DefaultGrantType,
                client_id = clientId,
                client_secret = secret,
                refresh_token = accessToken
            };

            JsonSerializer = new JavaScriptSerializer();
        }

        public void SetUserCredentials(string accessToken, string clientId, string secret)
        {
            UserCredentials = new AccessTokenRequest
            {
                grant_type = DefaultGrantType,
                client_id = clientId,
                client_secret = secret,
                refresh_token = accessToken
            };

            AccessToken = null;
        }

        /// <summary>
        /// Verify content according to configured policies
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Detailed results for each validation policy</returns>
        public ValidationResults CheckContent(ModerationContent content)
        {
            content.message = content.message ?? string.Empty;

            using (var client = new WebClient())
            {
                try
                {
                    var data = JsonSerializer.Serialize(content);

                    var authHeader = string.Format("{0} {1}", OAuthTokenPrefix, AccessToken);

                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add("Content-Type", "application/json");
                    client.Headers.Add(WebHeaderKeyAuthorization, authHeader);

                    var response = client.UploadData(UrlVerifyContent, "POST", Encoding.UTF8.GetBytes(data));

                    // refresh token
                    if (client.ResponseHeaders.AllKeys.Contains(WebHeaderKeyAuthorization))
                    {
                        AccessToken = client.ResponseHeaders[WebHeaderKeyAuthorization];
                    }

                    var json = Encoding.UTF8.GetString(response);
                    var result = JsonSerializer.Deserialize<ValidationResults>(json);

                    RetryCount = 0;

                    return result;
                }
                catch (WebException webEx)
                {
                    if (webEx.Response != null 
                        && ((HttpWebResponse)webEx.Response).StatusCode == HttpStatusCode.Forbidden 
                        && RetryCount + 1 <= MaxRetryCount)
                    {
                        AccessToken = null;
                        RetryCount++;

                        return CheckContent(content);
                    }

                    throw;
                }
            }
        }

        /// <summary>
        /// Current access token required for server communication
        /// </summary>
        private string AccessToken
        {
            get
            {
                if (string.IsNullOrEmpty(_accessToken))
                {
                    NewToken();
                }

                return _accessToken;
            }
            set { _accessToken = value; }
        }

        /// <summary>
        /// Updates current access token based on user credentials
        /// </summary>
        private void NewToken()
        {
            using (var client = new WebClient())
            {
                try
                {
                    var data = JsonSerializer.Serialize(UserCredentials);

                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add("Content-Type", "application/json");

                    var response = client.UploadData(UrlNewToken, "POST", Encoding.UTF8.GetBytes(data));
                    var json = Encoding.UTF8.GetString(response);

                    var result = JsonSerializer.Deserialize<AccessTokenResponse>(json);

                    AccessToken = result.access_token;
                }
                catch (Exception)
                {
                    AccessToken = null;
                }
            }
        }
    }
}
