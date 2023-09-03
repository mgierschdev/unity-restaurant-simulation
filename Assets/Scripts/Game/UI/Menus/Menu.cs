using Util;

namespace Game.UI.Menus
{
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