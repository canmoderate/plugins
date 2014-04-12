using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CanModerateNet.Data
{
    public struct ValidationResult
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

    [XmlRoot("validationResponse")]
    public struct ValidationResults
    {
        public string id { get; set; }
        public string trackingId { get; set; }
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
}
