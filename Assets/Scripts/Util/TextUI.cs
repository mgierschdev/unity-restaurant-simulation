// To be used for localization i8int
namespace Util
{
    /**
     * Problem: Provide UI text constants for labels and messages.
     * Goal: Centralize UI strings for reuse and localization.
     * Approach: Expose strings as constants and static fields.
     * Time: O(1) per access.
     * Space: O(1).
     */
    public static class TextUI
    {
        public const string Price = "Price";

        public const string Amount = "Stored";

        public const string CurrentLevel = "Level";

        public const string Store = "Store";

        public const string Upgrade = "Upgrade";

        public const string Storage = "Storage";

        public const string Settings = "Settings";

        public static string Tutorial = "Tutorial";

        public const string Max = "Max";

        public const string ConnectionProblem = "Could not connect.";
    }
}
