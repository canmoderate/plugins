using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CanModerateNet.Data;

namespace CanModerateNet2
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

        public CanModerateService()
        {

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
                    var data = SerializeXml(content);

                    var authHeader = string.Format("{0} {1}", OAuthTokenPrefix, AccessToken);

                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add("Content-Type", "application/xml");
                    client.Headers.Add(WebHeaderKeyAuthorization, authHeader);

                    var response = client.UploadData(UrlVerifyContent, "POST", Encoding.UTF8.GetBytes(data));

                    // refresh token
                    RefreshToken(client.ResponseHeaders);

                    var xml = Encoding.UTF8.GetString(response);
                    var result = DeserializeXml<ValidationResults>(xml);

                    RetryCount = 0;

                    return result;
                }
                catch (WebException webEx)
                {
                    if (webEx.Response != null 
                        && ((HttpWebResponse)webEx.Response).StatusCode == HttpStatusCode.Forbidden 
                        && RetryCount + 1 <= MaxRetryCount)
                    {
                        AccessToken = null; // reset invalid token
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
                    var data = SerializeXml(UserCredentials);

                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add("Content-Type", "application/xml");

                    var response = client.UploadData(UrlNewToken, "POST", Encoding.UTF8.GetBytes(data));
                    var xml = Encoding.UTF8.GetString(response);

                    var result = DeserializeXml<AccessTokenResponse>(xml);

                    AccessToken = result.access_token;
                }
                catch (Exception)
                {
                    AccessToken = null;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private static string SerializeXml<T>(T value)
        {
            var serializer = new XmlSerializer(typeof(T));
            string xml;
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true
            };

            using (var textWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }
                xml = textWriter.ToString(); 
            }
            return xml;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private static T DeserializeXml<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return default(T);
            }

            var serializer = new XmlSerializer(typeof(T));
            var settings = new XmlReaderSettings();
            T obj;

            using (var textReader = new StringReader(xml))
            {
                using (var xmlReader = XmlReader.Create(textReader, settings))
                {
                    obj = (T)serializer.Deserialize(xmlReader);
                }
            }
            return obj;
        }

        private void RefreshToken(WebHeaderCollection webHeaders)
        {
            if (webHeaders != null && webHeaders[WebHeaderKeyAuthorization] != null)
            {
                AccessToken = webHeaders[WebHeaderKeyAuthorization];
            }
        }
    }
}
