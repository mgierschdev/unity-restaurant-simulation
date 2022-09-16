using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    private GridController gridController;
    private UIDocument gameUI;
    private VisualElement rootVisualElement;
    private List<Button> bottomLeftPanelList;
    private Button exitEditModeButton;

    //Store List attributes
    [SerializeField]
    public VisualTreeAsset StoreListEntry;
    private ListView listView;
    private VisualElement centerContainer;
    private VisualElement downPanelContainer;

    // Drag effect with mouse
    private Vector3 initPointerDownPosition;

    void Awake()
    {
        // Grid Controller
        GameObject gridObj = GameObject.Find(Settings.GameGrid).gameObject;
        gridController = gridObj.GetComponent<GridController>();

        // UI elements
        gameUI = GetComponent<UIDocument>();
        rootVisualElement = gameUI.rootVisualElement;
        bottomLeftPanelList = new List<Button>();

        // Querying elements from the UI tree xmlu
        Button storeButton = rootVisualElement.Q<Button>("StoreButton");
        Button employeeButton = rootVisualElement.Q<Button>("EmployeeButton");
        Button editButton = rootVisualElement.Q<Button>("EditButton");
        Button closeCenterPanelButton = rootVisualElement.Q<Button>("CloseCenterPanelButton");
        exitEditModeButton = rootVisualElement.Q<Button>("ExitEditModeButton");
        listView = rootVisualElement.Q<ListView>("ListView");
        centerContainer = rootVisualElement.Q<VisualElement>("Center");
        downPanelContainer = rootVisualElement.Q<VisualElement>("Down");

        storeButton.RegisterCallback<ClickEvent>(OpenStoreWindow);
        employeeButton.RegisterCallback<ClickEvent>(SetButtonBehaviour);
        editButton.RegisterCallback<ClickEvent>(OpenEditPanel);
        exitEditModeButton.RegisterCallback<ClickEvent>(CloseEditPanel);
        closeCenterPanelButton.RegisterCallback<ClickEvent>(CloseCenterPanel);

        //Adding drag with mouse
        listView.RegisterCallback<PointerDownEvent>(ListViewRegisterDownEvent);
        listView.RegisterCallback<PointerMoveEvent>(ListViewMoveEvent);
        listView.RegisterCallback<PointerUpEvent>(ListViewPointerUpEvent);
        //Adding drag with mouse

        bottomLeftPanelList.Add(storeButton);
        bottomLeftPanelList.Add(employeeButton);
        bottomLeftPanelList.Add(editButton);
        exitEditModeButton.visible = false;

        PopulateStoreListView();
        centerContainer.visible = false;
        initPointerDownPosition = Vector3.positiveInfinity;
    }

    private void ListViewPointerUpEvent(PointerUpEvent pointer)
    {
        initPointerDownPosition = Vector3.positiveInfinity;
    }

    private void ListViewMoveEvent(PointerMoveEvent pointer)
    {
        if (initPointerDownPosition == Vector3.positiveInfinity)
        {
            return;
        }
        
    }

    private void ListViewRegisterDownEvent(PointerDownEvent pointer)
    {
        initPointerDownPosition = pointer.position;
    }

    private void CloseCenterPanel(ClickEvent ent)
    {
        centerContainer.visible = false;
        downPanelContainer.visible = true;
        gridController.DraggingObject = false;// Enables perspective hand
    }

    private void OpenStoreWindow(ClickEvent evt)
    {
        centerContainer.visible = true;
        gridController.DraggingObject = true;// Blocks perspective hand
        downPanelContainer.visible = false;
    }

    private void PopulateStoreListView()
    {
        listView.makeItem = () =>
        {
            // Instantiate the UXML template for the entry
            var newListEntry = StoreListEntry.Instantiate();
            string name = "test";
            newListEntry.userData = name;

            return newListEntry;
        };

        listView.bindItem = (item, index) =>
        {
            (item.userData as string).GetType();
        };

        List<string> list = new List<string>();

        for (int i = 0; i < 50; i++)
        {
            list.Add(i.ToString());
        }

        listView.itemsSource = list;
    }

    private void SetButtonBehaviour(ClickEvent evt)
    {
        Button button = evt.currentTarget as Button;
        GameLog.Log("clicking " + button.name);
    }

    private void OpenEditPanel(ClickEvent evt)
    {
        // we fix the camera in case the player is zoomed
        CloseAllMenus();
        gridController.HighlightGridBussFloor();
        PauseGame();
        exitEditModeButton.visible = true;
        Debug.Log("ExitEdit button " + exitEditModeButton);
    }

    private void CloseEditPanel(ClickEvent evt)
    {
        ShowLeftDownPanel();
        gridController.HideGridBussFloor();
        exitEditModeButton.visible = false;
        ResumeGame();
    }

    private void CloseAllMenus()
    {
        HideLeftDownPanel();
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
    }
    private void HideLeftDownPanel()
    {
        foreach (Button b in bottomLeftPanelList)
        {
            b.visible = false;
        }
    }
    private void ShowLeftDownPanel()
    {
        foreach (Button b in bottomLeftPanelList)
        {
            b.visible = true;
        }
    }

    // The only contract to with the edit panel logic
    public bool IsEditPanelOpen()
    {
        if (!gridController)
        {
            return false;
        }

        return exitEditModeButton.visible;
    }
}
