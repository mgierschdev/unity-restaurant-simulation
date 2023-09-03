using System.Collections.Generic;
using Unity.Services.Analytics;

namespace Services
{
    public static class UnityAnalytics
    {
        public static void PublishEvent(string eventName, Dictionary<string, object> dictionary)
        {
            AnalyticsService.Instance.CustomData(eventName, dictionary);
            AnalyticsService.Instance.Flush();
        }
    }
}