using System.Collections.Generic;
using Unity.Services.Analytics;

namespace Services
{
    /**
     * Problem: Send analytics events to Unity Analytics.
     * Goal: Provide a single publish entrypoint for custom events.
     * Approach: Call AnalyticsService.CustomData and Flush.
     * Time: O(1) per call plus network time.
     * Space: O(1).
     */
    public static class UnityAnalytics
    {
        public static void PublishEvent(string eventName, Dictionary<string, object> dictionary)
        {
            AnalyticsService.Instance.CustomData(eventName, dictionary);
            AnalyticsService.Instance.Flush();
        }
    }
}
