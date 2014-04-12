using System.Xml.Serialization;

namespace CanModerateNet.Data
{
    [XmlRoot("message")]
    public class ModerationContent
    {
        public string lang { get; set; } // "en-US"
        public string serverIp { get; set; }  // serverIp
        public string serverDomain { get; set; }
        public string ip { get; set; } // client IP
        public string trackingId { get; set; } // post id in user db
        public string userId { get; set; } // user id in user db
        public string message { get; set; } // required
        public string messageFormat { get; set; } // plain-text | html | xhtml 
    }
}
