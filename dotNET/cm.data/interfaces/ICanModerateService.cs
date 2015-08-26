using System;
using System.Collections.Generic;
using cm.data.models;

namespace cm.data.interfaces
{
    public interface ICanModerateService
    {
        void SetCredentials(string accessToken, string clientId, string secret);
        ValidationResults CheckContent(ModerationContent content);
        List<SiteMessageHistoryData> GetPostModerationResults(DateTime dateFrom, int pageSize = 1000);
    }
}
