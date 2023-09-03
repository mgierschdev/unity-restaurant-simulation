using System.Collections.Generic;
using Game.Grid;
using UnityEngine;

// Controlled attached to Game scene and main Game Object.
namespace Game.Controllers.Other_Controllers
{
    public class ClickController : MonoBehaviour
    {
        private bool _isPressingButton, _mouseOverUI;
        private float _lastClickTime;
        private GameTile _clickedGameTile;
        private Camera _mainCamera;
        private GameObject _clickedObject;
        private GameGridObject _clickedGameGridObject;

        private void Start()
        {
            _mainCamera = Camera.main;
            // Time passed between clicks 
            _lastClickTime = 0;
        }

        private void Update()
        {
            // Controls the state of the first and long click
            ClickControl();

            //Object Clicked Control, sets the last ClickedObject
            ObjectClickedControl();
        }

        private void ClickControl()
        {
            // first click 
            if (Input.GetMouseButtonDown(0))
            {
                _lastClickTime = Time.unscaledTime;
            }
        }

        // The object must have a collider attached, used for when clicking individual NPCs or detecting long click for the player
        private void ObjectClickedControl()
        {
            if (!Input.GetMouseButtonDown(0) || _mouseOverUI)
            {
                return;
            }

            Vector3Int clickPosition =
                BussGrid.GetPathFindingGridFromWorldPosition(_mainCamera.ScreenToWorldPoint(Input.mousePosition));
            GameTile tile = BussGrid.GetGameTileFromClickInPathFindingGrid(clickPosition);
            Vector2 worldPoint = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Collider2D[] hits = Physics2D.OverlapPointAll(worldPoint);
            SortedList<GameGridObject, int> list = new SortedList<GameGridObject, int>();

            foreach (Collider2D r in hits)
            {
                if (BussGrid.GetGameGridObjectsDictionary().ContainsKey(r.name))
                {
                    GameGridObject selected = BussGrid.GetGameGridObjectsDictionary()[r.name];
                    list.Add(selected, selected.GetSortingOrder());
                }
            }

            if (list.Count > 0)
            {
                _clickedGameGridObject = list.Keys[0];
            }
            else
            {
                _clickedGameGridObject = null;
            }

            if (tile != null)
            {
                _clickedGameTile = tile;
            }
        }

        public GameGridObject GetGameGridClickedObject()
        {
            return _clickedGameGridObject;
        }
    }
}