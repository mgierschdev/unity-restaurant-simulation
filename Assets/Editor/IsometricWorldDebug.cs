
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class IsometricWorldDebug : EditorWindow
{
    [SerializeField]
    private GridController Grid;
    private bool gridDebugEnabled;
    private Label gridDebugContent;
    private VisualElement gridDisplay;
    private TemplateContainer templateContainer;
    private VisualElement cell;
    private Button buttonStartDebug;
    private VisualElement mainContainer;
    private const string EMPTY_CELL_STYLE = "grid-cell-empty";
    private const string BUSY_CELL_STYLE = "grid-cell-busy";

    [UnityEditor.MenuItem("Custom/IsometricWorldDebug")]
    public static void ShowExample()
    {
        IsometricWorldDebug wnd = GetWindow<IsometricWorldDebug>();
        wnd.titleContent = new GUIContent("IsometricWorldDebug");
    }

    public void CreateGUI()
    {
        //Loading grid controller
        GameObject grid = GameObject.Find(Settings.GameGrid);
        Grid = grid.GetComponent<GridController>();
        gridDebugEnabled = false;

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/IsometricWorldDebug.uxml");
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/IsometricWorldDebug.uss");

        // Adding debug label
        templateContainer = visualTree.Instantiate();
        root.Add(templateContainer);
        templateContainer.styleSheets.Add(styleSheet);


        // Setting up Variables
        buttonStartDebug = templateContainer.Q<Button>("GridDebugButton");
        buttonStartDebug.RegisterCallback<ClickEvent>(SetButtonBehaviour);
        buttonStartDebug.text = "In order to start, enter in Play mode";
        gridDebugContent = templateContainer.Q<Label>("GridDebug");
        gridDisplay = templateContainer.Q<VisualElement>("GridDisplay");
        cell = templateContainer.Q<VisualElement>("Cell");
        mainContainer = templateContainer.Q<VisualElement>("MainContainer");
        mainContainer.SetEnabled(false);
        //cell.AddToClassList("grid-cell-empty");

        //button.text = "";

        //Set button hanflers
       // SetupButtonHandler();
    }

    // This iterates all buttons 
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
            mainContainer.SetEnabled(false);
        }
        else
        {
            button.text = "Stop Debug";
            gridDebugEnabled = true;
            mainContainer.SetEnabled(true);
        }
    }
    private void Update()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (gridDebugEnabled && Grid)
            {
                BussGridToText();
                string deubgText = " ";
                deubgText += DebugBussData();
                deubgText += EntireGridToText();
                gridDebugContent.text = deubgText;
            }

        }
    }

    private string BussGridToText()
    {
        string output = " ";
        int[,] grid = Grid.GetGridArray();
        List<GameTile> listBusinessFloor = Grid.GetListBusinessFloor();

        int[,] busGrid = new int[grid.GetLength(0), grid.GetLength(1)];
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (GameTile tile in listBusinessFloor)
        {
            minX = Mathf.Min(minX, tile.GridPosition.x);
            minY = Mathf.Min(minY, tile.GridPosition.y);
            maxX = Mathf.Max(maxX, tile.GridPosition.x);
            maxY = Mathf.Max(maxY, tile.GridPosition.y);

            busGrid[tile.GridPosition.x, tile.GridPosition.y] = grid[tile.GridPosition.x, tile.GridPosition.y];
        }

        //We set the max size of the editor display
        Debug.Log("Size: " + (maxX - minX));
        gridDisplay.style.

        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                output += " " + busGrid[i, j];
            }
            output += "\n";
        }
        return output;
    }

    private string DebugBussData()
    {
        string objects = "";
        string maps = "";

        maps += "Queue FreeBusinessSpots size: " + Grid.GetFreeBusinessSpots().Count + "\n";
        foreach (GameGridObject g in Grid.GetFreeBusinessSpots())
        {
            maps += "Name: " + g.Name + " gridPosition: " + g.GridPosition + " worldmapPosition " + g.WorldPosition + "\n";
        }

        maps += "\n\n\n";

        maps += "Queue TablesWithClient size: " + Grid.GetTablesWithClient().Count + "\n";
        foreach (GameGridObject g in Grid.GetTablesWithClient())
        {
            maps += "Name: " + g.Name + " gridPosition: " + g.GridPosition + " worldmapPosition " + g.WorldPosition + "\n";
        }

        maps += "\n\n\n";

        objects += "businessObjects size: " + Grid.GetBusinessObjects().Count + " \n";
        foreach (GameGridObject g in Grid.GetBusinessObjects().Values)
        {
            objects += "Name: " + g.Name + " gridPosition: " + g.GridPosition + " worldmapPosition " + g.WorldPosition + "\n";
        }

        objects += "\n\n\n";

        objects += "BusyBusinessSpotsMap size: " + Grid.GetBusyBusinessSpotsMap().Count + " \n";
        foreach (GameGridObject g in Grid.GetBusyBusinessSpotsMap().Values)
        {
            objects += "Name: " + g.Name + " gridPosition: " + g.GridPosition + " worldmapPosition " + g.WorldPosition + "\n";
        }

        objects += "\n\n\n";

        objects += "FreeBusinessSpotsMap size: " + Grid.GetFreeBusinessSpotsMap().Count + " \n";
        foreach (GameGridObject g in Grid.GetFreeBusinessSpotsMap().Values)
        {
            objects += "Name: " + g.Name + " gridPosition: " + g.GridPosition + " worldmapPosition " + g.WorldPosition + "\n";
        }

        return maps + " " + objects;
    }

    private string EntireGridToText()
    {
        string output = " ";
        int[,] grid = Grid.GetGridArray();
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] == -1)
                {
                    output += " 0";
                }
                else
                {
                    output += " " + grid[i, j];
                }
            }
            output += "\n";
        }
        return output;
    }

}