namespace Util
{
    /**
     * Problem: Centralize animator state names for info popups.
     * Goal: Avoid hard-coded string literals in animation logic.
     * Approach: Expose state names as constants.
     * Time: O(1) per access.
     * Space: O(1).
     */
    public static class InfoAnimatorState
    {
        public const string Idle = "Idle";
        public const string Moving = "Moving";
    }
}
