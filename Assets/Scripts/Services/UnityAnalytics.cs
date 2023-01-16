using System.Collections.Generic;
using Unity.Services.Analytics;

public static class UnityAnalytics
{
    public static void PublishEvent(string eventName, Dictionary<string, object> dictionary)
    {
        AnalyticsService.Instance.CustomData(eventName, dictionary);
        AnalyticsService.Instance.Flush();
    }
}