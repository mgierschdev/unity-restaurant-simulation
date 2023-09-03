#if (UNITY_EDITOR)

using System.Collections.Generic;
using Game.Controllers.NPC_Controllers;
using Game.Controllers.Other_Controllers;
using Game.Grid;
using Game.Players;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Util;
using Util.Collections;
using IEnumerator = System.Collections.IEnumerator;

namespace Editor
{
    public class StateMachineDebugPanel : EditorWindow
    {
        [SerializeField] private bool isGameSceneLoaded, gridDebugEnabled;
        private Label _gridDebugContent;

        private VisualElement _mainContainer,
            _clientContainerGraphDebugger,
            _employeeContainerGraphDebugger,
            _comboBoxContainer;

        private TemplateContainer _templateContainer;
        private Button _buttonStartDebug;
        private GameController _gameController;
        private List<Toggle> _npcToggle;
        private StateMachine<NpcState, NpcStateTransitions> _currentStateMachine;
        private List<VisualElement> _paintedStates;
        private HashSet<VisualElement> _paintedStatesSet;
        private Toggle _currentSelectedToggle;
        private Dictionary<NpcState, VisualElement> _clientGraphNodes, _employeeGraphNodes;
        private const int MaxComboboxView = 10;
        private int _togglePos;

        private const string GraphLevel = "graph-level",
            StateNode = "state-node",
            StateNodeActive = "state-node-active",
            StateNodePrevActive = "state-node-prev-active",
            StateNodePrevPrevActive = "state-node-prev-prev-active",
            StateNameTitleLabel = "state-name-title",
            NextStatesNodeLabel = "next-states-node-title";

        private string[] _cssColors;

        [MenuItem(Settings.GameName + "/Debug: NPC State Machine")]
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
            var root = rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Settings.IsometricWorldDebugUIStateMachine);
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(Settings.IsometricWorldDebugUIStylesStateMachine);
            _templateContainer = visualTree.Instantiate();
            root.Add(_templateContainer);
            _templateContainer.styleSheets.Add(styleSheet);
            _buttonStartDebug = _templateContainer.Q<Button>(Settings.DebugStartButton);
            _buttonStartDebug.RegisterCallback<ClickEvent>(SetButtonBehaviour);
            _buttonStartDebug.text = "In order to start, enter in Play mode. GameScene.";
            _gridDebugContent = _templateContainer.Q<Label>(Settings.GridDebugContent);
            _mainContainer = _templateContainer.Q<VisualElement>(Settings.MainContainer);

            _clientContainerGraphDebugger =
                _templateContainer.Q<VisualElement>(Settings.ClientContainerGraphDebugger); // Place to show the graph
            _employeeContainerGraphDebugger =
                _templateContainer.Q<VisualElement>(Settings.EmployeeContainerGraphDebugger); // Place to show the graph
            _comboBoxContainer =
                _templateContainer.Q<VisualElement>(Settings.ComboBoxContainer); // Place to show the graph
            _clientGraphNodes = new Dictionary<NpcState, VisualElement>();
            _employeeGraphNodes = new Dictionary<NpcState, VisualElement>();
            _paintedStates = new List<VisualElement>();
            _paintedStatesSet = new HashSet<VisualElement>();
            BuildUIGraph(NpcStateMachineFactory.GetClientStateMachine("ID"), _clientContainerGraphDebugger,
                _clientGraphNodes); // Building, Lazy loading
            BuildUIGraph(NpcStateMachineFactory.GetEmployeeStateMachine("ID"), _employeeContainerGraphDebugger,
                _employeeGraphNodes); // Building, Lazy loading
            _cssColors = new[] { StateNodeActive, StateNodePrevActive, StateNodePrevPrevActive };

