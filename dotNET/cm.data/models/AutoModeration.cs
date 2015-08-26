using System;
using System.Xml.Serialization;
using cm.data.common;

namespace cm.data.models
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

    [XmlRoot("validationResponse")]
    public class ValidationResultsResponse : BaseResponse
    {
        public ValidationResults data { get; set; }
    }

    public class ValidationResults
    {
        public string id { get; set; }
        public string trackingId { get; set; }
        public string config { get; set; }
        [XmlElement("results")]
        public ValidationResult[] results { get; set; }

        public bool IsInvalidHtml
        {
            get
            {
                foreach (var res in results)
                {
                    if (res.vcType == PolicyType.html && res.result == PolicyResult.fail && res.resultMessage == "invalid_html")
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public PolicyResult GetGeneralResult(PolicyType type = PolicyType.unknown)
        {
            foreach (var res in results)
            {
                var isTypeMatched = type == PolicyType.unknown || res.vcType == type;
                if (isTypeMatched && res.result == PolicyResult.fail)
                {
                    return PolicyResult.fail;
                }
            }

            return PolicyResult.success;
        }
    }

    public class ValidationResult
    {
        public PolicyType vcType { get; set; }

        public Guid? vcid { get; set; }
        public Guid? listId { get; set; }

        public string listName { get; set; }
        [XmlElement("entry")]
        public string[] entry { get; set; }

        public PolicyResult result { get; set; }
        public string resultMessage { get; set; }
    }
}
