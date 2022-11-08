using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using IEnumerator = System.Collections.IEnumerator;

public class GridDebugPanel : EditorWindow
{
    [SerializeField]
    private bool isGameSceneLoaded, gridDebugEnabled;
    private Label gridDebugContent;
    private VisualElement gridDisplay, mainContainer, ClientContainerGraphDebuger, EmployeeContainerGraphDebuger, comboBoxContainer;
    private TemplateContainer templateContainer;
    private Button buttonStartDebug;
    private GameController gameController;
    private List<Toggle> npcsToggle;
    private StateMachine<NpcState, NpcStateTransitions> currentStateMachine;
    private List<VisualElement> paintedStates;
    private HashSet<VisualElement> paintedStatesSet;
    private Toggle currentSelectedToggle;
    private Dictionary<NpcState, VisualElement> clientGraphNodes, employeeGraphNodes;
    private int maxComboboxView = 10, togglePos;
    private const string EMPTY_CELL_STYLE = "grid-cell-empty",
    BUSY_CELL_STYLE = "grid-cell-busy",
    ACTION_CELL_STYLE = "grid-cell-action",
    NPC_BUSY_CELL_STYLE = "grid-cell-npc",
    GRAPH_LEVEL = "graph-level",
    STATE_NODE = "state-node",
    STATE_NODE_ACTIVE = "state-node-active",
    STATE_NODE_PREV_ACTIVE = "state-node-prev-active",
    STATE_NODE_PREV_PREV_ACTIVE = "state-node-prev-prev-active",
    STATE_NAME_TITLE_LABEL = "state-name-title",
    NEXT_STATES_NODE_LABEL = "next-states-node-title";
    private string[] cssColors;

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
        buttonStartDebug.text = "In order to start, enter in Play mode. GameScene.";
        gridDebugContent = templateContainer.Q<Label>(Settings.GridDebugContent);
        gridDisplay = templateContainer.Q<VisualElement>(Settings.GridDisplay);
        mainContainer = templateContainer.Q<VisualElement>(Settings.MainContainer);

        ClientContainerGraphDebuger = templateContainer.Q<VisualElement>(Settings.ClientContainerGraphDebuger); // Place to show the graph
        EmployeeContainerGraphDebuger = templateContainer.Q<VisualElement>(Settings.EmployeeContainerGraphDebuger); // Place to show the graph
        comboBoxContainer = templateContainer.Q<VisualElement>(Settings.ComboBoxContainer); // Place to show the graph
        clientGraphNodes = new Dictionary<NpcState, VisualElement>();
        employeeGraphNodes = new Dictionary<NpcState, VisualElement>();
        paintedStates = new List<VisualElement>();
        paintedStatesSet = new HashSet<VisualElement>();
        BuildUIGraph(NPCStateMachineFactory.GetClientStateMachine(), ClientContainerGraphDebuger, clientGraphNodes); // Building, Lazy loading
        BuildUIGraph(NPCStateMachineFactory.GetEmployeeStateMachine(), EmployeeContainerGraphDebuger, employeeGraphNodes); // Building, Lazy loading
        cssColors = new string[] { STATE_NODE_ACTIVE, STATE_NODE_PREV_ACTIVE, STATE_NODE_PREV_PREV_ACTIVE };