            _npcToggle = new List<Toggle>();
            _currentSelectedToggle = null;
            BuildComboBoxView();
            EditorCoroutineUtility.StartCoroutine(UpdateUICoroutine(), this);
            _mainContainer.SetEnabled(false);
        }

        // IEnumerator to not refresh the UI on the Update which is computational expensive 
        private IEnumerator UpdateUICoroutine()
        {
            for (;;)
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    if (gridDebugEnabled)
                    {
                        if (_gameController == null)
                        {
                            GameObject gameObj = GameObject.Find(Settings.ConstParentGameObject);
                            _gameController = gameObj.GetComponent<GameController>();
                        }

                        SetPlayerData();
                        PaintCurrentSelectedNode();
                    }
                    else
                    {
                        _gridDebugContent.text = "";
                    }
                }

                yield return new WaitForSeconds(1f);
            }
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

            var button = evt.currentTarget as Button;
            if (button == null)
            {
                return;
            }

            if (gridDebugEnabled)
            {
                button.text = "Start Debug";
                gridDebugEnabled = false;
                _mainContainer.SetEnabled(false);
                _gridDebugContent.text = "";
            }
            else
            {
                button.text = "Stop Debug";
                gridDebugEnabled = true;
                _mainContainer.SetEnabled(true);
            }
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().name != Settings.GameScene)
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
            _togglePos = 0;
            string output = "Employe and Client states: \n";
            foreach (EmployeeController employeeController in _gameController.GetEmployeeSet())
            {
                AddToggle(employeeController.Name);
                output += employeeController.Name + " State: " + employeeController.GetNpcState();
                output += " Time:" + employeeController.GetNpcStateTime() + " \n";
            }

            foreach (ClientController current in _gameController.GetNpcSet())
            {
                AddToggle(current.Name);
                output += current.Name + " State: " + current.GetNpcState() + "(" +
                          (current.GetTable() != null ? current.GetTable().Name : "null") + ") Time:" +
                          current.GetNpcStateTime() + " - speed: " + current.GetSpeed() + " \n";
            }

            return output;
        }

        private void AddToggle(string npcName)
        {
            if (_togglePos >= MaxComboboxView)
            {
                return;
            }

            _npcToggle[_togglePos].visible = true;
            _npcToggle[_togglePos].text = npcName;
            _npcToggle[_togglePos++].name = npcName;
        }

        private void BuildComboBoxView()
        {
            // Toggle handler
            _comboBoxContainer.Clear();
            _npcToggle.Clear();

            for (int i = 0; i < MaxComboboxView; i++)
            {
                CreateComboBox(i.ToString());
            }
        }

        //create combobox
        private void CreateComboBox(string playerName)
        {
            Toggle playerToggle = new Toggle();
            playerToggle.SetEnabled(true);
            _comboBoxContainer.Add(playerToggle);
            playerToggle.name = playerName;
            playerToggle.text = playerName;
            _npcToggle.Add(playerToggle);
            playerToggle.RegisterCallback<ClickEvent>(ToggleClickListener);
            playerToggle.visible = false;
        }

        private void ToggleClickListener(ClickEvent evt)
        {
            _currentSelectedToggle = evt.currentTarget as Toggle;
            ComboBoxHandler(_currentSelectedToggle);

            if (_currentSelectedToggle != null && _currentSelectedToggle.name.Contains(Settings.EmployeePrefix))
            {
                _employeeContainerGraphDebugger.SetEnabled(true);
                _clientContainerGraphDebugger.SetEnabled(false);
            }
            else
            {
                _employeeContainerGraphDebugger.SetEnabled(false);
                _clientContainerGraphDebugger.SetEnabled(true);
            }

            if (_currentSelectedToggle != null)
            {
                SetStateMachine(_currentSelectedToggle.name);
            }
        }

        //paint selected node
        private void PaintCurrentSelectedNode()
        {
            if (_currentStateMachine == null)
            {
                return;
            }

            VisualElement node;

            if (_currentSelectedToggle.name.Contains(Settings.EmployeePrefix))
            {
                node = _employeeGraphNodes[_currentStateMachine.Current.State];
            }
            else
            {
                node = _clientGraphNodes[_currentStateMachine.Current.State];
            }

            _gridDebugContent.text = _currentStateMachine.DebugTransitions();

            SetNodesColor(node);
        }

        private void SetNodesColor(VisualElement currentNode)
        {
            if (_paintedStatesSet.Contains(currentNode))
            {
                return;
            }

            _paintedStates.Insert(0, currentNode);
            _paintedStatesSet.Add(currentNode);

            if (_paintedStates.Count > 3)
            {
                VisualElement node = _paintedStates[_paintedStates.Count - 1];
                node.RemoveFromClassList(StateNodeActive);
                node.RemoveFromClassList(StateNodePrevActive);
                node.RemoveFromClassList(StateNodePrevPrevActive);
                _paintedStatesSet.Remove(_paintedStates[_paintedStates.Count - 1]);
                _paintedStates.RemoveAt(_paintedStates.Count - 1);
            }

            for (int i = 0; i < _paintedStates.Count; i++)
            {
                VisualElement node = _paintedStates[i];
                node.RemoveFromClassList(StateNodeActive);
                node.RemoveFromClassList(StateNodePrevActive);
                node.RemoveFromClassList(StateNodePrevPrevActive);
                node.AddToClassList(_cssColors[i]);
            }
        }

        private void SetStateMachine(string id)
        {
            GameObjectMovementBase controller;

            if (id.Contains(Settings.EmployeePrefix))
            {
                controller = BussGrid.GameController.GetEmployee(id);
            }
            else
            {
                controller = BussGrid.GameController.GetNpc(id);
            }

            if (controller == null)
            {
                return;
            }

            _currentStateMachine = controller.GetStateMachine();
        }

        private void BuildUIGraph(StateMachine<NpcState, NpcStateTransitions> stateMachine, VisualElement parent,
            Dictionary<NpcState, VisualElement> map)
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
                VisualElement uiLevel = CreateUIGraphLevel(level.ToString());
                level++;

                while (size-- > 0)
                {
                    StateMachineNode<NpcState> current = queue.Dequeue();
                    if (visited.Contains(current.State))
                    {
                        continue;
                    }

                    visited.Add(current.State);
                    VisualElement uiNode = CreateUIGraphNode(current.State.ToString(), current.GetNextStates());
                    map.Add(current.State, uiNode);
                    uiLevel.Add(uiNode);

                    foreach (StateMachineNode<NpcState> node in current.TransitionStates)
                    {
                        if (!visited.Contains(node.State))
                        {
                            queue.Enqueue(node);
                        }
                    }
                }

                parent.Add(uiLevel);
            }
        }

        // This will handle that only one combobox can be selected at the time.
        private void ComboBoxHandler(Toggle toggle)
        {
            foreach (Toggle t in _npcToggle)
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
            visualElement.AddToClassList(GraphLevel);
            return visualElement;
        }

        private VisualElement CreateUIGraphNode(string titleName, string nextStates)
        {
            VisualElement visualElement = new VisualElement();
            visualElement.AddToClassList(StateNode);
            Label nodeTitle = new Label();
            nodeTitle.AddToClassList(StateNameTitleLabel);
            nodeTitle.name = titleName;
            nodeTitle.text = titleName;
            Label nodeNextStates = new Label();
            nodeNextStates.AddToClassList(NextStatesNodeLabel);
            nodeNextStates.name = nextStates;
            nodeNextStates.text = nextStates;
            visualElement.Add(nodeTitle);
            visualElement.Add(nodeNextStates);
            return visualElement;
        }
    }
}

#endif