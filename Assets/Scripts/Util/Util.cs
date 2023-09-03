using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Game.Controllers.Other_Controllers;
using Game.Grid;
using UnityEngine;
using Util.Collections;
using Random = UnityEngine.Random;

// This will contain Utility functions, to create Unity Object and other
namespace Util
{
    public static class Util
    {
        public const int NpcPositionZ = 0; // Z ordering objects , less closer to the camera
        public const int ObjectZPosition = -1;
        public const int SelectedObjectZPosition = -2;
        public const int HighlightObjectSortingPosition = 800; //UI sorting
        public static Color DisableColor = new Color(0.7f, 0.7f, 0.7f, 1); // Colors
        public static Color AvailableColor = new Color(0, 1, 0, 1);
        public static Color OccupiedColor = new Color(1, 0, 0, 1f);
        public static Color FreeColor = new Color(1, 1, 1, 1);
        public static Color HiddenColor = new Color(0, 0, 0, 0);
        public static Color UnderTilesColor = new Color32(200, 200, 200, 130);

        public static readonly int[,] AroundVectorPoints = new int[,]
            { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 }, { 1, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 } };

        public static readonly int[,] AroundPartialVectorPoints = new int[,] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };

        public static readonly int[,] AroundVectorPointsPlusTwo = new int[,]
        {
            { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 }, { 1, 1 }, { -1, -1 }, { 2, 2 }, { 0, -2 }, { -2, 0 }, { 0, 2 },
            { 2, 0 }
        };

        public static readonly int[,] ObjectSide = new int[,] { { 0, 1 }, { 1, 0 } };

        private const int ConstDefaultBackgroundOrderingLevel = 200;

        // Creates a Text object in the scene
        public static TextMesh CreateTextObject(string name, GameObject parent, string text, Vector3 localPosition,
            int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment)
        {
            var gameObject = new GameObject(name, typeof(TextMesh));
            var transform = gameObject.transform;
            transform.localPosition = localPosition;
            transform.SetParent(parent.transform, true);
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            var textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = ConstDefaultBackgroundOrderingLevel;
            return textMesh;
        }

        public static Vector3 GetMouseInWorldPosition()
        {
            if (Camera.main == null)
            {
                throw new ArgumentException(nameof(Camera));
            }

            var vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            vector.z = 0;
            return vector;
        }

        public static void PrintGrid(int[,] grid)
        {
            for (var i = 0; i < grid.GetLength(0); i++)
            {
                var row = "";
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    row += "(" + i + "," + j + ")" + grid[i, j] + " ";
                }

                Debug.Log(row);
            }

            Debug.Log(" ");
        }

        public static void PrintBussGrid(int[,] bGrid)
        {
            var output = " ";
            for (var i = 0; i < bGrid.GetLength(0); i++)
            {
                for (var j = 0; j < bGrid.GetLength(1); j++)
                {
                    if (bGrid[i, j] == -1)
                    {
                        output += " 0";
                    }
                    else
                    {
                        output += " " + bGrid[i, j];
                    }
                }

                output += "\n";
            }

            GameLog.Log(output);
        }

        public static double EuclideanDistance(int[] coordA, int[] coordB)
        {
            return Math.Sqrt(Math.Pow(coordA[0] - coordB[0], 2) + Math.Pow(coordA[1] - coordB[1], 2));
        }

        public static double EuclideanDistance(Vector3Int a, Vector3Int b)
        {
            return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
        }

        public static int[,] CloneGrid(int[,] grid)
        {
            int[,] newGrid = new int[grid.GetLength(0), grid.GetLength(1)];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    newGrid[i, j] = grid[i, j];
                }
            }

            return newGrid;
        }

        public static void PrintPath(List<Node> arr)
        {
            GameLog.Log("Printing Path");
            var s = "";
            foreach (var i in arr)
            {
                s += " " + i;
            }

            GameLog.Log(s);
        }

        public static void PrintGridPathNodes(int[,] arrayGrid)
        {
            GameLog.Log("Grid");
            for (var i = 0; i < arrayGrid.GetLength(0); i++)
            {
                var s = "";
                for (var j = 0; j < arrayGrid.GetLength(1); j++)
                {
                    s += "  .  " + arrayGrid[i, j];
                }

                GameLog.Log(s);
            }
        }

        public static Vector2Int GetXYInGameMap(Vector3 position)
        {
            return new Vector2Int(
                (int)Math.Round((position.x) * 1 / Settings.GridCellSize, MidpointRounding.AwayFromZero),
                (int)Math.Round((position.y) * 1 / Settings.GridCellSize, MidpointRounding.AwayFromZero));
        }

        public static MoveDirection GetDirectionFromVector(Vector3 vector)
        {
            if (vector == Vector3.left * Settings.GridCellSize)
            {
                return MoveDirection.Left;
            }
            else if (vector == Vector3.right * Settings.GridCellSize)
            {
                return MoveDirection.Right;
            }
            else if (vector == Vector3.up * Settings.GridCellSize)
            {
                return MoveDirection.Up;
            }
            else if (vector == Vector3.down * Settings.GridCellSize)
            {
                return MoveDirection.Down;
            }
            else if (vector == new Vector3(-1, -1, 0) * Settings.GridCellSize)
            {
                return MoveDirection.DownLeft;
            }
            else if (vector == new Vector3(1, -1, 0) * Settings.GridCellSize)
            {
                return MoveDirection.Downright;
            }
            else if (vector == new Vector3(-1, 1, 0) * Settings.GridCellSize)
            {
                return MoveDirection.UpLeft;
            }
            else if (vector == new Vector3(1, 1, 0) * Settings.GridCellSize)
            {
                return MoveDirection.Upright;
            }
            else
            {
                return MoveDirection.Idle;
            }
        }

        public static Vector3Int GetGridPositionFromMoveDirection(MoveDirection d, Vector3Int pos)
        {
            if (d == MoveDirection.Down)
            {
                return pos + new Vector3Int(0, -1, 0);
            }
            else if (d == MoveDirection.Up)
            {
                return pos + new Vector3Int(0, 1, 0);
            }
            else if (d == MoveDirection.Left)
            {
                return pos + new Vector3Int(-1, 0, 0);
            }
            else
            {
                return pos + new Vector3Int(-1, 0, 0);
            }
        }

        public static Vector3 GetVectorFromDirection(MoveDirection d)
        {
            //in case it is MoveDirection.IDLE do nothing
            Vector3 dir = new Vector3(0, 0);

            if (d == MoveDirection.Left)
            {
                dir = Vector3.left * Settings.GridCellSize;
            }
            else if (d == MoveDirection.Right)
            {
                dir = Vector3.right * Settings.GridCellSize;
            }
            else if (d == MoveDirection.Up)
            {
                dir = Vector3.up * Settings.GridCellSize;
            }
            else if (d == MoveDirection.Down)
            {
                dir = Vector3.down * Settings.GridCellSize;
            }
            else if (d == MoveDirection.DownLeft)
            {
                dir = new Vector3(-1, -1, 0) * Settings.GridCellSize;
            }
            else if (d == MoveDirection.Downright)
            {
                dir = new Vector3(1, -1, 0) * Settings.GridCellSize;
            }
            else if (d == MoveDirection.UpLeft)
            {
                dir = new Vector3(-1, 1, 0) * Settings.GridCellSize;
            }
            else if (d == MoveDirection.Upright)
            {
                dir = new Vector3(1, 1, 0) * Settings.GridCellSize;
            }

            return new Vector3(dir.x, dir.y);
        }

        // Translates a normalized angle to a direction from 0 - 360
        //       360 | 0
        //           .
        //       315 . 45 
        // 270 --------------- 90
        //       225 . 135
        //           .
        //          180

        public static MoveDirection GetDirectionFromAngles(float angle)
        {
            // offset for diagonal movements 
            float offset = 20;

            if (angle >= 45 - offset && angle <= 45 + offset)
            {
                return MoveDirection.Upright;
            }
            else if (angle >= 135 - offset && angle <= 135 + offset)
            {
                return MoveDirection.Downright;
            }
            else if (angle >= 225 - offset && angle <= 225 + offset)
            {
                return MoveDirection.DownLeft;
            }
            else if (angle >= 315 - offset && angle <= 315 + offset)
            {
                return MoveDirection.UpLeft;
            }
            else if ((angle > 315 + offset && angle <= 360) || (angle >= 0 && angle < 45 - offset))
            {
                return MoveDirection.Up;
            }
            else if (angle > 45 + offset && angle < 135 - offset)
            {
                return MoveDirection.Right;
            }
            else if (angle > 135 + offset && angle < 225 - offset)
            {
                return MoveDirection.Down;
            }
            else if (angle > 225 + offset && angle < 315 - offset)
            {
                return MoveDirection.Left;
            }
            else
            {
                return MoveDirection.Idle;
            }
        }

        public static TileType GetTileType(string tileName)
        {
            return tileName switch
            {
                "floor1" => TileType.SpamPoint,
                "WalkTile@3x" => TileType.WalkablePath,
                "floor3" => TileType.Floor3,
                "BussTile@3x" => TileType.BusFloor,
                "floor5" => TileType.Wall,
                "Complete@3x" => TileType.FloorObstacle,
                "MediumHorizontal@3x" => TileType.FloorMediumHorizontalObstacle,
                "MediumVertical@3x" => TileType.FloorMediumVerticalObstacle,
                "ShortHorizontal@3x" => TileType.FloorShortHorizontalObstacle,
                "ShortVertical@3x" => TileType.FloorShortVerticalObstacle,
                "GridTile" => TileType.IsometricGridTile,
                "Wall@3x" => TileType.Wall,
                "HighlightedFloor@3x" => TileType.FloorEdit,
                _ => TileType.Undefined
            };
        }

        public static ObjectType GetTileObjectType(TileType type)
        {
            switch (type)
            {
                case TileType.FloorObstacle:
                case TileType.FloorMediumHorizontalObstacle:
                case TileType.FloorMediumVerticalObstacle:
                case TileType.FloorShortHorizontalObstacle:
                case TileType.FloorShortVerticalObstacle:
                case TileType.IsometricGridTile:
                case TileType.Wall:
                    return ObjectType.Obstacle;
                case TileType.SpamPoint:
                case TileType.Floor3:
                case TileType.BusFloor:
                case TileType.WalkablePath:
                    return ObjectType.Floor;
                case TileType.IsometricSingleSquareObject:
                    break;
                case TileType.IsometricFourSquareObject:
                    break;
                case TileType.FloorEdit:
                    break;
                case TileType.Undefined:
                    break;
                default:
                    return ObjectType.Undefined;
            }

            return ObjectType.Undefined;
        }

        public static ObjectType GetObjectType(GameObject gameObject)
        {
            return gameObject.tag switch
            {
                Settings.NpcTag => ObjectType.Client,
                Settings.NpcEmployeeTag => ObjectType.Employee,
                _ => ObjectType.Undefined
            };
        }

        public static void PrintAllComponents(GameObject gameObject)
        {
            Component[] components = gameObject.GetComponents(typeof(Component));

            foreach (Component c in components)
            {
                GameLog.Log("Component: " + c.name + " " + c + " " + c.GetType());
            }
        }

        public static bool IsNull(GameObject gameObject, string message)
        {
            if (gameObject != null)
            {
                return false;
            }

            GameLog.LogWarning(message);
            return true;
        }

        public static bool IsNull(LoadSliderController gameObject, string message)
        {
            if (gameObject == null)
            {
                GameLog.LogWarning(message);
                return true;
            }

            return false;
        }

        public static Vector3Int GetVector3IntNegativeInfinity()
        {
            return new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
        }

        public static bool IsAtDistanceWithObject(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(new Vector3(a.x, a.y, 0), new Vector3(b.x, b.y, 0)) <=
                   0.03f; // Needs less precision for diagonal movement
        }

        // Measures the distance between a and b, and translate the transform position of the object to fix small precision problem, 0.01f
        public static bool IsAtDistanceWithObjectTranslate(Vector3 a, Vector3 b, Transform transform)
        {
            if (Vector3.Distance(new Vector3(a.x, a.y, 0), new Vector3(b.x, b.y, 0)) < Settings.MinDistanceToTarget)
            {
                transform.position = b;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static long GetUnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        // Returns the sorting given the coords of the objects
        public static int GetSorting(Vector3Int pos)
        {
            int x = pos.x;
            int y = pos.y;
            return (x + y) * -1;
        }

        public static void EnqueueToList(List<GameGridObject> list, GameGridObject obj)
        {
            list.Insert(0, obj);
        }

        public static GameGridObject DequeueFromList(List<GameGridObject> list)
        {
            if (list.Count == 0)
            {
                GameLog.Log("There is no spots to dequeue");
                return null;
            }

            GameGridObject obj = list.Last();
            list.RemoveAt(list.Count - 1);
            return obj;
        }

        public static GameGridObject PeekFromList(List<GameGridObject> list)
        {
            return list.Last();
        }

        public static Color GetRandomColor()
        {
            return new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        }

        public static Color GetRandomColor(int val)
        {
            switch (val)
            {
                case 0: return Color.black;
                case 1: return Color.blue;
                case 2: return Color.cyan;
                case 3: return Color.gray;
                case 4: return Color.green;
                case 5: return Color.grey;
                case 6: return Color.magenta;
                case 7: return Color.red;
                case 8: return Color.white;
                case 9: return Color.yellow;
                case 10: return Color.yellow;
            }

            return Color.black;
        }

        public static int[,] TransposeGridForDebugging(int[,] grid)
        {
            int w = grid.GetLength(0);
            int h = grid.GetLength(1);

            int[,] result = new int[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[h - j - 1, i] = grid[i, j];
                }
            }

            return result;
        }

        public static bool IsInternetReachable()
        {
            int count = 0;
            //Check if the device cannot reach the internet
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }
            //Check if the device can reach the internet via a carrier data network
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                // m_ReachabilityText = "Reachable via carrier data network.";
                count++;
            }
            //Check if the device can reach the internet via a LAN
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                // m_ReachabilityText = "Reachable via Local Area Network.";
                count++;
            }

            return count > 0;
        }

        public static Vector3 GetCameraPosition()
        {
            if (Camera.main == null)
            {
                throw new ArgumentException(nameof(Camera));
            }

            return Camera.main.transform.position;
        }

        public static bool CompareNegativeInfinity(Vector3 coord)
        {
            return coord.Equals(Vector3.negativeInfinity) ||
                   coord.Equals(new Vector3(Vector3.negativeInfinity.x, Vector3.negativeInfinity.y, 0));
        }

        public static bool CompareNegativeInfinity(Vector3Int coord)
        {
            return coord.Equals(GetVector3IntNegativeInfinity()) ||
                   coord.Equals(new Vector3Int(GetVector3IntNegativeInfinity().x, GetVector3IntNegativeInfinity().y, 0));
        }

        // Use camera Culling inside the animation instead
        public static void DrawMainCameraCorners()
        {
            if (Camera.main == null)
            {
                throw new ArgumentException(nameof(Camera.main));
            }

            var camera = Camera.main;
            var cameraGridPosition = BussGrid.GetLocalGridFromWorldPosition(camera.transform.position);
            const int size = 8;

            var rightBotCorner = cameraGridPosition + new Vector3Int(0, -size);
            var leftBotCorner = cameraGridPosition + new Vector3Int(-size, 0);
            var leftTopCorner = cameraGridPosition + new Vector3Int(0, size);
            var rightTopCorner = cameraGridPosition + new Vector3Int(size, 0);

            BussGrid.DrawCell(rightBotCorner);
            BussGrid.DrawCell(leftBotCorner);
            BussGrid.DrawCell(leftTopCorner);
            BussGrid.DrawCell(rightTopCorner);
        }


        public static string ConvertToTextAndReduceCurrency(double amount)
        {
            var amountStr = Math.Round(amount).ToString(CultureInfo.InvariantCulture);

            switch (amountStr.Length)
            {
                case <= 4:
                    return amount.ToString(CultureInfo.InvariantCulture);
                case > 4:
                {
                    var prefix = "";
                    for (var i = 0; i < amountStr.Length - 3; i++)
                    {
                        prefix += amountStr[i];
                    }

                    return prefix + "K";
                }
            }
        }

        public static Sprite LoadSpriteResource(string name)
        {
            Sprite sp = Resources.Load<Sprite>(name);
            if (!sp)
            {
                GameLog.LogWarning("Sprite not found SetInventoryItem() " + name);
                name = Settings.StoreSpritePath + Settings.DefaultSquareSprite;
                sp = Resources.Load<Sprite>(name);
            }

            return sp;
        }
    }
}