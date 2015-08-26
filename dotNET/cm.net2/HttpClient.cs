using System.Net;
using System.Text;
using cm.data.common;
using cm.data.interfaces;
using cm.data.models;

namespace cm.net2
{
    public class HttpClient : IHttpClient
    {
        private string _token;
        private readonly ISerializer _serializer;
        private AccessTokenRequest _userCredentials;

        public HttpClient()
        {
            _serializer = new Serializer();
        }

        public void SetCredentials(string accessToken, string clientId, string secret)
        {
            _userCredentials = new AccessTokenRequest
            {
                grant_type = Constants.DefaultGrantType,
                client_id = clientId,
                client_secret = secret,
                refresh_token = accessToken
            };

            _token = null;
        }

        public string Send(HttpMethod method, string url, string data)
        {
            if (string.IsNullOrEmpty(_token))
            {
                _token = GetNewToken();
            }

            using (var client = new WebClient())
            {
                var authHeader = string.Format("{0} {1}", Constants.OAuthTokenPrefix, _token);

                client.Encoding = Encoding.UTF8;
                client.Headers.Add("Content-Type", "application/xml");
                client.Headers.Add(Constants.WebHeaderKeyAuthorization, authHeader);

                var response = new byte[] { };

                switch (method)
                {
                    case HttpMethod.GET:
                        response = client.DownloadData(url);
                        break;
                    case HttpMethod.POST:
                        response = client.UploadData(url, method.ToString(), Encoding.UTF8.GetBytes(data));
                        break;
                }

                // refresh token
                if (client.ResponseHeaders != null && client.ResponseHeaders[Constants.WebHeaderKeyAuthorization] != null)
                {
                    _token = client.ResponseHeaders[Constants.WebHeaderKeyAuthorization];
                }

                return Encoding.UTF8.GetString(response);
            }
        }

        private string GetNewToken()
        {
            if (_userCredentials == null) throw new WebException("You must set credentials first.");

            var data = _serializer.Serialize(_userCredentials);

            using (var client = new WebClient())
            {
                var authHeader = string.Format("{0} {1}", Constants.OAuthTokenPrefix, _token);

                client.Encoding = Encoding.UTF8;
                client.Headers.Add("Content-Type", "application/xml");
                client.Headers.Add(Constants.WebHeaderKeyAuthorization, authHeader);

                var response = client.UploadData(Constants.UrlNewToken, HttpMethod.POST.ToString(), Encoding.UTF8.GetBytes(data));

                // refresh token
                if (client.ResponseHeaders != null && client.ResponseHeaders[Constants.WebHeaderKeyAuthorization] != null)
                {
                    _token = client.ResponseHeaders[Constants.WebHeaderKeyAuthorization];
                }

                var xml = Encoding.UTF8.GetString(response);
                var result = _serializer.Deserialize<AccessTokenResponse>(xml);

                return result.access_token;
            }
        }
    }
}
