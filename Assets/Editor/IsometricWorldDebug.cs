using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class IsometricWorldDebug : EditorWindow
{
    [SerializeField]
    private bool gridDebugEnabled;
    private Label gridDebugContent;
    private VisualElement gridDisplay;
    private TemplateContainer templateContainer;
    private Button buttonStartDebug;
    private VisualElement mainContainer;
    private GameController gameController;
    private const string EMPTY_CELL_STYLE = "grid-cell-empty";
    private const string BUSY_CELL_STYLE = "grid-cell-busy";
    private const string ACTION_CELL_STYLE = "grid-cell-action";

    [UnityEditor.MenuItem("Custom/IsometricWorldDebug")]
    public static void ShowExample()
    {
        IsometricWorldDebug wnd = GetWindow<IsometricWorldDebug>();
        wnd.titleContent = new GUIContent("IsometricWorldDebug");
    }

    public void CreateGUI()
    {
        gridDebugEnabled = false;

        //Loading grid controller
        GameObject gameObj = GameObject.Find(Settings.ConstParentGameObject);
        gameController = gameObj.GetComponent<GameController>();
        gridDebugEnabled = false;

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Settings.IsometricWorldDebugUI);
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Settings.IsometricWorldDebugUIStyles);

        // Adding debug label
        templateContainer = visualTree.Instantiate();
        root.Add(templateContainer);
        templateContainer.styleSheets.Add(styleSheet);

        // Setting up Variables
        buttonStartDebug = templateContainer.Q<Button>(Settings.DebugStartButton);
        buttonStartDebug.RegisterCallback<ClickEvent>(SetButtonBehaviour);
        buttonStartDebug.text = "In order to start, enter in Play mode";
        gridDebugContent = templateContainer.Q<Label>(Settings.GridDebugContent);
        gridDisplay = templateContainer.Q<VisualElement>(Settings.GridDisplay);
        mainContainer = templateContainer.Q<VisualElement>(Settings.MainContainer);
        mainContainer.SetEnabled(false);
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
            gridDisplay.Clear();
            gridDebugContent.text = "";
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
            if (gridDebugEnabled)
            {
                SetBussGrid();
                string debugText = " ";
                debugText += DebugBussData();
                debugText += GetPlayerStates();
                //deubgText += EntireGridToText();, This will print the entire grid
                gridDebugContent.text = debugText;
            }
            else
            {
                gridDebugContent.text = "";
                gridDisplay.Clear();
            }
        }
    }

    private void SetBussGrid()
    {
        int[,] grid = BussGrid.GetGridArray();
        List<GameTile> listBusinessFloor = BussGrid.GetListBusinessFloor();

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


        // we rotate the grid for the UI
        int[,] newGrid = new int[maxX - minX + 1, maxY - minY + 1];

        int iStart = minX;
        int iEnd = maxX + 1;
        int jStart = minY;
        int jEnd = maxY + 1;

        int indexX = 0;

        //traspose
        for (int i = iStart; i < iEnd; i++)
        {
            int indexY = 0;

            for (int j = jStart; j < jEnd; j++)
            {
                newGrid[indexX, indexY] = grid[i, j];

                indexY++;
            }

            indexX++;
        }

        newGrid = Util.TransposeGridForDebugging(newGrid);

        // We set the Display
        //We set the max size of the editor display
        gridDisplay.style.width = newGrid.GetLength(0) * 30;
        //we clean prev childs
        gridDisplay.Clear();

        for (int i = 0; i < newGrid.GetLength(0); i++)
        {
            for (int j = 0; j < newGrid.GetLength(1); j++)
            {
                VisualElement cell = new VisualElement();
                gridDisplay.Add(cell);

                if (newGrid[i, j] == 0)
                {
                    cell.AddToClassList(EMPTY_CELL_STYLE);
                }
                else if (newGrid[i, j] == 1)
                {
                    cell.AddToClassList(BUSY_CELL_STYLE);
                }
                else
                {
                    //Action cell
                    cell.AddToClassList(ACTION_CELL_STYLE);
                }
            }
        }
    }

    private string DebugBussData()
    {
        string objects = "";
        string maps = "";

        maps += "Queue FreeBusinessSpots size: " + BussGrid.GetFreeBusinessSpots().Count + "\n";
        foreach (GameGridObject g in BussGrid.GetFreeBusinessSpots())
        {
            maps += "<b>" + g.Name + "</b> \n";
        }

        maps += "\n\n";

        maps += "Queue TablesWithClient size: " + BussGrid.GetTablesWithClient().Count + "\n";
        foreach (GameGridObject g in BussGrid.GetTablesWithClient())
        {
            maps += "<b>" + g.Name + "</b> \n";
        }

        maps += "\n\n";

        objects += "businessObjects size: " + BussGrid.GetBusinessObjects().Count + " \n";
        foreach (GameGridObject g in BussGrid.GetBusinessObjects().Values)
        {
            objects += "<b>" + g.Name + "</b> \n";
        }

        objects += "\n\n";

        objects += "BusyBusinessSpotsMap size: " + BussGrid.GetBusyBusinessSpotsMap().Count + " \n";
        foreach (GameGridObject g in BussGrid.GetBusyBusinessSpotsMap().Values)
        {
            objects += "<b>" + g.Name + "</b>\n";
        }

        objects += "\n\n";

        objects += "FreeBusinessSpotsMap size: " + BussGrid.GetFreeBusinessSpotsMap().Count + " \n";
        foreach (GameGridObject g in BussGrid.GetFreeBusinessSpotsMap().Values)
        {
            objects += "<b>" + g.Name + "</b>\n";
        }

        return maps + " " + objects;
    }

    private string EntireGridToText()
    {
        string output = " ";
        int[,] grid = BussGrid.GetGridArray();
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

    private string GetPlayerStates()
    {
        string output = "Employe and Client states: \n";
        if (gameController.GetEmployeeController() != null)
        {
            output += gameController.GetEmployeeController().Name + " State: " + gameController.GetEmployeeController().GetNpcState();
            output += " Time:" + gameController.GetEmployeeController().GetNpcStateTime() + " \n";
        }
        else
        {
            output += "Employee: NONE \n";
        }

        foreach (NPCController current in gameController.GetNpcSet())
        {
            output += current.Name + " State: " + current.GetNpcState() + " Time in State: " + current.GetNpcStateTime() +" - speed: "+current.GetSpeed()+ " \n";
        }

        return output;
    }
}