using System.Xml.Serialization;

namespace CanModerateNet.Data
{
    [XmlRoot("refreshToken")]
    public class AccessTokenRequest
    {
        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string refresh_token { get; set; }
    }

    [XmlRoot("oauth")]
    public class AccessTokenResponse
    {
        public string access_token { get; set; }
    }
}
