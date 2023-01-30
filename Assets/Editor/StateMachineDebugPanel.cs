#if (UNITY_EDITOR) 

using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using IEnumerator = System.Collections.IEnumerator;

public class StateMachineDebugPanel : EditorWindow
{
    [SerializeField]
    private bool isGameSceneLoaded, gridDebugEnabled;
    private Label gridDebugContent;
    private VisualElement mainContainer, ClientContainerGraphDebuger, EmployeeContainerGraphDebuger, comboBoxContainer;
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
    private const string GRAPH_LEVEL = "graph-level",
    STATE_NODE = "state-node",
    STATE_NODE_ACTIVE = "state-node-active",
    STATE_NODE_PREV_ACTIVE = "state-node-prev-active",
    STATE_NODE_PREV_PREV_ACTIVE = "state-node-prev-prev-active",
    STATE_NAME_TITLE_LABEL = "state-name-title",
    NEXT_STATES_NODE_LABEL = "next-states-node-title";
    private string[] cssColors;

    [UnityEditor.MenuItem(Settings.gameName + "/Debug: NPC State Machine")]
    public static void ShowExample()
    {
        StateMachineDebugPanel wnd = GetWindow<StateMachineDebugPanel>();
        wnd.titleContent = new GUIContent("State Machine debug Panel");
    }

    public void CreateGUI()
    {
        gridDebugEnabled = false;
        isGameSceneLoaded = false;

        // Setting up: Editor window UI parameters
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Settings.IsometricWorldDebugUIStateMachine);
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Settings.IsometricWorldDebugUIStylesStateMachine);
        templateContainer = visualTree.Instantiate();
        root.Add(templateContainer);
        templateContainer.styleSheets.Add(styleSheet);
        buttonStartDebug = templateContainer.Q<Button>(Settings.DebugStartButton);
        buttonStartDebug.RegisterCallback<ClickEvent>(SetButtonBehaviour);
        buttonStartDebug.text = "In order to start, enter in Play mode. GameScene.";
        gridDebugContent = templateContainer.Q<Label>(Settings.GridDebugContent);
        mainContainer = templateContainer.Q<VisualElement>(Settings.MainContainer);

        ClientContainerGraphDebuger = templateContainer.Q<VisualElement>(Settings.ClientContainerGraphDebuger); // Place to show the graph
        EmployeeContainerGraphDebuger = templateContainer.Q<VisualElement>(Settings.EmployeeContainerGraphDebuger); // Place to show the graph
        comboBoxContainer = templateContainer.Q<VisualElement>(Settings.ComboBoxContainer); // Place to show the graph
        clientGraphNodes = new Dictionary<NpcState, VisualElement>();
        employeeGraphNodes = new Dictionary<NpcState, VisualElement>();
        paintedStates = new List<VisualElement>();
        paintedStatesSet = new HashSet<VisualElement>();
        BuildUIGraph(NPCStateMachineFactory.GetClientStateMachine("ID"), ClientContainerGraphDebuger, clientGraphNodes); // Building, Lazy loading
        BuildUIGraph(NPCStateMachineFactory.GetEmployeeStateMachine("ID"), EmployeeContainerGraphDebuger, employeeGraphNodes); // Building, Lazy loading
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
                // TODO: and scene is the game scene
                if (gridDebugEnabled)
                {
                    if (gameController == null)
                    {
                        GameObject gameObj = GameObject.Find(Settings.ConstParentGameObject);
                        gameController = gameObj.GetComponent<GameController>();
                    }

                    SetPlayerData();
                    PaintCurrentSelectedNode();
                }
                else
                {
                    gridDebugContent.text = "";
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
            gridDebugEnabled = false;
            isGameSceneLoaded = true;
        }
    }

    private string SetPlayerData()
    {
        togglePos = 0;
        string output = "Employe and Client states: \n";
        foreach (EmployeeController employeeController in gameController.GetEmployeeSet())
        {
            AddToggle(employeeController.Name);
            output += employeeController.Name + " State: " +employeeController.GetNpcState();
            output += " Time:" + employeeController.GetNpcStateTime() + " \n";
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

        gridDebugContent.text = currentStateMachine.DebugTransitions();

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
            controller = BussGrid.GameController.GetEmployee(ID);
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

#endif