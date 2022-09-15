using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    private GridController gridController;
    private UIDocument gameUI;
    private VisualElement rootVisualElement;
    private List<Button> bottomLeftPanel;
    private Button exitEditModeButton;
    // Start is called before the first frame update
    void Awake()
    {
        // Grid Controller
        GameObject gridObj = GameObject.Find(Settings.GameGrid).gameObject;
        gridController = gridObj.GetComponent<GridController>();

        // UI elements
        gameUI = GetComponent<UIDocument>();
        rootVisualElement = gameUI.rootVisualElement;
        bottomLeftPanel = new List<Button>();
        // VisualTreeAsset visualTree = rootVisualElement.visualTreeAssetSource;
        //TemplateContainer templateContainer = visualTree.CloneTree();
        Button storeButton = rootVisualElement.Q<Button>("StoreButton");
        Button employeeButton = rootVisualElement.Q<Button>("EmployeeButton");
        Button editButton = rootVisualElement.Q<Button>("EditButton");
        exitEditModeButton = rootVisualElement.Q<Button>("ExitEditModeButton");

        storeButton.RegisterCallback<ClickEvent>(SetButtonBehaviour);
        employeeButton.RegisterCallback<ClickEvent>(SetButtonBehaviour);
        editButton.RegisterCallback<ClickEvent>(OpenEditPanel);
        exitEditModeButton.RegisterCallback<ClickEvent>(CloseEditPanel);

        bottomLeftPanel.Add(storeButton);
        bottomLeftPanel.Add(employeeButton);
        bottomLeftPanel.Add(editButton);
        exitEditModeButton.visible = false;
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
        exitEditModeButton.visible = true;
        PauseGame();
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
        foreach (Button b in bottomLeftPanel)
        {
            b.visible = false;
        }
    }

    private void ShowLeftDownPanel()
    {
        foreach (Button b in bottomLeftPanel)
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
