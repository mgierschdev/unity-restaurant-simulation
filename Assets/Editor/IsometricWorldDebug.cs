
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class IsometricWorldDebug : EditorWindow
{
    [SerializeField]
    private GridController gameGridController;
    private bool gridDebugEnabled;
    private Label gridDebugContent;

    [UnityEditor.MenuItem("Custom/IsometricWorldDebug")]
    public static void ShowExample()
    {
        IsometricWorldDebug wnd = GetWindow<IsometricWorldDebug>();
        wnd.titleContent = new GUIContent("IsometricWorldDebug");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/IsometricWorldDebug.uxml");
        TemplateContainer labelFromUXML = visualTree.Instantiate();
        GameObject grid = GameObject.Find(Settings.GameGrid);
        gameGridController = grid.GetComponent<GridController>();
        gridDebugContent = new Label("");
        root.Add(labelFromUXML);
        root.Add(gridDebugContent);
        gridDebugEnabled = false;
        //Set button hanflers
        SetupButtonHandler();
    }

    private void SetupButtonHandler()
    {
        var buttons = rootVisualElement.Query<Button>();
        buttons.ForEach(RegisterHandler);
    }

    private void RegisterHandler(Button button)
    {
        button.RegisterCallback<ClickEvent>(SetButtonBehaviour);
    }

    private void SetButtonBehaviour(ClickEvent evt)
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        Button button = evt.currentTarget as Button;

        if (gridDebugEnabled)
        {
            button.text = "Start Debug";
            gridDebugEnabled = false;
        }
        else
        {
            button.text = "Stop Debug";
            gridDebugEnabled = true;
        }
    }
    private void Update()
    {
        if (gridDebugEnabled && gameGridController)
        {
            gridDebugContent.text = gameGridController.BussGridToText();
            Debug.Log("Setting grid debug content");
        }
    }
}