        npcsToggle = new List<Toggle>();
        currentSelectedToggle = null;
        buildComboBoxView();
        EditorCoroutineUtility.StartCoroutine(UpdateUICoroutine(), this);
        mainContainer.SetEnabled(false);
    }

    // IEnumerator to not refresh the UI on the Update which is computational expensive 
    private IEnumerator UpdateUICoroutine()
    {
        for (; ; )
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (gridDebugEnabled)
                {
                    SetBussGrid();
                    string debugText = " ";
                    debugText += DebugBussData();
                    debugText += SetPlayerData();
                    gridDebugContent.text = debugText;
                    PaintCurrentSelectedNode();
                }
                else
                {
                    gridDebugContent.text = "";
                    gridDisplay.Clear();
                }
            }
            yield return new WaitForSeconds(1f);
        }
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

        maps += " \n";

        maps += "Queue TablesWithClient size: " + BussGrid.GetFreeBusinessSpots().Length + "\n";
        foreach (KeyValuePair<GameGridObject, byte> g in BussGrid.GetFreeBusinessSpots())
        {
            if (!g.Key.HasClient())
            {
                continue;
            }
            maps += "<b>" + g.Key.Name + "</b> \n";
        }

        maps += " \n";

        maps += "BusinessObjects size: " + BussGrid.GetBusinessObjects().Count + " \n";
        foreach (GameGridObject g in BussGrid.GetBusinessObjects().Values)
        {
            maps += "<b>" + g.Name + " Stored:" + PlayerData.IsItemStored(g.Name) + " Client:" + (g.GetUsedBy() != null) + " Dragged:" + g.GetIsObjectBeingDragged() + " Selected:" + g.GetIsObjectSelected() + " Bought:" + g.GetIsItemBought() + "</b> \n";
        }

        maps += " \n";

        maps += "FirebaseObjects size: " + PlayerData.GetFirebaseGameUser().OBJECTS.Count + " \n";
        foreach (FirebaseGameObject g in PlayerData.GetFirebaseGameUser().OBJECTS)
        {
            maps += "<b>ID:" + ((StoreItemType)g.ID) + " Stored:" + g.IS_STORED + " Position (" + g.POSITION[0] + "," + g.POSITION[1] + ") Rotation:" + ((ObjectRotation)g.ROTATION) + "</b> \n";
        }

        maps += "\n";

        return maps;
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

    private string SetPlayerData()
    {
        togglePos = 0;
        string output = "Employe and Client states: \n";
        if (gameController.GetEmployeeController() != null)
        {
            AddToggle(gameController.GetEmployeeController().Name);
            output += gameController.GetEmployeeController().Name + " State: " + gameController.GetEmployeeController().GetNpcState();
            output += " Time:" + gameController.GetEmployeeController().GetNpcStateTime() + " \n";
        }
        else
        {
            output += "Employee: NONE \n";
        }

        foreach (NPCController current in gameController.GetNpcSet())
        {
            AddToggle(current.Name);
            output += current.Name + " State: " + current.GetNpcState() + "(" + (current.GetTable() != null ? current.GetTable().Name : "null") + ") Time:" + current.GetNpcStateTime() + " - speed: " + current.GetSpeed() + " \n";
        }

        return output;
    }

    private void AddToggle(string name)
    {
        if (togglePos >= maxComboboxView)
        {
            return;
        }
        npcsToggle[togglePos].visible = true;
        npcsToggle[togglePos].text = name;
        npcsToggle[togglePos++].name = name;
    }

    private void buildComboBoxView()
    {
        // Toggle handler
        comboBoxContainer.Clear();
        npcsToggle.Clear();

        for (int i = 0; i < maxComboboxView; i++)
        {
            CreateComboBox(i.ToString());
        }
    }

    //create combobox
    private void CreateComboBox(string name)
    {
        Toggle playerToggle = new Toggle();
        playerToggle.SetEnabled(true);
        comboBoxContainer.Add(playerToggle);
        playerToggle.name = name;
        playerToggle.text = name;
        npcsToggle.Add(playerToggle);
        playerToggle.RegisterCallback<ClickEvent>(ToggleClickListener);
        playerToggle.visible = false;
    }

    private void ToggleClickListener(ClickEvent evt)
    {
        currentSelectedToggle = evt.currentTarget as Toggle;
        ComboBoxHandler(currentSelectedToggle);

        if (currentSelectedToggle.name.Contains(Settings.EMPLOYEE_PREFIX))
        {
            EmployeeContainerGraphDebuger.SetEnabled(true);
            ClientContainerGraphDebuger.SetEnabled(false);
        }
        else
        {
            EmployeeContainerGraphDebuger.SetEnabled(false);
            ClientContainerGraphDebuger.SetEnabled(true);
        }

        SetStateMachine(currentSelectedToggle.name);
    }

    //paint selected node
    private void PaintCurrentSelectedNode()
    {
        if (currentStateMachine == null)
        {
            return;
        }

        VisualElement node;

        if (currentSelectedToggle.name.Contains(Settings.EMPLOYEE_PREFIX))
        {
            node = employeeGraphNodes[currentStateMachine.Current.State];
        }
        else
        {
            node = clientGraphNodes[currentStateMachine.Current.State];
        }

        SetNodesColor(node);
    }

    private void SetNodesColor(VisualElement currentNode)
    {
        if (paintedStatesSet.Contains(currentNode))
        {
            return;
        }

        paintedStates.Insert(0, currentNode);
        paintedStatesSet.Add(currentNode);

        if (paintedStates.Count > 3)
        {
            VisualElement node = paintedStates[paintedStates.Count - 1];
            node.RemoveFromClassList(STATE_NODE_ACTIVE);
            node.RemoveFromClassList(STATE_NODE_PREV_ACTIVE);
            node.RemoveFromClassList(STATE_NODE_PREV_PREV_ACTIVE);
            paintedStatesSet.Remove(paintedStates[paintedStates.Count - 1]);
            paintedStates.RemoveAt(paintedStates.Count - 1);
        }

        for (int i = 0; i < paintedStates.Count; i++)
        {
            VisualElement node = paintedStates[i];
            node.RemoveFromClassList(STATE_NODE_ACTIVE);
            node.RemoveFromClassList(STATE_NODE_PREV_ACTIVE);
            node.RemoveFromClassList(STATE_NODE_PREV_PREV_ACTIVE);
            node.AddToClassList(cssColors[i]);
        }
    }

    private void SetStateMachine(string ID)
    {
        GameObjectMovementBase controller;

        if (ID.Contains(Settings.EMPLOYEE_PREFIX))
        {
            controller = BussGrid.GameController.GetEmployeeController();
        }
        else
        {
            controller = BussGrid.GameController.GetNPC(ID);
        }

        if (controller == null)
        {
            return;
        }

        currentStateMachine = controller.GetStateMachine();
    }

    private void BuildUIGraph(StateMachine<NpcState, NpcStateTransitions> stateMachine, VisualElement parent, Dictionary<NpcState, VisualElement> map)
    {
        if (stateMachine == null)
        {
            return;
        }
        parent.Clear();

        StateMachineNode<NpcState> startNode = stateMachine.GetStartNode();
        Queue<StateMachineNode<NpcState>> queue = new Queue<StateMachineNode<NpcState>>();
        HashSet<NpcState> visited = new HashSet<NpcState>();

        queue.Enqueue(startNode);
        int level = 0;

        while (queue.Count != 0)
        {
            int size = queue.Count;
            VisualElement UILevel = CreateUIGraphLevel(level.ToString());
            level++;

            while (size-- > 0)
            {
                StateMachineNode<NpcState> current = queue.Dequeue();
                if (visited.Contains(current.State))
                {
                    continue;
                }

                visited.Add(current.State);
                VisualElement UINode = CreateUIGraphNode(current.State.ToString(), current.GetNextStates());
                map.Add(current.State, UINode);
                UILevel.Add(UINode);

                foreach (StateMachineNode<NpcState> node in current.TransitionStates)
                {
                    if (!visited.Contains(node.State))
                    {
                        queue.Enqueue(node);
                    }
                }
            }
            parent.Add(UILevel);
        }
    }

    // This will handle that only one combobox can be selected at the time.
    private void ComboBoxHandler(Toggle toggle)
    {
        foreach (Toggle t in npcsToggle)
        {
            if (toggle == t)
            {
                t.value = true;
            }
            else
            {
                t.value = false;
            }
        }
    }

    private VisualElement CreateUIGraphLevel(string id)
    {
        VisualElement visualElement = new VisualElement()
        {
            name = id
        };
        visualElement.AddToClassList(GRAPH_LEVEL);
        return visualElement;
    }

    private VisualElement CreateUIGraphNode(string name, string nextStates)
    {
        VisualElement visualElement = new VisualElement();
        visualElement.AddToClassList(STATE_NODE);
        Label nodeTitle = new Label();
        nodeTitle.AddToClassList(STATE_NAME_TITLE_LABEL);
        nodeTitle.name = name;
        nodeTitle.text = name;
        Label nodeNextStates = new Label();
        nodeNextStates.AddToClassList(NEXT_STATES_NODE_LABEL);
        nodeNextStates.name = nextStates;
        nodeNextStates.text = nextStates;
        visualElement.Add(nodeTitle);
        visualElement.Add(nodeNextStates);
        return visualElement;
    }
}