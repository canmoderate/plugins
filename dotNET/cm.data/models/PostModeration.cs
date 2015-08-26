using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace cm.data.models
{
    [XmlRoot("response")]
    public class SiteMessageHistoryDataResponse : BaseResponse
    {
        [XmlElement("data")]
        public List<SiteMessageHistory> data { get; set; }
    }

    public class SiteMessageHistoryData
    {
        public Guid id { get; set; }
        public string trackingId { get; set; }
        public string userId { get; set; }
        public DateTime created { get; set; }
        public string createdBy { get; set; }
        public List<ValidationResult> result { get; set; }
    } 

    public class SiteMessageHistory
    {
        public Guid id { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        [XmlElement("history")]
        public List<SiteMessageHistoryItem> history { get; set; }
        public string lastModifiedBy { get; set; }
        public string trackingId { get; set; }
        public string userId { get; set; }

    }

    public class SiteMessageHistoryItem
    {
        public Guid id { get; set; }
        public DateTime created { get; set; }
        public string createdByUserId { get; set; }
        public Guid siteId { get; set; }
        [XmlElement("result")]
        public List<ValidationResult> result { get; set; }
    }

}
