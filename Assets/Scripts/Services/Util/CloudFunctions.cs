namespace Services.Util
{
    /**
     * Problem: Centralize Cloud Code function names.
     * Goal: Avoid hard-coded function names across the codebase.
     * Approach: Expose static string identifiers.
     * Time: O(1) per access.
     * Space: O(1).
     */
    public static class CloudFunctions
    {
        // The value has to be the same as the cloud function name
        public static string CloudCodeGetPlayerData = "CloudCodeGetPlayerData";
    }
}
