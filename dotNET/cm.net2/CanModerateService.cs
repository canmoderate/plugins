using System;
using System.Collections.Generic;
using System.Net;
using cm.data.common;
using cm.data.interfaces;
using cm.data.models;

namespace cm.net2
{
    public class CanModerateService : ICanModerateService
    {
        private readonly IHttpClient _httpClient;
        private readonly ISerializer _serializer;

        public CanModerateService()
        {
            _httpClient = new HttpClient();
            _serializer = new Serializer();
        }

        public void SetCredentials(string accessToken, string clientId, string secret)
        {
            _httpClient.SetCredentials(accessToken, clientId, secret);
        }

        public ValidationResults CheckContent(ModerationContent content)
        {
            content.message = content.message ?? string.Empty;

            var data = _serializer.Serialize(content);
            var json = _httpClient.Send(HttpMethod.POST, Constants.UrlVerifyContent, data);
            var result = _serializer.Deserialize<ValidationResultsResponse>(json);

            if (result.data == null || !string.IsNullOrEmpty(result.error))
            {
                throw new WebException(result.error);
            }

            return result.data;
        }

        public List<SiteMessageHistoryData> GetPostModerationResults(DateTime dateFrom, int pageSize = 1000)
        {
            var url = string.Format(Constants.UrlHumanModerationQueue, dateFrom.ToString("o"), pageSize);
            var json = _httpClient.Send(HttpMethod.GET, url);
            var result = _serializer.Deserialize<SiteMessageHistoryDataResponse>(json);

            if (result.data == null || !string.IsNullOrEmpty(result.error))
            {
                throw new WebException(result.error);
            }

            var list = new List<SiteMessageHistoryData>();
            foreach (var res in result.data)
            {
                var history = res.history[res.history.Count - 1];
                list.Add(new SiteMessageHistoryData()
                {
                    id = res.id,
                    userId = res.userId,
                    trackingId = res.trackingId,
                    created = history.created,
                    result = history.result,
                    createdBy = history.createdByUserId,
                });
            }

            return list;
        }
    }
}
