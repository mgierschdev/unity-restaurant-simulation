using System;
using Game.Controllers.Other_Controllers;
using Game.Grid;
using Game.Players;
using UnityEngine;
using UnityEngine.Tilemaps;
using Util;

//Will handle all GetComponent and Find Calls and cache the items for future reference
namespace Game.Controllers.Grid_Objects_Controllers
{
    public class GridComponentsController : MonoBehaviour
    {
        public void Awake()
        {
            try
            {
                //Grid Components
                BussGrid.TilemapPathFinding = GameObject.Find(Settings.PathFindingGrid).GetComponent<Tilemap>();
                BussGrid.TilemapFloor = GameObject.Find(Settings.TilemapSpamFloor).GetComponent<Tilemap>();
                BussGrid.TilemapColliders = GameObject.Find(Settings.TilemapColliders).GetComponent<Tilemap>();
                //BussGrid.TilemapObjects = GameObject.Find(Settings.TilemapObjects).GetComponent<Tilemap>();
                BussGrid.TilemapWalkingPath = GameObject.Find(Settings.TilemapWalkingPath).GetComponent<Tilemap>();
                GameObject gameObj = GameObject.Find(Settings.ConstParentGameObject);
                BussGrid.GameController = gameObj.GetComponent<GameController>();
                BussGrid.ControllerGameObject = gameObject;
                //Buss TileFloor, returns it depending on the PLayer GridSize
                BussGrid.TilemapGameFloor = GameObject.Find(PlayerData.GetTileBussFloor()).GetComponent<Tilemap>();
                BussGrid.Init();
            }
            catch (Exception e)
            {
                GameLog.LogError(e.ToString());
            }
        }
    }
}