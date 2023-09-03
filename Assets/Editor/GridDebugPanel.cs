#if (UNITY_EDITOR)

using System;
using System.Collections.Generic;
using Game.Controllers.NPC_Controllers;
using Game.Controllers.Other_Controllers;
using Game.Grid;
using Game.Players;
using Game.Players.Model;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Util;
using IEnumerator = System.Collections.IEnumerator;

namespace Editor
{
    public class GridDebugPanel : EditorWindow
    {
        [SerializeField] private bool isGameSceneLoaded, gridDebugEnabled;
        private Label _gridDebugContent;

        private VisualElement _gridDisplay,
            _mainContainer,
            _clientContainerGraphDebugger,
            _employeeContainerGraphDebugger;

        private TemplateContainer _templateContainer;
        private Button _buttonStartDebug;
        private GameController _gameController;

        private const string EmptyCellStyle = "grid-cell-empty",
            BusyCellStyle = "grid-cell-busy",
            ActionCellStyle = "grid-cell-action",
            NpcBusyCellStyle = "grid-cell-npc";

        [MenuItem(Settings.GameName + "/Play: First Scene")]
        public static void RunMainScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + Settings.LoadScene + ".unity");
            EditorApplication.isPlaying = true;
        }

        [MenuItem(Settings.GameName + "/Debug: Grid Panel")]
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
            _templateContainer = visualTree.Instantiate();
            root.Add(_templateContainer);
            _templateContainer.styleSheets.Add(styleSheet);
            _buttonStartDebug = _templateContainer.Q<Button>(Settings.DebugStartButton);
            _buttonStartDebug.RegisterCallback<ClickEvent>(SetButtonBehaviour);
            _buttonStartDebug.text = "In order to start, enter in Play mode. GameScene.";
            _gridDebugContent = _templateContainer.Q<Label>(Settings.GridDebugContent);
            _gridDisplay = _templateContainer.Q<VisualElement>(Settings.GridDisplay);
            _mainContainer = _templateContainer.Q<VisualElement>(Settings.MainContainer);
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

                        SetBussGrid();
                        string debugText = " ";
                        debugText += DebugBussData();
                        debugText += SetPlayerData();
                        _gridDebugContent.text = debugText;
                    }
                    else
                    {
                        _gridDebugContent.text = "";
                        _gridDisplay.Clear();
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

            var button = evt.currentTarget as Button;

            if (button == null)
            {
                throw new ArgumentException(nameof(button));
            }

            if (gridDebugEnabled)
            {
                button.text = "Start Debug";
                gridDebugEnabled = false;
                _mainContainer.SetEnabled(false);
                _gridDisplay.Clear();
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
                //Loading grid controller
                gridDebugEnabled = false;
                isGameSceneLoaded = true;
            }
        }

        private void SetBussGrid()
        {
            int[,] grid = BussGrid.GetGridArray();
            int[,] busGrid = BussGrid.GetBussGridWithinGrid();

            // Transpose
            for (int i = 0; i < busGrid.GetLength(0); i++)
            {
                for (int j = 0; j < busGrid.GetLength(1); j++)
                {
                    if (BussGrid.GameController.GetPlayerPositionSet().Contains(new Vector3Int(i, j, 0)))
                    {
                        busGrid[i, j] = -2;
                    }
                    else
                    {
                        busGrid[i, j] = grid[i, j];
                    }
                }
            }

            var newGrid = Util.Util.TransposeGridForDebugging(busGrid);

            // We set the Display
            // We set the max size of the editor display
            _gridDisplay.style.width = grid.GetLength(0) * 30;
            // We clean prev children
            _gridDisplay.Clear();

            for (int i = 10; i < newGrid.GetLength(0) - 5; i++)
            {
                for (int j = 0; j < newGrid.GetLength(1); j++)
                {
                    VisualElement cell = new VisualElement();
                    _gridDisplay.Add(cell);

                    if (newGrid[i, j] == (int)CellValue.NpcPosition)
                    {
                        cell.AddToClassList(NpcBusyCellStyle);
                        continue;
                    }

                    if (newGrid[i, j] == (int)CellValue.Empty)
                    {
                        cell.AddToClassList(EmptyCellStyle);
                    }
                    else if (newGrid[i, j] == (int)CellValue.Busy)
                    {
                        cell.AddToClassList(BusyCellStyle);
                    }
                    else
                    {
                        cell.AddToClassList(ActionCellStyle);
                    }
                }
            }
        }

        private string DebugBussData()
        {
            string maps = "";

            maps += "Queue FreeBusinessSpots size: " + TableHandler.GetFreeBusinessSpots().Length + "\n";
            foreach (KeyValuePair<GameGridObject, byte> g in TableHandler.GetFreeBusinessSpots())
            {
                if (!g.Key.IsFree())
                {
                    continue;
                }

                maps += "<b>" + g.Key.Name + "</b> \n";
            }

            maps += " \n";

            maps += "Queue TablesWithClient size: " + TableHandler.GetFreeBusinessSpots().Length + "\n";
            foreach (KeyValuePair<GameGridObject, byte> g in TableHandler.GetFreeBusinessSpots())
            {
                if (!g.Key.HasClient())
                {
                    continue;
                }

                maps += "<b>" + g.Key.Name + "</b> \n";
            }

            maps += " \n";

            maps += "Active: BusinessObjects Total: " + BussGrid.GetGameGridObjectsDictionary().Count + " \n";
            foreach (GameGridObject g in BussGrid.GetGameGridObjectsDictionary().Values)
            {
                if (PlayerData.IsItemStored(g.Name))
                {
                    continue;
                }

                maps += "<b>" + g.Name + " Stored:" + PlayerData.IsItemStored(g.Name) + " Client:" +
                        (g.GetUsedBy() != null) + " Dragged:" + g.GetIsObjectBeingDragged() + " Selected:" +
                        g.GetIsObjectSelected() + " Bought:" + g.GetIsItemBought() + " Client:" +
                        (g.GetUsedBy() != null) + " Emp:" + g.HasAttendedBy() + "</b> \n";
            }

            maps += " \n";

            maps += "Backend storage size: " + PlayerData.GerUserObjects().Count + " \n";
            foreach (DataGameObject g in PlayerData.GerUserObjects())
            {
                maps += "<b>ID:" + ((StoreItemType)g.id) + " Stored:" + g.isStored + " Position (" + g.position[0] +
                        "," + g.position[1] + ") Rotation:" + ((ObjectRotation)g.rotation) + "</b> \n";
            }

            maps += "\n";

            return maps;
        }

        private string SetPlayerData()
        {
            string output = "Employee and Client states: \n";

            foreach (EmployeeController employeeController in _gameController.GetEmployeeSet())
            {
                output += employeeController.Name + " State: " + employeeController.GetNpcState();
                output += " Time:" + employeeController.GetNpcStateTime() + " \n";
            }

            foreach (ClientController current in _gameController.GetNpcSet())
            {
                output += current.Name + " State: " + current.GetNpcState() + "(" +
                          (current.GetTable() != null ? current.GetTable().Name : "null") + ") Time:" +
                          current.GetNpcStateTime() + " - speed: " + current.GetSpeed() + " \n";
            }

            return output;
        }
    }
}

#endif