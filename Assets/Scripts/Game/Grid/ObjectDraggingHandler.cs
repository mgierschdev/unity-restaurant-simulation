// Handlers object selection and dragging
public static class ObjectDraggingHandler
{
    private static string currentClickedActiveGameObject;
    private static BaseObjectController previewGameGridObject;

    public static void Init()
    {
        isDraggingEnabled = false;
        currentClickedActiveGameObject = "";
    }

    private static GameGridObject GetActiveGameGridObject()
    {
        GameGridObject gameGridObject = null;
        if (currentClickedActiveGameObject != "")
        {
            if (!BussGrid.GetGameGridObjectsDictionary().ContainsKey(currentClickedActiveGameObject) && previewGameGridObject != null)
            {
                //Meanning the item is on previoud but not inventory
                return previewGameGridObject.GetGameGridObject();
            }
            else
            {
                gameGridObject = BussGrid.GetGameGridObjectsDictionary()[currentClickedActiveGameObject];
            }

        }
        return gameGridObject;
    }

    public static void ClearCurrentClickedActiveGameObject()
    {
        currentClickedActiveGameObject = "";
    }

    // Used to highlight the current object being edited
    public static void SetActiveGameGridObject(GameGridObject obj)
    {
        isDraggingEnabled = true;

        if (currentClickedActiveGameObject != "" && BussGrid.GetGameGridObjectsDictionary().ContainsKey(currentClickedActiveGameObject))
        {
            GameGridObject gameGridObject = BussGrid.GetGameGridObjectsDictionary()[currentClickedActiveGameObject];
            gameGridObject.HideEditMenu();
        }

        currentClickedActiveGameObject = obj.Name;
        obj.ShowEditMenu();
    }

    public static void HideHighlightedGridBussFloor()
    {
        if (currentClickedActiveGameObject != "" && BussGrid.GetGameGridObjectsDictionary().ContainsKey(currentClickedActiveGameObject))
        {
            GameGridObject gameGridObject = BussGrid.GetGameGridObjectsDictionary()[currentClickedActiveGameObject];
            gameGridObject.HideEditMenu();
            currentClickedActiveGameObject = "";
        }
    }

    //Is dragging mode enabled and object selected?
    private static bool isDraggingEnabled; // Is any object being dragged

    public static bool IsDraggingEnabled(GameGridObject obj)
    {
        return isDraggingEnabled && IsThisSelectedObject(obj.Name);
    }
    // Is any object being dragged
    public static void SetIsDraggingEnable(bool val)
    {
        isDraggingEnabled = val;
    }

    public static bool GetIsDraggingEnabled()
    {
        return isDraggingEnabled;
    }

    public static bool IsThisSelectedObject(string objName)
    {
        return currentClickedActiveGameObject == objName;
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

        previewGameGridObject = null;
    }

    public static void SetPreviewItem(BaseObjectController obj)
    {
        previewGameGridObject = obj;
    }
}