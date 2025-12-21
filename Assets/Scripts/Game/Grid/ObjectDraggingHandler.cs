// Handlers object selection and dragging

using Game.Controllers.Grid_Objects_Controllers;

namespace Game.Grid
{
    /**
     * Problem: Manage selection and dragging state for grid objects.
     * Goal: Track the active object and toggle edit/preview behavior.
     * Approach: Store selected object IDs and update object state helpers.
     * Time: O(1) per operation.
     * Space: O(1).
     */
    public static class ObjectDraggingHandler
    {
        private static string _currentClickedActiveGameObject;
        private static BaseObjectController _previewGameGridObject;

        public static void Init()
        {
            _isDraggingEnabled = false;
            _currentClickedActiveGameObject = "";
        }

        private static GameGridObject GetActiveGameGridObject()
        {
            GameGridObject gameGridObject = null;
            if (_currentClickedActiveGameObject != "")
            {
                if (!BussGrid.GetGameGridObjectsDictionary().ContainsKey(_currentClickedActiveGameObject) &&
                    _previewGameGridObject != null)
                {
                    //Meanning the item is on preview but not in inventory
                    return _previewGameGridObject.GetGameGridObject();
                }
                else
                {
                    gameGridObject = BussGrid.GetGameGridObjectsDictionary()[_currentClickedActiveGameObject];
                }
            }

            return gameGridObject;
        }

        public static void ClearCurrentClickedActiveGameObject()
        {
            _currentClickedActiveGameObject = "";
        }

        // Used to highlight the current object being edited
        public static void SetActiveGameGridObject(GameGridObject obj)
        {
            _isDraggingEnabled = true;

            if (_currentClickedActiveGameObject != "" &&
                BussGrid.GetGameGridObjectsDictionary().ContainsKey(_currentClickedActiveGameObject))
            {
                GameGridObject gameGridObject =
                    BussGrid.GetGameGridObjectsDictionary()[_currentClickedActiveGameObject];
                gameGridObject.HideEditMenu();
            }

            // we highlight the floor for the object
            obj.ShowAvailableUnderTiles();
            _currentClickedActiveGameObject = obj.Name;
            obj.ShowEditMenu();
        }

        public static void HideHighlightedGridBussFloor()
        {
            if (_currentClickedActiveGameObject != "" &&
                BussGrid.GetGameGridObjectsDictionary().ContainsKey(_currentClickedActiveGameObject))
            {
                GameGridObject gameGridObject =
                    BussGrid.GetGameGridObjectsDictionary()[_currentClickedActiveGameObject];
                gameGridObject.HideEditMenu();
                _currentClickedActiveGameObject = "";
            }
        }

        //Is dragging mode enabled and object selected?
        private static bool _isDraggingEnabled; // Is any object being dragged

        public static bool IsDraggingEnabled(GameGridObject obj)
        {
            return _isDraggingEnabled && IsThisSelectedObject(obj.Name);
        }

        // Is any object being dragged
        public static void SetIsDraggingEnable(bool val)
        {
            _isDraggingEnabled = val;
        }

        public static bool GetIsDraggingEnabled()
        {
            return _isDraggingEnabled;
        }

        public static bool IsThisSelectedObject(string objName)
        {
            return _currentClickedActiveGameObject == objName;
        }

        public static void DisableDragging()
        {
            GameGridObject obj = GetActiveGameGridObject();

            if (obj == null)
            {
                return;
            }

            // Handling preview items
            if (!obj.GetIsItemBought())
            {
                obj.CancelPurchase();
            }
            else
            {
                obj.SetInactive();
            }

            _previewGameGridObject = null;
        }

        public static void SetPreviewItem(BaseObjectController obj)
        {
            _previewGameGridObject = obj;
        }
    }
}
