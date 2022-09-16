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
    private ListView storeItems;
    private VisualElement storeListContainer;

    void Awake()
    {
        // Grid Controller
        GameObject gridObj = GameObject.Find(Settings.GameGrid).gameObject;
        gridController = gridObj.GetComponent<GridController>();

        // UI elements
        gameUI = GetComponent<UIDocument>();
        rootVisualElement = gameUI.rootVisualElement;
        bottomLeftPanelList = new List<Button>();
        // VisualTreeAsset visualTree = rootVisualElement.visualTreeAssetSource;
        //TemplateContainer templateContainer = visualTree.CloneTree();
        Button storeButton = rootVisualElement.Q<Button>("StoreButton");
        Button employeeButton = rootVisualElement.Q<Button>("EmployeeButton");
        Button editButton = rootVisualElement.Q<Button>("EditButton");
        exitEditModeButton = rootVisualElement.Q<Button>("ExitEditModeButton");
        storeItems = rootVisualElement.Q<ListView>("StoreListView");
        storeListContainer = rootVisualElement.Q<VisualElement>("StoreListContainer");

        storeButton.RegisterCallback<ClickEvent>(OpenStoreWindow);
        employeeButton.RegisterCallback<ClickEvent>(SetButtonBehaviour);
        editButton.RegisterCallback<ClickEvent>(OpenEditPanel);
        exitEditModeButton.RegisterCallback<ClickEvent>(CloseEditPanel);

        bottomLeftPanelList.Add(storeButton);
        bottomLeftPanelList.Add(employeeButton);
        bottomLeftPanelList.Add(editButton);
        exitEditModeButton.visible = false;

        PopulateStoreListView();
        storeListContainer.visible = false;
    }

    private void OpenStoreWindow(ClickEvent evt)
    {
        storeListContainer.visible = true;
        gridController.DraggingObject = true;// Blocks perspective hand
    }

    private void PopulateStoreListView()
    {

        Debug.Log(StoreListEntry.name);

        storeItems.makeItem = () =>
        {
            // Instantiate the UXML template for the entry
            var newListEntry = StoreListEntry.Instantiate();
            string name = "test";
            newListEntry.userData = name;

            return newListEntry;
        };

        storeItems.bindItem = (item, index) =>
        {
            (item.userData as string).GetType();
        };

        List<string> list = new List<string>();
        list.Add("1");
        list.Add("2");
        list.Add("3");
        list.Add("4");
        storeItems.itemsSource = list;
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
        Debug.Log("ExitEdit button "+exitEditModeButton);
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
