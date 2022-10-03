
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

        // Adding debug label
        TemplateContainer tamplateContainer = visualTree.Instantiate();
        root.Add(tamplateContainer);

        // Setting up Variables
        gridDebugContent = tamplateContainer.Q<Label>("GridDebug");
        gridDisplay = tamplateContainer.Q<VisualElement>("GridDisplay");

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
        if (gridDebugEnabled && Grid)
        {
            gridDebugContent.text = BussGridToText();
            gridDebugContent.text += DebugBussData();
            gridDebugContent.text += EntireGridToText();
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