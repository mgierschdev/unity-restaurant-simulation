using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class GridDebugPanel : EditorWindow
{
    [SerializeField]
    private bool isGameSceneLoaded;
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
    private const string NPC_BUSY_CELL_STYLE = "grid-cell-npc";

    [UnityEditor.MenuItem(Settings.gameName + "/Play First Scene")]
    public static void RunMainScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/" + Settings.LoadScene + ".unity");
        EditorApplication.isPlaying = true;
    }

    [UnityEditor.MenuItem(Settings.gameName + "/Grid Debug Panel")]
    public static void ShowExample()
    {
        GridDebugPanel wnd = GetWindow<GridDebugPanel>();
        wnd.titleContent = new GUIContent("Grid Debug Panel");
    }

    public void CreateGUI()
    {
        gridDebugEnabled = false;
        isGameSceneLoaded = false;

        // Setting up: Editor window UI parameters
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Settings.IsometricWorldDebugUI);
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Settings.IsometricWorldDebugUIStyles);
        templateContainer = visualTree.Instantiate();
        root.Add(templateContainer);
        templateContainer.styleSheets.Add(styleSheet);
        buttonStartDebug = templateContainer.Q<Button>(Settings.DebugStartButton);
        buttonStartDebug.RegisterCallback<ClickEvent>(SetButtonBehaviour);
        buttonStartDebug.text = "In order to start, enter in Play mode . GameScene.";
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
        if (EditorSceneManager.GetActiveScene().name != Settings.GameScene)
        {
            return;
        }

        if (!isGameSceneLoaded)
        {
            //Loading grid controller
            GameObject gameObj = GameObject.Find(Settings.ConstParentGameObject);
            gameController = gameObj.GetComponent<GameController>();
            gridDebugEnabled = false;
            isGameSceneLoaded = true;
        }
        else
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (gridDebugEnabled)
                {
                    SetBussGrid();
                    string debugText = " ";
                    debugText += DebugBussData();
                    debugText += GetPlayerStates();
                    gridDebugContent.text = debugText;
                }
                else
                {
                    gridDebugContent.text = "";
                    gridDisplay.Clear();
                }
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

        // Traspose
        for (int i = 0; i < busGrid.GetLength(0); i++)
        {
            for (int j = 0; j < busGrid.GetLength(1); j++)
            {
                if (BussGrid.IsThereNPCInPosition(new Vector3Int(i, j)))
                {
                    busGrid[i, j] = -2;
                }
                else
                {
                    busGrid[i, j] = grid[i, j];
                }
            }
        }

        int[,] newGrid = Util.TransposeGridForDebugging(busGrid);

        // We set the Display
        // We set the max size of the editor display
        gridDisplay.style.width = grid.GetLength(0) * 30;
        // We clean prev childs
        gridDisplay.Clear();

        for (int i = 10; i < newGrid.GetLength(0) - 5; i++)
        {
            for (int j = 0; j < newGrid.GetLength(1); j++)
            {
                VisualElement cell = new VisualElement();
                gridDisplay.Add(cell);

                if (newGrid[i, j] == (int)CellValue.NPC_POSITION)
                {
                    cell.AddToClassList(NPC_BUSY_CELL_STYLE);
                    continue;
                }

                if (newGrid[i, j] == (int)CellValue.EMPTY)
                {
                    cell.AddToClassList(EMPTY_CELL_STYLE);
                }
                else if (newGrid[i, j] == (int)CellValue.BUSY)
                {
                    cell.AddToClassList(BUSY_CELL_STYLE);
                }
                else
                {
                    cell.AddToClassList(ACTION_CELL_STYLE);
                }
            }
        }
    }

    private string DebugBussData()
    {
        string objects = "";
        string maps = "";

        maps += "Queue FreeBusinessSpots size: " + BussGrid.GetFreeBusinessSpots().Length + "\n";
        foreach (KeyValuePair<GameGridObject, byte> g in BussGrid.GetFreeBusinessSpots())
        {
            if (!g.Key.IsFree())
            {
                continue;
            }
            maps += "<b>" + g.Key.Name + "</b> \n";
        }

        maps += "\n \n";

        maps += "Queue TablesWithClient size: " + BussGrid.GetFreeBusinessSpots().Length + "\n";
        foreach (KeyValuePair<GameGridObject, byte> g in BussGrid.GetFreeBusinessSpots())
        {
            if (!g.Key.HasClient())
            {
                continue;
            }
            maps += "<b>" + g.Key.Name + "</b> \n";
        }

        maps += "\n \n";

        objects += "BusinessObjects size: " + BussGrid.GetBusinessObjects().Count + " \n";
        foreach (GameGridObject g in BussGrid.GetBusinessObjects().Values)
        {
            objects += "<b>" + g.Name + " Stored:" + PlayerData.IsItemStored(g.Name) + " Client:" + (g.GetUsedBy() != null) + " Dragged:" + g.GetIsObjectBeingDragged() + " Selected:" + g.GetIsObjectSelected() + " Bought:" + g.GetIsItemBought() + "</b> \n";
        }

        maps += "\n \n";

        objects += "FirebaseObjects size: " + PlayerData.GetFirebaseGameUser().OBJECTS.Count + " \n";
        foreach (FirebaseGameObject g in PlayerData.GetFirebaseGameUser().OBJECTS)
        {
            objects += "<b>ID:" + ((StoreItemType)g.ID) + " Stored:" + g.IS_STORED + " Position (" + g.POSITION[0] + "," + g.POSITION[1] + ") Rotation:" + ((ObjectRotation)g.ROTATION) + "</b> \n";
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
            output += current.Name + " State: " + current.GetNpcState() + "(" + (current.GetTable() != null ? current.GetTable().Name : "null") + ") Time:" + current.GetNpcStateTime() + " - speed: " + current.GetSpeed() + " \n";
        }

        return output;
    }
}