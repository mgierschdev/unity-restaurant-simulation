using Util;

namespace Game.UI.Menus
{
    /**
     * Problem: Represent a menu item with tab and type metadata.
     * Goal: Encapsulate menu classification for UI rendering.
     * Approach: Store menu tab/type and expose accessors.
     * Time: O(1) per access.
     * Space: O(1).
     */
    public class MenuItem
    {
        public string Name;

        private readonly MenuType _type;
        private readonly MenuTab _menuTab;

        public MenuItem(MenuTab menuTab, MenuType type, string name)
        {
            this._menuTab = menuTab;
            this.Name = name;
            this._type = type;
        }

        public MenuType GetMenuType()
        {
            return _type;
        }

        public MenuTab GetMenuTab()
        {
            return _menuTab;
        }
    }
}
